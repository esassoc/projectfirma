//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vGeoServerTcsiForestFuelsTreatmentAcres]
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
    public partial class vGeoServerTcsiForestFuelsTreatmentAcres
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vGeoServerTcsiForestFuelsTreatmentAcres()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vGeoServerTcsiForestFuelsTreatmentAcres(int projectID, int primaryKey, double? biomassRemovalReportedAcres, double? biomassRemovalExpectedAcres, double? broadcastBurningReportedAcres, double? broadcastBurningExpectedAcres, double? chemicalTreatmentReportedAcres, double? chemicalTreatmentExpectedAcres, double? chippingReportedAcres, double? chippingExpectedAcres, double? culturalBurningReportedAcres, double? culturalBurningExpectedAcres, double? handPilingReportedAcres, double? handPilingExpectedAcres, double? handThinningReportedAcres, double? handThinningExpectedAcres, double? helicopterYardingReportedAcres, double? helicopterYardingExpectedAcres, double? jackpotBurningReportedAcres, double? jackpotBurningExpectedAcres, double? machinePilingReportedAcres, double? machinePilingExpectedAcres, double? masticationReportedAcres, double? masticationExpectedAcres, double? mechanicalThinningReportedAcres, double? mechanicalThinningExpectedAcres, double? pileBurningReportedAcres, double? pileBurningExpectedAcres, double? prescribedBurningReportedAcres, double? prescribedBurningExpectedAcres, double? prescribedTargetedGrazingReportedAcres, double? prescribedTargetedGrazingExpectedAcres, double? pruningReportedAcres, double? pruningExpectedAcres, double? salvageCutReportedAcres, double? salvageCutExpectedAcres, double? unspecifiedTreatmentTypeReportedAcres, double? unspecifiedTreatmentTypeExpectedAcres) : this()
        {
            this.ProjectID = projectID;
            this.PrimaryKey = primaryKey;
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
        public vGeoServerTcsiForestFuelsTreatmentAcres(vGeoServerTcsiForestFuelsTreatmentAcres vGeoServerTcsiForestFuelsTreatmentAcres) : this()
        {
            this.ProjectID = vGeoServerTcsiForestFuelsTreatmentAcres.ProjectID;
            this.PrimaryKey = vGeoServerTcsiForestFuelsTreatmentAcres.PrimaryKey;
            this.BiomassRemovalReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.BiomassRemovalReportedAcres;
            this.BiomassRemovalExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.BiomassRemovalExpectedAcres;
            this.BroadcastBurningReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.BroadcastBurningReportedAcres;
            this.BroadcastBurningExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.BroadcastBurningExpectedAcres;
            this.ChemicalTreatmentReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.ChemicalTreatmentReportedAcres;
            this.ChemicalTreatmentExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.ChemicalTreatmentExpectedAcres;
            this.ChippingReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.ChippingReportedAcres;
            this.ChippingExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.ChippingExpectedAcres;
            this.CulturalBurningReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.CulturalBurningReportedAcres;
            this.CulturalBurningExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.CulturalBurningExpectedAcres;
            this.HandPilingReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.HandPilingReportedAcres;
            this.HandPilingExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.HandPilingExpectedAcres;
            this.HandThinningReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.HandThinningReportedAcres;
            this.HandThinningExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.HandThinningExpectedAcres;
            this.HelicopterYardingReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.HelicopterYardingReportedAcres;
            this.HelicopterYardingExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.HelicopterYardingExpectedAcres;
            this.JackpotBurningReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.JackpotBurningReportedAcres;
            this.JackpotBurningExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.JackpotBurningExpectedAcres;
            this.MachinePilingReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.MachinePilingReportedAcres;
            this.MachinePilingExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.MachinePilingExpectedAcres;
            this.MasticationReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.MasticationReportedAcres;
            this.MasticationExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.MasticationExpectedAcres;
            this.MechanicalThinningReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.MechanicalThinningReportedAcres;
            this.MechanicalThinningExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.MechanicalThinningExpectedAcres;
            this.PileBurningReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.PileBurningReportedAcres;
            this.PileBurningExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.PileBurningExpectedAcres;
            this.PrescribedBurningReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.PrescribedBurningReportedAcres;
            this.PrescribedBurningExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.PrescribedBurningExpectedAcres;
            this.PrescribedTargetedGrazingReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.PrescribedTargetedGrazingReportedAcres;
            this.PrescribedTargetedGrazingExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.PrescribedTargetedGrazingExpectedAcres;
            this.PruningReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.PruningReportedAcres;
            this.PruningExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.PruningExpectedAcres;
            this.SalvageCutReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.SalvageCutReportedAcres;
            this.SalvageCutExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.SalvageCutExpectedAcres;
            this.UnspecifiedTreatmentTypeReportedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.UnspecifiedTreatmentTypeReportedAcres;
            this.UnspecifiedTreatmentTypeExpectedAcres = vGeoServerTcsiForestFuelsTreatmentAcres.UnspecifiedTreatmentTypeExpectedAcres;
            CallAfterConstructor(vGeoServerTcsiForestFuelsTreatmentAcres);
        }

        partial void CallAfterConstructor(vGeoServerTcsiForestFuelsTreatmentAcres vGeoServerTcsiForestFuelsTreatmentAcres);

        public int ProjectID { get; set; }
        public int PrimaryKey { get; set; }
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