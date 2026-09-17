if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerTcsiProjectSimpleLocations'))
	drop view dbo.vGeoServerTcsiProjectSimpleLocations
go

create view [dbo].[vGeoServerTcsiProjectSimpleLocations]
as

-- TCSI-only variant of vGeoServerProjectSimpleLocations (PF-2833): the standard column set plus the
-- per-treatment-type forest fuels reduction acreage rollup. The TCSProjectTracker GeoServer workspace
-- publishes its ProjectSimpleLocations feature type from this view; all other tenants stay on the
-- shared vGeoServerProjectSimpleLocations and never carry these columns.
-- Fuels columns are null for projects that do not report the performance measure at all, and 0 for
-- projects that report it but not that treatment type.

select
    sl.ProjectID,
    sl.PrimaryKey,
    sl.ProjectName,
    sl.ProjectLocationPoint,
    sl.TenantID,
    sl.TenantName,
    sl.ProjectDescription,
    sl.ProjectStage,
    sl.TaxonomyLeaf,
    sl.PrimaryContactOrganization,
    sl.PrimaryContactPerson,
    sl.PlanningDesignStartYear,
    sl.ImplementationStartYear,
    sl.CompletionYear,
    sl.SecuredFunding,
    sl.TargetedFunding,
    sl.EstimatedTotalCost,
    sl.GisAcres,
    -- Source (PF-2838): TCSI-only. Projects loaded through the external data-integration sync carry an
    -- ExternalID; everything else was created by a user in the tracker. The tenant's configured
    -- source-of-record name labels the value so it is self-describing ('EIP Project Tracker' for TCSI).
    case when p.ExternalID is not null
         then coalesce(nullif(ta.ProjectExternalSourceOfRecordName, ''), 'External data integration')
         else 'User-added'
    end as Source,
    sl.ProjectLastUpdated,
    ff.BiomassRemovalReportedAcres,
    ff.BiomassRemovalExpectedAcres,
    ff.BroadcastBurningReportedAcres,
    ff.BroadcastBurningExpectedAcres,
    ff.ChemicalTreatmentReportedAcres,
    ff.ChemicalTreatmentExpectedAcres,
    ff.ChippingReportedAcres,
    ff.ChippingExpectedAcres,
    ff.CulturalBurningReportedAcres,
    ff.CulturalBurningExpectedAcres,
    ff.HandPilingReportedAcres,
    ff.HandPilingExpectedAcres,
    ff.HandThinningReportedAcres,
    ff.HandThinningExpectedAcres,
    ff.HelicopterYardingReportedAcres,
    ff.HelicopterYardingExpectedAcres,
    ff.JackpotBurningReportedAcres,
    ff.JackpotBurningExpectedAcres,
    ff.MachinePilingReportedAcres,
    ff.MachinePilingExpectedAcres,
    ff.MasticationReportedAcres,
    ff.MasticationExpectedAcres,
    ff.MechanicalThinningReportedAcres,
    ff.MechanicalThinningExpectedAcres,
    ff.PileBurningReportedAcres,
    ff.PileBurningExpectedAcres,
    ff.PrescribedBurningReportedAcres,
    ff.PrescribedBurningExpectedAcres,
    ff.PrescribedTargetedGrazingReportedAcres,
    ff.PrescribedTargetedGrazingExpectedAcres,
    ff.PruningReportedAcres,
    ff.PruningExpectedAcres,
    ff.SalvageCutReportedAcres,
    ff.SalvageCutExpectedAcres,
    ff.UnspecifiedTreatmentTypeReportedAcres,
    ff.UnspecifiedTreatmentTypeExpectedAcres

from dbo.vGeoServerProjectSimpleLocations sl
left join dbo.vGeoServerTcsiForestFuelsTreatmentAcres ff on sl.ProjectID = ff.ProjectID
join dbo.Project p on sl.ProjectID = p.ProjectID
left join dbo.TenantAttribute ta on sl.TenantID = ta.TenantID
where sl.TenantName = 'TCSProjectTracker'
