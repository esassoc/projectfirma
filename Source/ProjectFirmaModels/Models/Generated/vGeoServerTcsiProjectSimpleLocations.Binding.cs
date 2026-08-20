//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vGeoServerTcsiProjectSimpleLocations]
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using CodeFirstStoreFunctions;
using LtInfo.Common;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;

namespace ProjectFirmaModels.Models
{
    public partial class vGeoServerTcsiProjectSimpleLocations
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vGeoServerTcsiProjectSimpleLocations()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vGeoServerTcsiProjectSimpleLocations(int projectID, int primaryKey, string projectName, int tenantID, string tenantName, string projectDescription, string projectStage, string taxonomyLeaf, string primaryContactOrganization, string primaryContactPerson, int? planningDesignStartYear, int? implementationStartYear, int? completionYear, decimal? securedFunding, decimal? targetedFunding, decimal? estimatedTotalCost, decimal? gisAcres, string source, DateTime projectLastUpdated, double? biomassRemovalReportedAcres, double? biomassRemovalExpectedAcres, double? broadcastBurningReportedAcres, double? broadcastBurningExpectedAcres, double? chemicalTreatmentReportedAcres, double? chemicalTreatmentExpectedAcres, double? chippingReportedAcres, double? chippingExpectedAcres, double? culturalBurningReportedAcres, double? culturalBurningExpectedAcres, double? handPilingReportedAcres, double? handPilingExpectedAcres, double? handThinningReportedAcres, double? handThinningExpectedAcres, double? helicopterYardingReportedAcres, double? helicopterYardingExpectedAcres, double? jackpotBurningReportedAcres, double? jackpotBurningExpectedAcres, double? machinePilingReportedAcres, double? machinePilingExpectedAcres, double? masticationReportedAcres, double? masticationExpectedAcres, double? mechanicalThinningReportedAcres, double? mechanicalThinningExpectedAcres, double? pileBurningReportedAcres, double? pileBurningExpectedAcres, double? prescribedBurningReportedAcres, double? prescribedBurningExpectedAcres, double? prescribedTargetedGrazingReportedAcres, double? prescribedTargetedGrazingExpectedAcres, double? pruningReportedAcres, double? pruningExpectedAcres, double? salvageCutReportedAcres, double? salvageCutExpectedAcres, double? unspecifiedTreatmentTypeReportedAcres, double? unspecifiedTreatmentTypeExpectedAcres) : this()
        {
            this.ProjectID = projectID;
            this.PrimaryKey = primaryKey;
            this.ProjectName = projectName;
            this.TenantID = tenantID;
            this.TenantName = tenantName;
            this.ProjectDescription = projectDescription;
            this.ProjectStage = projectStage;
            this.TaxonomyLeaf = taxonomyLeaf;
            this.PrimaryContactOrganization = primaryContactOrganization;
            this.PrimaryContactPerson = primaryContactPerson;
            this.PlanningDesignStartYear = planningDesignStartYear;
            this.ImplementationStartYear = implementationStartYear;
            this.CompletionYear = completionYear;
            this.SecuredFunding = securedFunding;
            this.TargetedFunding = targetedFunding;
            this.EstimatedTotalCost = estimatedTotalCost;
            this.GisAcres = gisAcres;
            this.Source = source;
            this.ProjectLastUpdated = projectLastUpdated;
            this.BiomassRemovalReportedAcres = biomassRemovalReportedAcres;
            this.BiomassRemovalExpectedAcres = biomassRemovalExpectedAcres;
            this.BroadcastBurningReportedAcres = broadcastBurningReportedAcres;
            this.BroadcastBurningExpectedAcres = broadcastBurningExpectedAcres;
            this.ChemicalTreatmentReportedAcres = chemicalTreatmentReportedAcres;
            this.ChemicalTreatmentExpectedAcres = chemicalTreatmentExpectedAcres;
            this.ChippingReportedAcres = chippingReportedAcres;
            this.ChippingExpectedAcres = chippingExpectedAcres;
            this.CulturalBurningReportedAcres = culturalBurningReportedAcres;
            this.CulturalBurningExpectedAcres = culturalBurningExpectedAcres;
            this.HandPilingReportedAcres = handPilingReportedAcres;
            this.HandPilingExpectedAcres = handPilingExpectedAcres;
            this.HandThinningReportedAcres = handThinningReportedAcres;
            this.HandThinningExpectedAcres = handThinningExpectedAcres;
            this.HelicopterYardingReportedAcres = helicopterYardingReportedAcres;
            this.HelicopterYardingExpectedAcres = helicopterYardingExpectedAcres;
            this.JackpotBurningReportedAcres = jackpotBurningReportedAcres;
            this.JackpotBurningExpectedAcres = jackpotBurningExpectedAcres;
            this.MachinePilingReportedAcres = machinePilingReportedAcres;
            this.MachinePilingExpectedAcres = machinePilingExpectedAcres;
            this.MasticationReportedAcres = masticationReportedAcres;
            this.MasticationExpectedAcres = masticationExpectedAcres;
            this.MechanicalThinningReportedAcres = mechanicalThinningReportedAcres;
            this.MechanicalThinningExpectedAcres = mechanicalThinningExpectedAcres;
            this.PileBurningReportedAcres = pileBurningReportedAcres;
            this.PileBurningExpectedAcres = pileBurningExpectedAcres;
            this.PrescribedBurningReportedAcres = prescribedBurningReportedAcres;
            this.PrescribedBurningExpectedAcres = prescribedBurningExpectedAcres;
            this.PrescribedTargetedGrazingReportedAcres = prescribedTargetedGrazingReportedAcres;
            this.PrescribedTargetedGrazingExpectedAcres = prescribedTargetedGrazingExpectedAcres;
            this.PruningReportedAcres = pruningReportedAcres;
            this.PruningExpectedAcres = pruningExpectedAcres;
            this.SalvageCutReportedAcres = salvageCutReportedAcres;
            this.SalvageCutExpectedAcres = salvageCutExpectedAcres;
            this.UnspecifiedTreatmentTypeReportedAcres = unspecifiedTreatmentTypeReportedAcres;
            this.UnspecifiedTreatmentTypeExpectedAcres = unspecifiedTreatmentTypeExpectedAcres;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public vGeoServerTcsiProjectSimpleLocations(vGeoServerTcsiProjectSimpleLocations vGeoServerTcsiProjectSimpleLocations) : this()
        {
            this.ProjectID = vGeoServerTcsiProjectSimpleLocations.ProjectID;
            this.PrimaryKey = vGeoServerTcsiProjectSimpleLocations.PrimaryKey;
            this.ProjectName = vGeoServerTcsiProjectSimpleLocations.ProjectName;
            this.TenantID = vGeoServerTcsiProjectSimpleLocations.TenantID;
            this.TenantName = vGeoServerTcsiProjectSimpleLocations.TenantName;
            this.ProjectDescription = vGeoServerTcsiProjectSimpleLocations.ProjectDescription;
            this.ProjectStage = vGeoServerTcsiProjectSimpleLocations.ProjectStage;
            this.TaxonomyLeaf = vGeoServerTcsiProjectSimpleLocations.TaxonomyLeaf;
            this.PrimaryContactOrganization = vGeoServerTcsiProjectSimpleLocations.PrimaryContactOrganization;
            this.PrimaryContactPerson = vGeoServerTcsiProjectSimpleLocations.PrimaryContactPerson;
            this.PlanningDesignStartYear = vGeoServerTcsiProjectSimpleLocations.PlanningDesignStartYear;
            this.ImplementationStartYear = vGeoServerTcsiProjectSimpleLocations.ImplementationStartYear;
            this.CompletionYear = vGeoServerTcsiProjectSimpleLocations.CompletionYear;
            this.SecuredFunding = vGeoServerTcsiProjectSimpleLocations.SecuredFunding;
            this.TargetedFunding = vGeoServerTcsiProjectSimpleLocations.TargetedFunding;
            this.EstimatedTotalCost = vGeoServerTcsiProjectSimpleLocations.EstimatedTotalCost;
            this.GisAcres = vGeoServerTcsiProjectSimpleLocations.GisAcres;
            this.Source = vGeoServerTcsiProjectSimpleLocations.Source;
            this.ProjectLastUpdated = vGeoServerTcsiProjectSimpleLocations.ProjectLastUpdated;
            this.BiomassRemovalReportedAcres = vGeoServerTcsiProjectSimpleLocations.BiomassRemovalReportedAcres;
            this.BiomassRemovalExpectedAcres = vGeoServerTcsiProjectSimpleLocations.BiomassRemovalExpectedAcres;
            this.BroadcastBurningReportedAcres = vGeoServerTcsiProjectSimpleLocations.BroadcastBurningReportedAcres;
            this.BroadcastBurningExpectedAcres = vGeoServerTcsiProjectSimpleLocations.BroadcastBurningExpectedAcres;
            this.ChemicalTreatmentReportedAcres = vGeoServerTcsiProjectSimpleLocations.ChemicalTreatmentReportedAcres;
            this.ChemicalTreatmentExpectedAcres = vGeoServerTcsiProjectSimpleLocations.ChemicalTreatmentExpectedAcres;
            this.ChippingReportedAcres = vGeoServerTcsiProjectSimpleLocations.ChippingReportedAcres;
            this.ChippingExpectedAcres = vGeoServerTcsiProjectSimpleLocations.ChippingExpectedAcres;
            this.CulturalBurningReportedAcres = vGeoServerTcsiProjectSimpleLocations.CulturalBurningReportedAcres;
            this.CulturalBurningExpectedAcres = vGeoServerTcsiProjectSimpleLocations.CulturalBurningExpectedAcres;
            this.HandPilingReportedAcres = vGeoServerTcsiProjectSimpleLocations.HandPilingReportedAcres;
            this.HandPilingExpectedAcres = vGeoServerTcsiProjectSimpleLocations.HandPilingExpectedAcres;
            this.HandThinningReportedAcres = vGeoServerTcsiProjectSimpleLocations.HandThinningReportedAcres;
            this.HandThinningExpectedAcres = vGeoServerTcsiProjectSimpleLocations.HandThinningExpectedAcres;
            this.HelicopterYardingReportedAcres = vGeoServerTcsiProjectSimpleLocations.HelicopterYardingReportedAcres;
            this.HelicopterYardingExpectedAcres = vGeoServerTcsiProjectSimpleLocations.HelicopterYardingExpectedAcres;
            this.JackpotBurningReportedAcres = vGeoServerTcsiProjectSimpleLocations.JackpotBurningReportedAcres;
            this.JackpotBurningExpectedAcres = vGeoServerTcsiProjectSimpleLocations.JackpotBurningExpectedAcres;
            this.MachinePilingReportedAcres = vGeoServerTcsiProjectSimpleLocations.MachinePilingReportedAcres;
            this.MachinePilingExpectedAcres = vGeoServerTcsiProjectSimpleLocations.MachinePilingExpectedAcres;
            this.MasticationReportedAcres = vGeoServerTcsiProjectSimpleLocations.MasticationReportedAcres;
            this.MasticationExpectedAcres = vGeoServerTcsiProjectSimpleLocations.MasticationExpectedAcres;
            this.MechanicalThinningReportedAcres = vGeoServerTcsiProjectSimpleLocations.MechanicalThinningReportedAcres;
            this.MechanicalThinningExpectedAcres = vGeoServerTcsiProjectSimpleLocations.MechanicalThinningExpectedAcres;
            this.PileBurningReportedAcres = vGeoServerTcsiProjectSimpleLocations.PileBurningReportedAcres;
            this.PileBurningExpectedAcres = vGeoServerTcsiProjectSimpleLocations.PileBurningExpectedAcres;
            this.PrescribedBurningReportedAcres = vGeoServerTcsiProjectSimpleLocations.PrescribedBurningReportedAcres;
            this.PrescribedBurningExpectedAcres = vGeoServerTcsiProjectSimpleLocations.PrescribedBurningExpectedAcres;
            this.PrescribedTargetedGrazingReportedAcres = vGeoServerTcsiProjectSimpleLocations.PrescribedTargetedGrazingReportedAcres;
            this.PrescribedTargetedGrazingExpectedAcres = vGeoServerTcsiProjectSimpleLocations.PrescribedTargetedGrazingExpectedAcres;
            this.PruningReportedAcres = vGeoServerTcsiProjectSimpleLocations.PruningReportedAcres;
            this.PruningExpectedAcres = vGeoServerTcsiProjectSimpleLocations.PruningExpectedAcres;
            this.SalvageCutReportedAcres = vGeoServerTcsiProjectSimpleLocations.SalvageCutReportedAcres;
            this.SalvageCutExpectedAcres = vGeoServerTcsiProjectSimpleLocations.SalvageCutExpectedAcres;
            this.UnspecifiedTreatmentTypeReportedAcres = vGeoServerTcsiProjectSimpleLocations.UnspecifiedTreatmentTypeReportedAcres;
            this.UnspecifiedTreatmentTypeExpectedAcres = vGeoServerTcsiProjectSimpleLocations.UnspecifiedTreatmentTypeExpectedAcres;
            CallAfterConstructor(vGeoServerTcsiProjectSimpleLocations);
        }

        partial void CallAfterConstructor(vGeoServerTcsiProjectSimpleLocations vGeoServerTcsiProjectSimpleLocations);

        public int ProjectID { get; set; }
        public int PrimaryKey { get; set; }
        public string ProjectName { get; set; }
        public int TenantID { get; set; }
        public string TenantName { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectStage { get; set; }
        public string TaxonomyLeaf { get; set; }
        public string PrimaryContactOrganization { get; set; }
        public string PrimaryContactPerson { get; set; }
        public int? PlanningDesignStartYear { get; set; }
        public int? ImplementationStartYear { get; set; }
        public int? CompletionYear { get; set; }
        public decimal? SecuredFunding { get; set; }
        public decimal? TargetedFunding { get; set; }
        public decimal? EstimatedTotalCost { get; set; }
        public decimal? GisAcres { get; set; }
        public string Source { get; set; }
        public DateTime ProjectLastUpdated { get; set; }
        public double? BiomassRemovalReportedAcres { get; set; }
        public double? BiomassRemovalExpectedAcres { get; set; }
        public double? BroadcastBurningReportedAcres { get; set; }
        public double? BroadcastBurningExpectedAcres { get; set; }
        public double? ChemicalTreatmentReportedAcres { get; set; }
        public double? ChemicalTreatmentExpectedAcres { get; set; }
        public double? ChippingReportedAcres { get; set; }
        public double? ChippingExpectedAcres { get; set; }
        public double? CulturalBurningReportedAcres { get; set; }
        public double? CulturalBurningExpectedAcres { get; set; }
        public double? HandPilingReportedAcres { get; set; }
        public double? HandPilingExpectedAcres { get; set; }
        public double? HandThinningReportedAcres { get; set; }
        public double? HandThinningExpectedAcres { get; set; }
        public double? HelicopterYardingReportedAcres { get; set; }
        public double? HelicopterYardingExpectedAcres { get; set; }
        public double? JackpotBurningReportedAcres { get; set; }
        public double? JackpotBurningExpectedAcres { get; set; }
        public double? MachinePilingReportedAcres { get; set; }
        public double? MachinePilingExpectedAcres { get; set; }
        public double? MasticationReportedAcres { get; set; }
        public double? MasticationExpectedAcres { get; set; }
        public double? MechanicalThinningReportedAcres { get; set; }
        public double? MechanicalThinningExpectedAcres { get; set; }
        public double? PileBurningReportedAcres { get; set; }
        public double? PileBurningExpectedAcres { get; set; }
        public double? PrescribedBurningReportedAcres { get; set; }
        public double? PrescribedBurningExpectedAcres { get; set; }
        public double? PrescribedTargetedGrazingReportedAcres { get; set; }
        public double? PrescribedTargetedGrazingExpectedAcres { get; set; }
        public double? PruningReportedAcres { get; set; }
        public double? PruningExpectedAcres { get; set; }
        public double? SalvageCutReportedAcres { get; set; }
        public double? SalvageCutExpectedAcres { get; set; }
        public double? UnspecifiedTreatmentTypeReportedAcres { get; set; }
        public double? UnspecifiedTreatmentTypeExpectedAcres { get; set; }
    }
}