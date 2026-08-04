//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vGeoServerTcsiProjectDetailedLocations]
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
    public partial class vGeoServerTcsiProjectDetailedLocations
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vGeoServerTcsiProjectDetailedLocations()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vGeoServerTcsiProjectDetailedLocations(int projectLocationID, int primaryKey, int projectID, string projectName, int tenantID, string tenantName, bool locationIsPrivate, int projectApprovalStatusID, string geometryType, int projectStageID, string projectStageColor, string projectDescription, string projectStage, string taxonomyLeaf, string primaryContactOrganization, string primaryContactPerson, int? planningDesignStartYear, int? implementationStartYear, int? completionYear, decimal? securedFunding, decimal? targetedFunding, decimal? estimatedTotalCost, DateTime projectLastUpdated, double? biomassRemovalReportedAcres, double? biomassRemovalExpectedAcres, double? broadcastBurningReportedAcres, double? broadcastBurningExpectedAcres, double? chemicalTreatmentReportedAcres, double? chemicalTreatmentExpectedAcres, double? chippingReportedAcres, double? chippingExpectedAcres, double? culturalBurningReportedAcres, double? culturalBurningExpectedAcres, double? handPilingReportedAcres, double? handPilingExpectedAcres, double? handThinningReportedAcres, double? handThinningExpectedAcres, double? helicopterYardingReportedAcres, double? helicopterYardingExpectedAcres, double? jackpotBurningReportedAcres, double? jackpotBurningExpectedAcres, double? machinePilingReportedAcres, double? machinePilingExpectedAcres, double? masticationReportedAcres, double? masticationExpectedAcres, double? mechanicalThinningReportedAcres, double? mechanicalThinningExpectedAcres, double? pileBurningReportedAcres, double? pileBurningExpectedAcres, double? prescribedBurningReportedAcres, double? prescribedBurningExpectedAcres, double? prescribedTargetedGrazingReportedAcres, double? prescribedTargetedGrazingExpectedAcres, double? pruningReportedAcres, double? pruningExpectedAcres, double? salvageCutReportedAcres, double? salvageCutExpectedAcres, double? unspecifiedTreatmentTypeReportedAcres, double? unspecifiedTreatmentTypeExpectedAcres) : this()
        {
            this.ProjectLocationID = projectLocationID;
            this.PrimaryKey = primaryKey;
            this.ProjectID = projectID;
            this.ProjectName = projectName;
            this.TenantID = tenantID;
            this.TenantName = tenantName;
            this.LocationIsPrivate = locationIsPrivate;
            this.ProjectApprovalStatusID = projectApprovalStatusID;
            this.GeometryType = geometryType;
            this.ProjectStageID = projectStageID;
            this.ProjectStageColor = projectStageColor;
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
        public vGeoServerTcsiProjectDetailedLocations(vGeoServerTcsiProjectDetailedLocations vGeoServerTcsiProjectDetailedLocations) : this()
        {
            this.ProjectLocationID = vGeoServerTcsiProjectDetailedLocations.ProjectLocationID;
            this.PrimaryKey = vGeoServerTcsiProjectDetailedLocations.PrimaryKey;
            this.ProjectID = vGeoServerTcsiProjectDetailedLocations.ProjectID;
            this.ProjectName = vGeoServerTcsiProjectDetailedLocations.ProjectName;
            this.TenantID = vGeoServerTcsiProjectDetailedLocations.TenantID;
            this.TenantName = vGeoServerTcsiProjectDetailedLocations.TenantName;
            this.LocationIsPrivate = vGeoServerTcsiProjectDetailedLocations.LocationIsPrivate;
            this.ProjectApprovalStatusID = vGeoServerTcsiProjectDetailedLocations.ProjectApprovalStatusID;
            this.GeometryType = vGeoServerTcsiProjectDetailedLocations.GeometryType;
            this.ProjectStageID = vGeoServerTcsiProjectDetailedLocations.ProjectStageID;
            this.ProjectStageColor = vGeoServerTcsiProjectDetailedLocations.ProjectStageColor;
            this.ProjectDescription = vGeoServerTcsiProjectDetailedLocations.ProjectDescription;
            this.ProjectStage = vGeoServerTcsiProjectDetailedLocations.ProjectStage;
            this.TaxonomyLeaf = vGeoServerTcsiProjectDetailedLocations.TaxonomyLeaf;
            this.PrimaryContactOrganization = vGeoServerTcsiProjectDetailedLocations.PrimaryContactOrganization;
            this.PrimaryContactPerson = vGeoServerTcsiProjectDetailedLocations.PrimaryContactPerson;
            this.PlanningDesignStartYear = vGeoServerTcsiProjectDetailedLocations.PlanningDesignStartYear;
            this.ImplementationStartYear = vGeoServerTcsiProjectDetailedLocations.ImplementationStartYear;
            this.CompletionYear = vGeoServerTcsiProjectDetailedLocations.CompletionYear;
            this.SecuredFunding = vGeoServerTcsiProjectDetailedLocations.SecuredFunding;
            this.TargetedFunding = vGeoServerTcsiProjectDetailedLocations.TargetedFunding;
            this.EstimatedTotalCost = vGeoServerTcsiProjectDetailedLocations.EstimatedTotalCost;
            this.ProjectLastUpdated = vGeoServerTcsiProjectDetailedLocations.ProjectLastUpdated;
            this.BiomassRemovalReportedAcres = vGeoServerTcsiProjectDetailedLocations.BiomassRemovalReportedAcres;
            this.BiomassRemovalExpectedAcres = vGeoServerTcsiProjectDetailedLocations.BiomassRemovalExpectedAcres;
            this.BroadcastBurningReportedAcres = vGeoServerTcsiProjectDetailedLocations.BroadcastBurningReportedAcres;
            this.BroadcastBurningExpectedAcres = vGeoServerTcsiProjectDetailedLocations.BroadcastBurningExpectedAcres;
            this.ChemicalTreatmentReportedAcres = vGeoServerTcsiProjectDetailedLocations.ChemicalTreatmentReportedAcres;
            this.ChemicalTreatmentExpectedAcres = vGeoServerTcsiProjectDetailedLocations.ChemicalTreatmentExpectedAcres;
            this.ChippingReportedAcres = vGeoServerTcsiProjectDetailedLocations.ChippingReportedAcres;
            this.ChippingExpectedAcres = vGeoServerTcsiProjectDetailedLocations.ChippingExpectedAcres;
            this.CulturalBurningReportedAcres = vGeoServerTcsiProjectDetailedLocations.CulturalBurningReportedAcres;
            this.CulturalBurningExpectedAcres = vGeoServerTcsiProjectDetailedLocations.CulturalBurningExpectedAcres;
            this.HandPilingReportedAcres = vGeoServerTcsiProjectDetailedLocations.HandPilingReportedAcres;
            this.HandPilingExpectedAcres = vGeoServerTcsiProjectDetailedLocations.HandPilingExpectedAcres;
            this.HandThinningReportedAcres = vGeoServerTcsiProjectDetailedLocations.HandThinningReportedAcres;
            this.HandThinningExpectedAcres = vGeoServerTcsiProjectDetailedLocations.HandThinningExpectedAcres;
            this.HelicopterYardingReportedAcres = vGeoServerTcsiProjectDetailedLocations.HelicopterYardingReportedAcres;
            this.HelicopterYardingExpectedAcres = vGeoServerTcsiProjectDetailedLocations.HelicopterYardingExpectedAcres;
            this.JackpotBurningReportedAcres = vGeoServerTcsiProjectDetailedLocations.JackpotBurningReportedAcres;
            this.JackpotBurningExpectedAcres = vGeoServerTcsiProjectDetailedLocations.JackpotBurningExpectedAcres;
            this.MachinePilingReportedAcres = vGeoServerTcsiProjectDetailedLocations.MachinePilingReportedAcres;
            this.MachinePilingExpectedAcres = vGeoServerTcsiProjectDetailedLocations.MachinePilingExpectedAcres;
            this.MasticationReportedAcres = vGeoServerTcsiProjectDetailedLocations.MasticationReportedAcres;
            this.MasticationExpectedAcres = vGeoServerTcsiProjectDetailedLocations.MasticationExpectedAcres;
            this.MechanicalThinningReportedAcres = vGeoServerTcsiProjectDetailedLocations.MechanicalThinningReportedAcres;
            this.MechanicalThinningExpectedAcres = vGeoServerTcsiProjectDetailedLocations.MechanicalThinningExpectedAcres;
            this.PileBurningReportedAcres = vGeoServerTcsiProjectDetailedLocations.PileBurningReportedAcres;
            this.PileBurningExpectedAcres = vGeoServerTcsiProjectDetailedLocations.PileBurningExpectedAcres;
            this.PrescribedBurningReportedAcres = vGeoServerTcsiProjectDetailedLocations.PrescribedBurningReportedAcres;
            this.PrescribedBurningExpectedAcres = vGeoServerTcsiProjectDetailedLocations.PrescribedBurningExpectedAcres;
            this.PrescribedTargetedGrazingReportedAcres = vGeoServerTcsiProjectDetailedLocations.PrescribedTargetedGrazingReportedAcres;
            this.PrescribedTargetedGrazingExpectedAcres = vGeoServerTcsiProjectDetailedLocations.PrescribedTargetedGrazingExpectedAcres;
            this.PruningReportedAcres = vGeoServerTcsiProjectDetailedLocations.PruningReportedAcres;
            this.PruningExpectedAcres = vGeoServerTcsiProjectDetailedLocations.PruningExpectedAcres;
            this.SalvageCutReportedAcres = vGeoServerTcsiProjectDetailedLocations.SalvageCutReportedAcres;
            this.SalvageCutExpectedAcres = vGeoServerTcsiProjectDetailedLocations.SalvageCutExpectedAcres;
            this.UnspecifiedTreatmentTypeReportedAcres = vGeoServerTcsiProjectDetailedLocations.UnspecifiedTreatmentTypeReportedAcres;
            this.UnspecifiedTreatmentTypeExpectedAcres = vGeoServerTcsiProjectDetailedLocations.UnspecifiedTreatmentTypeExpectedAcres;
            CallAfterConstructor(vGeoServerTcsiProjectDetailedLocations);
        }

        partial void CallAfterConstructor(vGeoServerTcsiProjectDetailedLocations vGeoServerTcsiProjectDetailedLocations);

        public int ProjectLocationID { get; set; }
        public int PrimaryKey { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int TenantID { get; set; }
        public string TenantName { get; set; }
        public bool LocationIsPrivate { get; set; }
        public int ProjectApprovalStatusID { get; set; }
        public string GeometryType { get; set; }
        public int ProjectStageID { get; set; }
        public string ProjectStageColor { get; set; }
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