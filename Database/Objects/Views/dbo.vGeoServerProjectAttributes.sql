if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerProjectAttributes'))
	drop view dbo.vGeoServerProjectAttributes
go

create view [dbo].[vGeoServerProjectAttributes]
as

-- Standard project-level attributes carried on every GeoServer WFS/WMS project layer (PF-2832).
-- Joined by ProjectID from vGeoServerProjectSimpleLocations and vGeoServerProjectDetailedLocations,
-- so it must return exactly one row per project to keep those layers' primary keys unique.

select
    p.ProjectID,
    p.ProjectID as PrimaryKey,
    p.ProjectDescription,
    coalesce(nullif(pscl.ProjectStageLabel, ''), ps.ProjectStageDisplayName) as ProjectStage,
    case when tl.TaxonomyLeafCode is not null and tl.TaxonomyLeafCode != '' then tl.TaxonomyLeafCode + ': ' + tl.TaxonomyLeafName else tl.TaxonomyLeafName end as TaxonomyLeaf,
    pco.PrimaryContactOrganization,
    case when person.PersonID is not null then person.FirstName + ' ' + person.LastName
         else nullif(p.PrimaryContactPersonFullName, '') end as PrimaryContactPerson,
    p.PlanningDesignStartYear,
    p.ImplementationStartYear,
    p.CompletionYear,
    coalesce(pfsb.SecuredFunding, 0) as SecuredFunding,
    coalesce(pfsb.TargetedFunding, 0) as TargetedFunding,
    coalesce(pfsb.SecuredFunding, 0) + coalesce(pfsb.TargetedFunding, 0) + coalesce(pnfsi.NoFundingSourceIdentified, 0) as EstimatedTotalCost,
    p.LastUpdatedDate as ProjectLastUpdated

from dbo.Project p
join dbo.ProjectStage ps on p.ProjectStageID = ps.ProjectStageID
left join dbo.ProjectStageCustomLabel pscl on p.ProjectStageID = pscl.ProjectStageID and p.TenantID = pscl.TenantID
join dbo.TaxonomyLeaf tl on p.TaxonomyLeafID = tl.TaxonomyLeafID
left join dbo.Person person on p.PrimaryContactPersonID = person.PersonID
outer apply
(
    select top 1 coalesce(o.OrganizationShortName, o.OrganizationName) as PrimaryContactOrganization
    from dbo.ProjectOrganization po
    join dbo.OrganizationRelationshipType ort on po.OrganizationRelationshipTypeID = ort.OrganizationRelationshipTypeID
    join dbo.Organization o on po.OrganizationID = o.OrganizationID
    where po.ProjectID = p.ProjectID and ort.IsPrimaryContact = 1
    order by po.ProjectOrganizationID
) pco
left join
(
    select b.ProjectID, sum(b.SecuredAmount) as SecuredFunding, sum(b.TargetedAmount) as TargetedFunding
    from dbo.ProjectFundingSourceBudget b
    group by b.ProjectID
) pfsb on p.ProjectID = pfsb.ProjectID
left join
(
    select nfsi.ProjectID, sum(nfsi.NoFundingSourceIdentifiedYet) as NoFundingSourceIdentified
    from dbo.ProjectNoFundingSourceIdentified nfsi
    group by nfsi.ProjectID
) pnfsi on p.ProjectID = pnfsi.ProjectID
