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

## How it deploys (no manual GeoServer steps)

The 7 TCSProjectTracker feature types are repointed in the svn GeoServer data_dir snapshot as **r218740**
(`C:\svn\sitkatech\trunk\ProjectFirma\GeoServerDocker\data_dir\workspaces\TCSProjectTracker\GeoServerSqlDataSource\*\featuretype.xml`):
`ProjectSimpleLocations` -> `vGeoServerTcsiProjectSimpleLocations`; the six `ProjectDetailedLocations*`
types -> `vGeoServerTcsiProjectDetailedLocations`. The existing `cqlFilter` values stay valid.

`Build\release.cmd` runs `make.cmd database <env> release` and then `make.cmd geoserverDocker <env> release`.
The geoserverDocker target (`Build\geoserverDocker.build`) mirrors the minio `data_dir` into a sandbox,
robocopy-mirrors the **local svn working copy** `GeoServerDocker\data_dir` over it, mirrors it back to minio,
and POSTs the Portainer webhook to restart the container (which also clears cached feature type schemas).
So the DB release always precedes the GeoServer deploy and nothing needs to be done by hand.

Two things to know about that target:

- It does **not** `svn update` first. It deploys whatever is in `C:\svn\sitkatech\trunk\ProjectFirma\GeoServerDocker`
  on the machine running the release. Make sure that working copy is at or past r218740 before releasing.
- `mc mirror` only uploads changed files, so file timestamps on minio show when each file last changed.

## Per-environment verification (QA, then Prod)

1. DB: `select top 1 * from dbo.vGeoServerTcsiProjectSimpleLocations` succeeds and `dbo.gt_pk_metadata`
   includes the two `vGeoServerTcsi*` rows (release script 0592).
2. Deployed config: `mc.exe cat ProjectFirma<QA|Prod>/projectfirmabucket/data_dir/workspaces/TCSProjectTracker/GeoServerSqlDataSource/ProjectSimpleLocations/featuretype.xml`
   shows `<nativeName>vGeoServerTcsiProjectSimpleLocations</nativeName>`. If it still shows the shared view, the
   geoserverDocker step either did not run or ran from a stale working copy.
3. WFS: fuels columns appear alongside the PF-2832 attributes:
   `https://<geoserver-host>/geoserver/TCSProjectTracker/wfs?service=wfs&version=2.0.0&request=DescribeFeatureType&typeNames=TCSProjectTracker:ProjectSimpleLocations`
4. Another tenant's layer (e.g. NCRPProjectTracker) does **not** carry the fuels columns.
5. The TCSI in-app Project Map still renders simple + detailed locations and stage colors (no UI change expected).

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
