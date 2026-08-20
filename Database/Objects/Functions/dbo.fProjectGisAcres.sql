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
    -- PF-2838: acreage derived from the union of a project's detailed location geometries.
    --
    -- Stored geometry is SRID 4326 planar `geometry`, so STArea() on it yields square degrees; the
    -- geography cast is what makes the result a real-world area (geodesic, WGS84 ellipsoid).
    -- MakeValid() is required on BOTH sides: some tenants' geometry is valid as `geometry` but
    -- invalid as `geography` and raises error 24144 without it.
    -- UnionAggregate first so overlapping features are not double-counted.
    --
    -- Returns NULL when the project has no detailed locations at all, and 0 when it has geometry
    -- with no area (point/line-only locations). Callers rely on that distinction.
    --
    -- Project.GisAcres caches this value; it is recomputed on write in DatabaseEntities.SaveChanges
    -- rather than evaluated by the GeoServer views, because computing it on the fly measured ~3.5s
    -- per tenant-filtered WFS query.
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
