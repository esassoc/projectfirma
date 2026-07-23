/*-----------------------------------------------------------------------
<copyright file="EditBasicsViewModel.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
Copyright (c) Tahoe Regional Planning Agency and Environmental Science Associates. All rights reserved.
<author>Environmental Science Associates</author>
</copyright>

<license>
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

Source code is available upon request via <support@sitkatech.com>.
</license>
-----------------------------------------------------------------------*/
using LtInfo.Common;
using LtInfo.Common.Models;
using LtInfo.Common.Mvc;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Models;
using ProjectFirma.Web.Security;
using ProjectFirmaModels;
using ProjectFirmaModels.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace ProjectFirma.Web.Views.Tenant
{
    public class EditBasicsViewModel : FormViewModel, IValidatableObject
    {
        [Required]
        public int? TenantID { get; set; }

        [Required]
        [FieldDefinitionDisplay(FieldDefinitionEnum.TenantShortDisplayName)]
        public string TenantShortDisplayName { get; set; }

        [Required]
        [FieldDefinitionDisplay(FieldDefinitionEnum.ToolDisplayName)]
        public string ToolDisplayName { get; set; }

        [DisplayName("Primary Contact")]
        public int? PrimaryContactPersonID { get; set; }

        [DisplayName("Minimum Year")]
        [Required(ErrorMessage = "Must specify a Minimum Year")]
        public int? MinimumYear { get; set; }

        [Required(ErrorMessage = "Must specify a Budget Type")]
        [DisplayName("Budget Type")]
        public int BudgetTypeID { get; set; }

        [DisplayName("Cost Types")]
        public List<string> CostTypes { get; set; }

        [DisplayName("Number Of Taxonomy Tiers To Use")]
        [Required(ErrorMessage = "Must specify a Number Of Taxonomy Tiers To Use")]
        public int? TaxonomyLevelID { get; set; }

        [DisplayName("Performance Measure Taxonomy Tier Association")]
        public int? AssociatePerfomanceMeasureTaxonomyLevelID { get; set; }

        [DisplayName("Tenant Style Sheet")]
        [SitkaFileExtensions("css")]
        public HttpPostedFileBase TenantStyleSheetFileResourceData { get; set; }

        [DisplayName("Tenant Banner Logo")]
        [SitkaFileExtensions("jpg|jpeg|gif|png")]
        public HttpPostedFileBase TenantBannerLogoFileResourceData { get; set; }

        [DisplayName("Square Logo")]
        [SitkaFileExtensions("jpg|jpeg|gif|png")]
        public HttpPostedFileBase TenantSquareLogoFileResourceData { get; set; }

        [DisplayName("Map Service Url")]
        [Url]
        public string MapServiceUrl { get; set; }

        [DisplayName("GeospatialArea Layer Name")]
        public string GeospatialAreaLayerName { get; set; }

        [DisplayName("External Data Source Enabled")]
        [Required]
        public bool? ProjectExternalDataSourceEnabled { get; set; }

        [DisplayName("External Data Source URL")]
        [StringLength(TenantAttribute.FieldLengths.ProjectExternalDataSourceApiUrl)]
        public string ProjectExternalDataSourceApiUrl { get; set; }

        [DisplayName("Source of Record Name")]
        [StringLength(TenantAttribute.FieldLengths.ProjectExternalSourceOfRecordName)]
        public string ProjectExternalSourceOfRecordName { get; set; }

        [DisplayName("Source of Record URL")]
        [StringLength(TenantAttribute.FieldLengths.ProjectExternalSourceOfRecordUrl)]
        public string ProjectExternalSourceOfRecordUrl { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.ShowProposalsToThePublic)]
        [Required]
        public bool? ShowProposalsToThePublic { get; set; }
        
        [DisplayName("Enable Accomplishments Dashboard")]
        public bool EnableAccomplishmentsDashboard { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.EnableSimpleAccomplishmentsDashboard)]
        public bool EnableSimpleAccomplishmentsDashboard { get; set; }

        [DisplayName("Accomplishments Dashboard Visible by Admin/Project Steward Only")]
        public bool AccomplishmentsDashboardVisibilityAdminOnly { get; set; }

        [DisplayName("Enable Secondary Project Taxonomy Leaf")]
        public bool EnableSecondaryProjectTaxonomyLeaf { get; set; }

        [DisplayName("Can Manage Custom Attributes")]
        public bool CanManageCustomAttributes { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.ExcludeTargetedFundingOrganizations)]
        public bool ExcludeTargetedFundingOrganizations { get; set; }

        [DisplayName("Google Analytics Tracking Code")]
        public string GoogleAnalyticsTrackingCode { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.UseProjectTimeline)]
        public bool UseProjectTimeline { get; set; }

        [DisplayName("Enable Status Updates")]
        public bool EnableStatusUpdates { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.EnableProjectEvaluations)]
        public bool EnableProjectEvaluations { get; set; }

        [DisplayName("GeoServer Namespace")]
        public string GeoServerNamespace { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.EnableProjectCategory)]
        public bool EnableProjectCategories { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.EnableReports)]
        public bool EnableReports { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.ReportVisibility)]
        public bool AreReportsPublic { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.EnableMatchmaker)]
        public bool EnableMatchmaker { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.EnableSolicitations)]
        public bool EnableSolicitations { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.TrackAccomplishments)]
        public bool TrackAccomplishments { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.SetTargetsByGeospatialArea)]
        public bool SetTargetsByGeospatialArea { get; set; }

        [DisplayName("Report Financials at project level")]
        public bool ReportFinancialsAtProjectLevel { get; set; }

        [DisplayName("Auto-Display Detailed Locations on Project Map")]
        public bool EnableDetailedLocationPolygonsOnProjectMap { get; set; }

        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public EditBasicsViewModel()
        {
        }

        public EditBasicsViewModel(ProjectFirmaModels.Models.Tenant tenant, TenantAttribute tenantAttribute)
        {
            TenantID = tenant.TenantID;
            TenantShortDisplayName = tenantAttribute.TenantShortDisplayName;
            ToolDisplayName = tenantAttribute.ToolDisplayName;
            PrimaryContactPersonID = tenantAttribute.PrimaryContactPersonID;
            TaxonomyLevelID = tenantAttribute.TaxonomyLevelID;
            AssociatePerfomanceMeasureTaxonomyLevelID = tenantAttribute.AssociatePerfomanceMeasureTaxonomyLevelID;
            MinimumYear = tenantAttribute.MinimumYear;
            BudgetTypeID = tenantAttribute.BudgetTypeID;
            ProjectExternalDataSourceEnabled = tenantAttribute.ProjectExternalDataSourceEnabled;
            ProjectExternalDataSourceApiUrl = tenantAttribute.ProjectExternalDataSourceApiUrl;
            ProjectExternalSourceOfRecordName = tenantAttribute.ProjectExternalSourceOfRecordName;
            ProjectExternalSourceOfRecordUrl = tenantAttribute.ProjectExternalSourceOfRecordUrl;
            ShowProposalsToThePublic = tenantAttribute.ShowProposalsToThePublic;
            EnableAccomplishmentsDashboard = tenantAttribute.EnableAccomplishmentsDashboard;
            EnableSimpleAccomplishmentsDashboard = tenantAttribute.EnableSimpleAccomplishmentsDashboard;
            AccomplishmentsDashboardVisibilityAdminOnly = tenantAttribute.AccomplishmentsDashboardVisibilityAdminOnly;
            EnableSecondaryProjectTaxonomyLeaf = tenantAttribute.EnableSecondaryProjectTaxonomyLeaf;
            CanManageCustomAttributes = tenantAttribute.CanManageCustomAttributes;
            ExcludeTargetedFundingOrganizations = tenantAttribute.ExcludeTargetedFundingOrganizations;
            GoogleAnalyticsTrackingCode = tenantAttribute.GoogleAnalyticsTrackingCode;
            UseProjectTimeline = tenantAttribute.UseProjectTimeline;
            EnableStatusUpdates = tenantAttribute.EnableStatusUpdates;
            EnableProjectEvaluations = tenantAttribute.EnableEvaluations;
            GeoServerNamespace = tenantAttribute.GeoServerNamespace;
            EnableProjectCategories = tenantAttribute.EnableProjectCategories;
            EnableReports = tenantAttribute.EnableReports;
            AreReportsPublic = tenantAttribute.AreReportsPublic;
            EnableMatchmaker = tenantAttribute.EnableMatchmaker;
            TrackAccomplishments = tenantAttribute.TrackAccomplishments;
            EnableSolicitations = tenantAttribute.EnableSolicitations;
            SetTargetsByGeospatialArea = tenantAttribute.SetTargetsByGeospatialArea;
            ReportFinancialsAtProjectLevel = tenantAttribute.ReportFinancialsAtProjectLevel;
            EnableDetailedLocationPolygonsOnProjectMap = tenantAttribute.EnableDetailedLocationPolygonsOnProjectMap ?? false;
        }

        public void UpdateModel(TenantAttribute tenantAttribute, FirmaSession currentFirmaSession)
        {
            tenantAttribute.TenantShortDisplayName = TenantShortDisplayName;
            tenantAttribute.ToolDisplayName = ToolDisplayName;
            tenantAttribute.ShowProposalsToThePublic = ShowProposalsToThePublic.GetValueOrDefault();
            tenantAttribute.EnableAccomplishmentsDashboard = EnableAccomplishmentsDashboard;
            tenantAttribute.EnableSimpleAccomplishmentsDashboard = EnableSimpleAccomplishmentsDashboard;
            tenantAttribute.AccomplishmentsDashboardVisibilityAdminOnly = AccomplishmentsDashboardVisibilityAdminOnly;
            tenantAttribute.EnableSecondaryProjectTaxonomyLeaf = EnableSecondaryProjectTaxonomyLeaf;
            tenantAttribute.CanManageCustomAttributes = CanManageCustomAttributes;
            tenantAttribute.ExcludeTargetedFundingOrganizations = ExcludeTargetedFundingOrganizations;
            tenantAttribute.GoogleAnalyticsTrackingCode = GoogleAnalyticsTrackingCode;
            tenantAttribute.UseProjectTimeline = UseProjectTimeline;
            tenantAttribute.EnableStatusUpdates = EnableStatusUpdates;
            tenantAttribute.GeoServerNamespace = GeoServerNamespace;
            tenantAttribute.EnableProjectCategories = EnableProjectCategories;

            Person primaryContactPerson = null;
            if (PrimaryContactPersonID != null)
            {
                primaryContactPerson = HttpRequestStorage.DatabaseEntities.People.GetPerson(PrimaryContactPersonID.Value);
            }
            tenantAttribute.PrimaryContactPerson = primaryContactPerson;
            tenantAttribute.TaxonomyLevelID = TaxonomyLevelID ?? ModelObjectHelpers.NotYetAssignedID;
            if (TrackAccomplishments)
            {
                // only change this ID if Accomplishment Tracking is turned on. It is a required field in the DB. Also changing it results in deleting any Taxonomy-PM relationships
                tenantAttribute.AssociatePerfomanceMeasureTaxonomyLevelID = AssociatePerfomanceMeasureTaxonomyLevelID ?? ModelObjectHelpers.NotYetAssignedID;

            }
            tenantAttribute.MinimumYear = MinimumYear ?? 0;
            tenantAttribute.BudgetTypeID = BudgetTypeID;

            tenantAttribute.ProjectExternalDataSourceEnabled = ProjectExternalDataSourceEnabled ?? false;
            if (!tenantAttribute.ProjectExternalDataSourceEnabled)
            {
                tenantAttribute.ProjectExternalDataSourceApiUrl = null;
                tenantAttribute.ProjectExternalSourceOfRecordName = null;
                tenantAttribute.ProjectExternalSourceOfRecordUrl = null;
            }
            else
            {
                tenantAttribute.ProjectExternalDataSourceApiUrl = ProjectExternalDataSourceApiUrl;
                tenantAttribute.ProjectExternalSourceOfRecordName = ProjectExternalSourceOfRecordName;
                tenantAttribute.ProjectExternalSourceOfRecordUrl = ProjectExternalSourceOfRecordUrl;
            }
            
            tenantAttribute.EnableEvaluations = EnableProjectEvaluations;
            tenantAttribute.EnableReports = EnableReports;
            tenantAttribute.AreReportsPublic = AreReportsPublic;
            tenantAttribute.EnableMatchmaker = EnableMatchmaker;
            tenantAttribute.TrackAccomplishments = TrackAccomplishments;
            tenantAttribute.EnableSolicitations = EnableSolicitations;
            tenantAttribute.SetTargetsByGeospatialArea = SetTargetsByGeospatialArea;
            tenantAttribute.ReportFinancialsAtProjectLevel = ReportFinancialsAtProjectLevel;
            tenantAttribute.EnableDetailedLocationPolygonsOnProjectMap = EnableDetailedLocationPolygonsOnProjectMap;
        }

        public void UpdateCostTypes(List<CostType> existingCostTypes, IList<CostType> allCostTypes)
        {
            var incomingCostTypes = new List<CostType>();
            CostTypes.ForEach(x => incomingCostTypes.Add(new CostType(x)));
            var databaseEntities = HttpRequestStorage.DatabaseEntities;
            existingCostTypes.Merge(incomingCostTypes,
                allCostTypes,
                (x, y) => x.CostTypeName == y.CostTypeName,
                databaseEntities);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (PrimaryContactPersonID != null)
            {
                var primaryContact = HttpRequestStorage.DatabaseEntities.People.GetPerson(PrimaryContactPersonID.Value);
                if (!new FirmaAdminFeature().HasPermissionByPerson(primaryContact))
                {
                    errors.Add(new SitkaValidationResult<EditBasicsViewModel, int?>($"{FieldDefinitionEnum.OrganizationPrimaryContact.ToType().GetFieldDefinitionLabel()} must be an admin.", m => m.PrimaryContactPersonID));
                }
            }

            if (TrackAccomplishments && TaxonomyLevelID.Value < AssociatePerfomanceMeasureTaxonomyLevelID.Value)
            {
                errors.Add(new SitkaValidationResult<EditBasicsViewModel, int?>("Cannot choose a Taxonomy Tier that does not exist!", m => m.AssociatePerfomanceMeasureTaxonomyLevelID));
            }

            // At least one Cost Type is required is selecting a Budget Type that uses Cost Types
            if (BudgetTypeID == BudgetType.AnnualBudgetByCostType.BudgetTypeID && CostTypes == null)
            {
                errors.Add(new SitkaValidationResult<EditBasicsViewModel, int>($"One or more Cost Types must exist when selecting '{BudgetType.AnnualBudgetByCostType.BudgetTypeDisplayName}'", m => m.BudgetTypeID));
            }

            // Ensure that the Google Analytics code is a valid format since this is being displayed raw in the header of the website
            if (!GoogleAnalyticsTrackingCode.IsNullOrWhiteSpace())
            {
                Regex regexPattern = new Regex(@"^(?i)g(?i)-[a-z0-9]{4,10}$");
                if (!regexPattern.IsMatch(GoogleAnalyticsTrackingCode))
                {
                    errors.Add(new SitkaValidationResult<EditBasicsViewModel, string>($"The Google Analytics tracking code provided is invalid.", m => m.GoogleAnalyticsTrackingCode));
                }
            }

            if (!UseProjectTimeline && EnableStatusUpdates)
            {
                errors.Add(new SitkaValidationResult<EditBasicsViewModel, bool>($"Cannot Enable Status Updates without {FieldDefinitionEnum.UseProjectTimeline.ToType().GetFieldDefinitionLabel()} also selected.", m => m.EnableStatusUpdates));
            }

            if(ProjectExternalDataSourceEnabled.HasValue && ProjectExternalDataSourceEnabled.Value)
            {
                if (string.IsNullOrWhiteSpace(ProjectExternalSourceOfRecordName))
                {
                    errors.Add(new SitkaValidationResult<EditBasicsViewModel, string>($"Source of Record Name is required when External Data Source is enabled.", m => m.ProjectExternalSourceOfRecordName));
                }
                if (string.IsNullOrWhiteSpace(ProjectExternalSourceOfRecordUrl))
                {
                    errors.Add(new SitkaValidationResult<EditBasicsViewModel, string>($"Source of Record URL is required when External Data Source is enabled.", m => m.ProjectExternalSourceOfRecordUrl));
                }
            }

            return errors;
        }
    }
}
