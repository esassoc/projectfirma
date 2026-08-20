-- PF-2838: expose a geometry-derived "GIS Acres" attribute on every tenant's GeoServer project
-- layers, and a TCSI-only "Source" attribute distinguishing user-added projects from projects
-- loaded through the external data-integration sync.
--
-- Source needs no schema change: it is derived in the vGeoServerTcsiProject* views from
-- Project.ExternalID plus the tenant's configured TenantAttribute.ProjectExternalSourceOfRecordName.
--
-- GIS Acres is stored rather than computed in the views. Computing it on the fly measured ~3.5s per
-- tenant-filtered WFS query (the optimizer does not push the tenant predicate into the geometry
-- rollup), and these views also back the in-app Project Map and WMS tile rendering. It is instead
-- recomputed on write in DatabaseEntities.SaveChanges whenever a ProjectLocation is added, modified
-- or deleted.

alter table dbo.Project add GisAcres decimal(18,2) null
GO

-- dbo.fProjectGisAcres is also maintained in Database/Objects/Functions. It is defined here as well
-- so the backfill below does not depend on the Objects/Functions folder having been deployed first.
IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.fProjectGisAcres'))
drop function dbo.fProjectGisAcres
GO
CREATE FUNCTION dbo.fProjectGisAcres
(
    @piProjectID int
)
RETURNS decimal(18,2)
AS
BEGIN
    -- Stored geometry is SRID 4326 planar `geometry`, so STArea() on it yields square degrees; the
    -- geography cast is what makes the result a real-world area. MakeValid() is required on BOTH
    -- sides -- some tenants' geometry is valid as `geometry` but invalid as `geography` (error 24144).
    -- UnionAggregate first so overlapping features are not double-counted.
    -- Returns NULL when the project has no detailed locations, 0 for point/line-only geometry.
    return
    (
        select round(
            geography::STGeomFromWKB(
                geometry::UnionAggregate(pl.ProjectLocationGeometry.MakeValid())
                    .MakeValid().STAsBinary(), 4326)
                .MakeValid().STArea() / 4046.8564224, 2)
        from dbo.ProjectLocation pl
        where pl.ProjectID = @piProjectID
    )
END
GO

-- One-time backfill for existing projects. Only projects that actually have detailed locations are
-- touched; every other project keeps GisAcres = null, which is the "no mapped geometry" signal.
update p
set GisAcres = dbo.fProjectGisAcres(p.ProjectID)
from dbo.Project p
where exists (select 1 from dbo.ProjectLocation pl where pl.ProjectID = p.ProjectID)
GO
