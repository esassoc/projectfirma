if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerProjectSimpleLocations'))
	drop view dbo.vGeoServerProjectSimpleLocations
go

create view [dbo].[vGeoServerProjectSimpleLocations]
as

select
	p.ProjectID,
    p.ProjectID as PrimaryKey,
    p.ProjectName,
    p.ProjectLocationPoint,
    p.TenantID,
    t.TenantName,
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
    pa.ProjectLastUpdated
from
	dbo.Project p
	join dbo.Tenant t on p.TenantID = t.TenantID
	join dbo.vGeoServerProjectAttributes pa on p.ProjectID = pa.ProjectID
    where LocationIsPrivate = 0 and ProjectLocationPoint is not null