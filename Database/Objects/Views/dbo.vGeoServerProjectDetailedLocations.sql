if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerProjectDetailedLocations'))
	drop view dbo.vGeoServerProjectDetailedLocations
go

create view [dbo].[vGeoServerProjectDetailedLocations]
as

select 
    pl.ProjectLocationID,
    pl.ProjectLocationID as PrimaryKey,
    pl.ProjectID,
    p.ProjectName,
    pl.ProjectLocationGeometry,
    pl.TenantID,
    t.TenantName,
	p.LocationIsPrivate,
	p.ProjectApprovalStatusID,
	pl.ProjectLocationGeometry.STGeometryType() as GeometryType,
	p.ProjectStageID,
	coalesce(nullif(pscl.ProjectStageColor, ''), ps.ProjectStageColor) as ProjectStageColor,
	pa.ProjectDescription,
	pa.ProjectStage,
	pa.TaxonomyLeaf,
	pa.PrimaryContactOrganization,
	pa.PrimaryContactPerson,
	pa.PlanningDesignStartYear,
	pa.ImplementationStartYear,
	pa.CompletionYear,
	pa.SecuredFunding,
	pa.TargetedFunding,
	pa.EstimatedTotalCost,
	pa.GisAcres,
	pa.ProjectLastUpdated

from dbo.ProjectLocation pl
join dbo.Project p on pl.ProjectID = p.ProjectID
join dbo.Tenant t on pl.TenantID = t.TenantID
join dbo.ProjectStage ps on p.ProjectStageID = ps.ProjectStageID
left join dbo.ProjectStageCustomLabel pscl on p.ProjectStageID = pscl.ProjectStageID and p.TenantID = pscl.TenantID
join dbo.vGeoServerProjectAttributes pa on p.ProjectID = pa.ProjectID
where p.LocationIsPrivate = 0