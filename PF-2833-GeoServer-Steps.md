# PF-2833 — GeoServer steps to expose TCSI forest fuels treatment acreage on WFS/WMS

The DB side of PF-2833 adds three views:

- `dbo.vGeoServerTcsiForestFuelsTreatmentAcres` — one row per TCSI project reporting the
  "Acres of Forest Fuels Reduction Treatment" performance measure; a `<TreatmentType>ReportedAcres` /
  `<TreatmentType>ExpectedAcres` column pair for each of the 18 Treatment Type options, summed across
  all reporting years and all Treatment Phases / Critical Zone permutations.
- `dbo.vGeoServerTcsiProjectSimpleLocations` — the standard PF-2832 column set plus the 36 fuels columns.
- `dbo.vGeoServerTcsiProjectDetailedLocations` — same, for detailed locations.

Fuels columns are `null` for projects that do not report the measure at all, and `0` for projects that
report it but not that treatment type. Release script `0592` registers the two location views in
`dbo.gt_pk_metadata` so WFS feature IDs keep using `PrimaryKey`.

These columns are **TCSI-only**: the shared `vGeoServerProjectSimpleLocations` /
`vGeoServerProjectDetailedLocations` views are unchanged, and only the TCSProjectTracker workspace is
repointed. No other tenant's layers change, and no app code changes (layer names stay the same).

## Per-environment steps (QA, then Prod) — after the DB release has run

1. Confirm the DB release ran: `select top 1 * from dbo.vGeoServerTcsiProjectSimpleLocations` succeeds
   and `select * from dbo.gt_pk_metadata` includes the two `vGeoServerTcsi*` rows.
2. Repoint the 7 TCSProjectTracker feature types to the TCSI views. The admin UI cannot edit a
   published layer's native name, so use REST (or edit the data_dir XML and reload):

   ```
   # ProjectSimpleLocations -> vGeoServerTcsiProjectSimpleLocations
   curl -u <admin> -X PUT -H "Content-type: text/xml" \
     -d "<featureType><nativeName>vGeoServerTcsiProjectSimpleLocations</nativeName></featureType>" \
     https://<geoserver-host>/geoserver/rest/workspaces/TCSProjectTracker/datastores/GeoServerSqlDataSource/featuretypes/ProjectSimpleLocations.xml

   # Each detailed feature type -> vGeoServerTcsiProjectDetailedLocations
   for ft in ProjectDetailedLocations ProjectDetailedLocationsLineString ProjectDetailedLocationsMultiPolygon \
             ProjectDetailedLocationsPoint ProjectDetailedLocationsPolygon ProjectDetailedLocationsPublicApproved; do
     curl -u <admin> -X PUT -H "Content-type: text/xml" \
       -d "<featureType><nativeName>vGeoServerTcsiProjectDetailedLocations</nativeName></featureType>" \
       https://<geoserver-host>/geoserver/rest/workspaces/TCSProjectTracker/datastores/GeoServerSqlDataSource/featuretypes/$ft.xml
   done
   ```

   The existing `cqlFilter` values (`TenantName = 'TCSProjectTracker'` etc.) stay valid — the TCSI views
   keep the `TenantName` column (the filter is now redundant with the views' own tenant restriction,
   but harmless).
3. Clear cached feature type schemas: `curl -u <admin> -X POST https://<geoserver-host>/geoserver/rest/reset`
   (or admin UI **Server Status → Reload**).
4. Spot-check WFS — the fuels columns should appear alongside the PF-2832 attributes:
   `https://<geoserver-host>/geoserver/TCSProjectTracker/wfs?service=wfs&version=2.0.0&request=GetFeature&typeNames=TCSProjectTracker:ProjectDetailedLocationsPublicApproved&outputFormat=application/json&count=1`
5. Spot-check another tenant's layer (e.g. NCRPProjectTracker) — it should **not** carry the fuels columns.
6. Confirm the TCSI in-app Project Map still renders simple + detailed locations and stage colors
   (no UI change expected).
7. Commit the `nativeName` edits in the svn data_dir snapshot
   (`C:\svn\sitkatech\trunk\ProjectFirma\GeoServerDocker\data_dir\workspaces\TCSProjectTracker\GeoServerSqlDataSource\*\featuretype.xml`)
   so the Docker image stays reproducible. The 7 files are already edited in the local svn working
   copy (2026-08-04) — do **not** commit them to svn until the QA DB release has run, or a rebuilt
   GeoServer container would point at views that don't exist yet.

## Caveats for GIS data consumers

- WFS columns are static. A Treatment Type option added in the app will not surface until a matching
  column pair is added to `dbo.vGeoServerTcsiForestFuelsTreatmentAcres` (this is inherent to the
  no-1-to-many-in-WFS rule; PF-2833 is the approved aggregation exception). The view resolves the
  performance measure and subcategory by display name, so renaming the PM, the "Treatment Type"
  subcategory, or an option in the app silently empties the corresponding column(s) — treat those
  names as part of the interface.
- Shapefile exports truncate attribute names to 10 characters (DBF limit), which mangles these long
  column names; GeoJSON, GML, and CSV outputs are unaffected.
- Values are cumulative across all reporting years (per the contract, no per-year breakdown).
