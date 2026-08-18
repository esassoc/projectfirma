-- PF-2836: Organize Project Map layers into a grouped, ordered layer list.
--
-- Adds an optional group name and sort order to the two tables that back map layers, plus the two
-- group names for the layers that are not row-backed (Mapped Projects, Detailed Location, Streets).
-- Every column is nullable and left NULL for all tenants except TCS Project Tracker (TenantID 14),
-- so every other tenant keeps the existing ungrouped, insertion-ordered layer control.

ALTER TABLE dbo.ExternalMapLayer ADD
    MapLayerGroupName varchar(100) NULL,
    SortOrder int NULL
GO

ALTER TABLE dbo.GeospatialAreaType ADD
    MapLayerGroupName varchar(100) NULL,
    SortOrder int NULL
GO

ALTER TABLE dbo.TenantAttribute ADD
    ProjectDataMapLayerGroupName varchar(100) NULL,
    ReferenceMapLayerGroupName varchar(100) NULL
GO

-- TCS Project Tracker (TenantID 14) layout. SortOrder is a single sequence across both tables so the
-- group headings render in the requested order; the layers that are not row-backed occupy 1-3 and are
-- ordered in code.
UPDATE dbo.TenantAttribute
SET ProjectDataMapLayerGroupName = 'Project Data',
    ReferenceMapLayerGroupName = 'Reference Layers'
WHERE TenantID = 14
GO

UPDATE gat
SET MapLayerGroupName = 'Reference Layers',
    SortOrder = x.SortOrder
FROM dbo.GeospatialAreaType gat
JOIN (VALUES
    ('County', 4),
    ('National Forest', 5),
    ('Watershed - HUC 8', 6),
    ('Watershed - HUC 10', 7),
    ('WUI Zone', 8)
) AS x (GeospatialAreaTypeName, SortOrder)
    ON x.GeospatialAreaTypeName = gat.GeospatialAreaTypeName
WHERE gat.TenantID = 14
GO

-- Alphabetical by DisplayName, as requested.
UPDATE eml
SET MapLayerGroupName = 'Collaborative Groups',
    SortOrder = x.SortOrder
FROM dbo.ExternalMapLayer eml
JOIN (VALUES
    ('French Meadows Partnership', 9),
    ('Headwaters Connect', 10),
    ('Health Eldorado Landscape Partnership', 11),
    ('Middle Truckee River Watershed Forest Partnership', 12),
    ('North Yuba Forest Partnership', 13),
    ('Tahoe Fire and Fuels Team', 14),
    ('Tahoe-Central Sierra Initiative', 15)
) AS x (DisplayName, SortOrder)
    ON x.DisplayName = eml.DisplayName
WHERE eml.TenantID = 14
GO
