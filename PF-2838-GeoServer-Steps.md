# PF-2838 — GeoServer steps to expose Source and GIS Acres on WFS/WMS

The DB side of PF-2838 adds two attributes to the GeoServer project layers:

- **`GisAcres`** — acreage derived from the union of a project's detailed location geometries.
  **Uniform: every tenant's layers carry it.** Added to `dbo.vGeoServerProjectAttributes`, so it flows
  into `dbo.vGeoServerProjectSimpleLocations` and `dbo.vGeoServerProjectDetailedLocations`, and from
  there into the TCSI-only views.
- **`Source`** — `'User-added'` vs the tenant's configured source-of-record name
  (`'EIP Project Tracker'` for TCSProjectTracker). **TCSI-only:** added to
  `dbo.vGeoServerTcsiProjectSimpleLocations` and `dbo.vGeoServerTcsiProjectDetailedLocations` only.
  No other tenant's WFS output gains this column.

Release script `0594` adds `Project.GisAcres`, creates `dbo.fProjectGisAcres`, and backfills existing
projects. No `gt_pk_metadata` change is needed — the `PrimaryKey` columns are unchanged and rows
already exist for all four published location views (release scripts 0573 and 0592).

Unlike PF-2833, **no `nativeName` repointing and no data_dir/svn changes are required.** We edited
existing views rather than adding new ones, and `featuretype.xml` does not enumerate attributes, so
GeoServer rediscovers the columns from the views on a catalog reset.

## Per-environment steps (QA, then Prod) — after the DB release has run

1. Confirm the DB release ran:
   ```sql
   select top 1 GisAcres from dbo.Project
   select top 1 Source from dbo.vGeoServerTcsiProjectDetailedLocations
   ```
   Both must succeed. Also confirm the backfill populated values:
   `select count(*) from dbo.Project where GisAcres is not null` should equal the number of projects
   that have `ProjectLocation` rows.
2. Clear cached feature-type schemas so GeoServer rediscovers the new columns:
   `curl -u <admin> -X POST https://<geoserver-host>/geoserver/rest/reset`
   (or admin UI **Server Status → Reload**). `/rest/reset` clears feature type schemas; `/rest/reload`
   only re-reads config and is not sufficient.
3. Spot-check TCSI over WFS — both `GisAcres` and `Source` should appear:
   `https://<geoserver-host>/geoserver/TCSProjectTracker/wfs?service=wfs&version=2.0.0&request=GetFeature&typeNames=TCSProjectTracker:ProjectDetailedLocationsPublicApproved&outputFormat=application/json&count=1`
4. Spot-check another tenant (e.g. NCRPProjectTracker) — `GisAcres` present, `Source` **absent**.
5. Confirm the in-app Project Map still renders simple + detailed locations and stage colors
   (no UI change expected — neither attribute is surfaced in the tracker).

## How GisAcres is maintained

`Project.GisAcres` is a stored value, not computed by the views. Computing it on the fly measured
**~3.5s per tenant-filtered WFS query** (the optimizer does not push the tenant predicate into the
geometry rollup), and these views also back the in-app Project Map and WMS tile rendering. After the
change the same query runs in ~58ms.

It is recomputed on write in `DatabaseEntities.SaveChanges` whenever a `ProjectLocation` is added,
modified, or deleted — so WFS is never stale, and untouched projects are never recomputed. The hook
lives in `SaveChangesImpl` **and** `SaveChangesWithNoAuditing`; the second is required because
`SyncProjectsForTscProjectTrackerBackgroundJob` saves through it and would otherwise never update
acreage for the ~375 externally synced TCSI projects.

The recompute is a set-based `update ... set GisAcres = dbo.fProjectGisAcres(ProjectID)` issued as raw
SQL, deliberately: this is a derived value, so it should neither generate `AuditLog` rows nor bump
`Project.LastUpdatedDate`. Worst-case single-project recompute measured 234ms (a project with 4,005
location rows); typical projects are single-digit ms.

## Caveats for GIS data consumers

- **`GisAcres` null vs 0.** `null` means the project has no detailed location geometry at all. `0`
  means it has geometry with no area — point or line locations only. Note that because the value is
  rounded to 2 decimals, a sliver polygon under 0.005 acres also reports `0.00`.
- **Units and method.** International acres (m² / 4046.8564224), 2 decimal places. Area is geodesic on
  the WGS84 ellipsoid: stored geometry is SRID 4326 planar `geometry`, so `STArea()` on it would return
  square degrees; the view function casts to `geography` first. Overlapping features within one project
  are unioned, so they are not double-counted.
- **`GisAcres` is project-level, not per-feature.** On the detailed-locations layers every feature
  belonging to the same project carries the same project-total acreage. Do not sum it across features.
- **`GisAcres` is unrelated to the PF-2833 fuels acreage columns**, which are user-reported performance
  measure values, and to any user-entered acreage custom attribute.
- **`Source` reflects `Project.ExternalID`.** A project counts as externally sourced when it has an
  `ExternalID`. That field is admin-editable in the project wizard, so clearing or setting it changes
  the reported provenance. The label comes from `TenantAttribute.ProjectExternalSourceOfRecordName`, so
  renaming that tenant attribute changes the attribute's values — treat it as part of the interface.
- **Known gap:** projects created through the admin XLSX bulk upload report as `'User-added'`. That code
  path leaves no distinguishing mark on the `Project` row (it is byte-for-byte identical to a
  wizard-created project), so it cannot currently be told apart. If TCSI considers bulk upload "data
  integration", closing this needs a real provenance column — a separate story.
- Both column names are well under the 10-character DBF limit, so unlike PF-2833's long fuels column
  names they survive shapefile export intact.

## Data-quality note worth raising with the TCSI PM

23 projects compute to over 1,000 km², with a maximum of ~12.1M acres (~49,000 km²). These may be
legitimate basin-wide polygons or may be bad imported geometry. Worth eyeballing before TCSI relies on
the numbers.
