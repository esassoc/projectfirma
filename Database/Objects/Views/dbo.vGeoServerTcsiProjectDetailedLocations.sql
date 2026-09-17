if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerTcsiProjectDetailedLocations'))
	drop view dbo.vGeoServerTcsiProjectDetailedLocations
go

create view [dbo].[vGeoServerTcsiProjectDetailedLocations]
as

-- TCSI-only variant of vGeoServerProjectDetailedLocations (PF-2833): the standard column set plus the
-- per-treatment-type forest fuels reduction acreage rollup. The TCSProjectTracker GeoServer workspace
-- publishes its ProjectDetailedLocations* feature types from this view; all other tenants stay on the
-- shared vGeoServerProjectDetailedLocations and never carry these columns.
-- Fuels columns are null for projects that do not report the performance measure at all, and 0 for
-- projects that report it but not that treatment type.

select
    dl.ProjectLocationID,
    dl.PrimaryKey,
    dl.ProjectID,
    dl.ProjectName,
    dl.ProjectLocationGeometry,
    dl.TenantID,
    dl.TenantName,
    dl.LocationIsPrivate,
    dl.ProjectApprovalStatusID,
    dl.GeometryType,
    dl.ProjectStageID,
    dl.ProjectStageColor,
    dl.ProjectDescription,
    dl.ProjectStage,
    dl.TaxonomyLeaf,
    dl.PrimaryContactOrganization,
    dl.PrimaryContactPerson,
    dl.PlanningDesignStartYear,
    dl.ImplementationStartYear,
    dl.CompletionYear,
    dl.SecuredFunding,
    dl.TargetedFunding,
    dl.EstimatedTotalCost,
    dl.GisAcres,
    -- Source (PF-2838): TCSI-only. Projects loaded through the external data-integration sync carry an
    -- ExternalID; everything else was created by a user in the tracker. The tenant's configured
    -- source-of-record name labels the value so it is self-describing ('EIP Project Tracker' for TCSI).
    case when p.ExternalID is not null
         then coalesce(nullif(ta.ProjectExternalSourceOfRecordName, ''), 'External data integration')
         else 'User-added'
    end as Source,
    dl.ProjectLastUpdated,
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

from dbo.vGeoServerProjectDetailedLocations dl
left join dbo.vGeoServerTcsiForestFuelsTreatmentAcres ff on dl.ProjectID = ff.ProjectID
join dbo.Project p on dl.ProjectID = p.ProjectID
left join dbo.TenantAttribute ta on dl.TenantID = ta.TenantID
where dl.TenantName = 'TCSProjectTracker'
