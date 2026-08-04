if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerTcsiForestFuelsTreatmentAcres'))
	drop view dbo.vGeoServerTcsiForestFuelsTreatmentAcres
go

create view [dbo].[vGeoServerTcsiForestFuelsTreatmentAcres]
as

-- TCSI-only rollup of the "Acres of Forest Fuels Reduction Treatment" performance measure (PF-2833).
-- One row per TCSI project that reports the measure; one Reported/Expected column pair per Treatment Type
-- option, summed across all reporting years and all Treatment Phases / Critical Zone permutations.
-- Joined by ProjectID from vGeoServerTcsiProjectSimpleLocations and vGeoServerTcsiProjectDetailedLocations,
-- so it must return exactly one row per project to keep those layers' primary keys unique.
-- The performance measure and subcategory are resolved by display name (IDs are identity-generated and
-- differ across environments). WFS columns are static: a Treatment Type option added in the app will not
-- surface over WFS/WMS until a matching column pair is added here.

with FuelsTreatmentAcres as
(
    select
        pma.ProjectID,
        rtrim(ltrim(pmso.PerformanceMeasureSubcategoryOptionName)) as TreatmentType,
        pma.ActualValue as ReportedAcres,
        cast(null as float) as ExpectedAcres
    from dbo.Tenant t
    join dbo.PerformanceMeasure pm on t.TenantID = pm.TenantID
    join dbo.PerformanceMeasureSubcategory pms on pm.PerformanceMeasureID = pms.PerformanceMeasureID
    join dbo.PerformanceMeasureActual pma on pm.PerformanceMeasureID = pma.PerformanceMeasureID
    join dbo.PerformanceMeasureActualSubcategoryOption pmaso on pma.PerformanceMeasureActualID = pmaso.PerformanceMeasureActualID
        and pms.PerformanceMeasureSubcategoryID = pmaso.PerformanceMeasureSubcategoryID
    join dbo.PerformanceMeasureSubcategoryOption pmso on pmaso.PerformanceMeasureSubcategoryOptionID = pmso.PerformanceMeasureSubcategoryOptionID
    where t.TenantName = 'TCSProjectTracker'
        and pm.PerformanceMeasureDisplayName = 'Acres of Forest Fuels Reduction Treatment'
        and pms.PerformanceMeasureSubcategoryDisplayName = 'Treatment Type'

    union all

    select
        pme.ProjectID,
        rtrim(ltrim(pmso.PerformanceMeasureSubcategoryOptionName)) as TreatmentType,
        cast(null as float) as ReportedAcres,
        pme.ExpectedValue as ExpectedAcres
    from dbo.Tenant t
    join dbo.PerformanceMeasure pm on t.TenantID = pm.TenantID
    join dbo.PerformanceMeasureSubcategory pms on pm.PerformanceMeasureID = pms.PerformanceMeasureID
    join dbo.PerformanceMeasureExpected pme on pm.PerformanceMeasureID = pme.PerformanceMeasureID
    join dbo.PerformanceMeasureExpectedSubcategoryOption pmeso on pme.PerformanceMeasureExpectedID = pmeso.PerformanceMeasureExpectedID
        and pms.PerformanceMeasureSubcategoryID = pmeso.PerformanceMeasureSubcategoryID
    join dbo.PerformanceMeasureSubcategoryOption pmso on pmeso.PerformanceMeasureSubcategoryOptionID = pmso.PerformanceMeasureSubcategoryOptionID
    where t.TenantName = 'TCSProjectTracker'
        and pm.PerformanceMeasureDisplayName = 'Acres of Forest Fuels Reduction Treatment'
        and pms.PerformanceMeasureSubcategoryDisplayName = 'Treatment Type'
)
select
    ProjectID,
    ProjectID as PrimaryKey,
    coalesce(sum(case when TreatmentType = 'Biomass Removal' then ReportedAcres end), 0) as BiomassRemovalReportedAcres,
    coalesce(sum(case when TreatmentType = 'Biomass Removal' then ExpectedAcres end), 0) as BiomassRemovalExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Broadcast Burning' then ReportedAcres end), 0) as BroadcastBurningReportedAcres,
    coalesce(sum(case when TreatmentType = 'Broadcast Burning' then ExpectedAcres end), 0) as BroadcastBurningExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Chemical Treatment' then ReportedAcres end), 0) as ChemicalTreatmentReportedAcres,
    coalesce(sum(case when TreatmentType = 'Chemical Treatment' then ExpectedAcres end), 0) as ChemicalTreatmentExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Chipping' then ReportedAcres end), 0) as ChippingReportedAcres,
    coalesce(sum(case when TreatmentType = 'Chipping' then ExpectedAcres end), 0) as ChippingExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Cultural Burning' then ReportedAcres end), 0) as CulturalBurningReportedAcres,
    coalesce(sum(case when TreatmentType = 'Cultural Burning' then ExpectedAcres end), 0) as CulturalBurningExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Hand Piling' then ReportedAcres end), 0) as HandPilingReportedAcres,
    coalesce(sum(case when TreatmentType = 'Hand Piling' then ExpectedAcres end), 0) as HandPilingExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Hand Thinning' then ReportedAcres end), 0) as HandThinningReportedAcres,
    coalesce(sum(case when TreatmentType = 'Hand Thinning' then ExpectedAcres end), 0) as HandThinningExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Helicopter Yarding' then ReportedAcres end), 0) as HelicopterYardingReportedAcres,
    coalesce(sum(case when TreatmentType = 'Helicopter Yarding' then ExpectedAcres end), 0) as HelicopterYardingExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Jackpot Burning' then ReportedAcres end), 0) as JackpotBurningReportedAcres,
    coalesce(sum(case when TreatmentType = 'Jackpot Burning' then ExpectedAcres end), 0) as JackpotBurningExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Machine Piling' then ReportedAcres end), 0) as MachinePilingReportedAcres,
    coalesce(sum(case when TreatmentType = 'Machine Piling' then ExpectedAcres end), 0) as MachinePilingExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Mastication' then ReportedAcres end), 0) as MasticationReportedAcres,
    coalesce(sum(case when TreatmentType = 'Mastication' then ExpectedAcres end), 0) as MasticationExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Mechanical Thinning' then ReportedAcres end), 0) as MechanicalThinningReportedAcres,
    coalesce(sum(case when TreatmentType = 'Mechanical Thinning' then ExpectedAcres end), 0) as MechanicalThinningExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Pile Burning' then ReportedAcres end), 0) as PileBurningReportedAcres,
    coalesce(sum(case when TreatmentType = 'Pile Burning' then ExpectedAcres end), 0) as PileBurningExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Prescribed Burning' then ReportedAcres end), 0) as PrescribedBurningReportedAcres,
    coalesce(sum(case when TreatmentType = 'Prescribed Burning' then ExpectedAcres end), 0) as PrescribedBurningExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Prescribed/Targeted Grazing' then ReportedAcres end), 0) as PrescribedTargetedGrazingReportedAcres,
    coalesce(sum(case when TreatmentType = 'Prescribed/Targeted Grazing' then ExpectedAcres end), 0) as PrescribedTargetedGrazingExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Pruning' then ReportedAcres end), 0) as PruningReportedAcres,
    coalesce(sum(case when TreatmentType = 'Pruning' then ExpectedAcres end), 0) as PruningExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Salvage Cut' then ReportedAcres end), 0) as SalvageCutReportedAcres,
    coalesce(sum(case when TreatmentType = 'Salvage Cut' then ExpectedAcres end), 0) as SalvageCutExpectedAcres,
    coalesce(sum(case when TreatmentType = 'Unspecified' then ReportedAcres end), 0) as UnspecifiedTreatmentTypeReportedAcres,
    coalesce(sum(case when TreatmentType = 'Unspecified' then ExpectedAcres end), 0) as UnspecifiedTreatmentTypeExpectedAcres
from FuelsTreatmentAcres
group by ProjectID
