/*-----------------------------------------------------------------------
<copyright file="ProjectCustomGridSpec.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LtInfo.Common;
using LtInfo.Common.AgGridWrappers;
using LtInfo.Common.Views;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Controllers;
using ProjectFirma.Web.Models;
using ProjectFirma.Web.Security;
using ProjectFirmaModels.Models;

namespace ProjectFirma.Web.Views.ProjectCustomGrid
{
    public class ProjectCustomGridSpec : GridSpec<ProjectFirmaModels.Models.Project>
    {

        public static HtmlString MakeProjectStatusAddLinkAndText(ProjectFirmaModels.Models.Project project
            , FirmaSession currentFirmaSession
            , vProjectDetail projectDetail
            , string projectLabel
            , bool hasProjectApprovalPermissionBySession
            , string statusUpdateLabel)
        {
            
            var editIconAsModalDialogLinkBootstrap = new HtmlString(string.Empty);
            var isEditableToThisFirmaSession = project.IsEditableToThisFirmaSession(currentFirmaSession, projectDetail, projectLabel, hasProjectApprovalPermissionBySession);

            var returnString = new HtmlString("");
            if (!isEditableToThisFirmaSession) return returnString;

            editIconAsModalDialogLinkBootstrap = AgGridHtmlHelpers.MakePlusIconAsModalDialogLinkBootstrap(
                project.GetAddProjectProjectStatusFromGridUrl()
                , $"Add {statusUpdateLabel}");

            var currentProjectStatus = project.GetCurrentProjectStatus();
            var colorString = currentProjectStatus != null ? currentProjectStatus.ProjectStatusColor : "transparent";
            var projectStatusDisplayName = currentProjectStatus != null ? currentProjectStatus.ProjectStatusDisplayName : "no status";
            returnString = new HtmlString($"<div style=\"border-left:10px solid {colorString}; padding-left:5px;\">{editIconAsModalDialogLinkBootstrap} {projectStatusDisplayName}</div>");

            return returnString;
        }

        
        private void AddProjectCustomGridField(FirmaSession currentFirmaSession
            , ProjectCustomGridConfiguration projectCustomGridConfiguration
            ,bool userHasEditProjectAsAdminPermissions
            , Dictionary<int, vProjectDetail> projectDetailsDictionary
            , Dictionary<int,ProjectFirmaModels.Models.TaxonomyLeaf> taxonomyLeafDictionary
            , string projectLabel
            , bool hasProjectApprovalPermissionBySession
            , string statusUpdateLabel
            , List<int> sitkaAdminPersonIDs)
        {

            switch (projectCustomGridConfiguration.ProjectCustomGridColumn.ToEnum)
            {
                // Non-optional fields
                // Project Name
                case ProjectCustomGridColumnEnum.ProjectName:
                    Add(FieldDefinitionEnum.ProjectName.ToType().ToGridHeaderString(), x => new HtmlLinkObject(x.ProjectName, x.GetDetailUrl()).ToJsonObjectForAgGrid() , 300, AgGridColumnFilterType.HtmlLinkJson);
                    break;
                case ProjectCustomGridColumnEnum.PrimaryContactOrganization:
                    Add(FieldDefinitionEnum.IsPrimaryContactOrganization.ToType().ToGridHeaderString(),
                        x => new HtmlLinkObject(projectDetailsDictionary[x.ProjectID].PrimaryContactOrganizationDisplayName, projectDetailsDictionary[x.ProjectID].PrimaryContactOrganizationID.HasValue ? OrganizationModelExtensions.SummaryUrlTemplate.ParameterReplace(projectDetailsDictionary[x.ProjectID].PrimaryContactOrganizationID.Value) : string.Empty).ToJsonObjectForAgGrid(), 150, AgGridColumnFilterType.HtmlLinkJson);
                    break;
                case ProjectCustomGridColumnEnum.ProjectStage:
                    Add(FieldDefinitionEnum.ProjectStage.ToType().ToGridHeaderString(), x => x.ProjectStage.GetProjectStageDisplayName(), 90, AgGridColumnFilterType.SelectFilterStrict);
                    break;
                case ProjectCustomGridColumnEnum.NumberOfExpectedPerformanceMeasureRecords:
                    Add($"# Of Expected {MultiTenantHelpers.GetPerformanceMeasureName()} Records", x => projectDetailsDictionary[x.ProjectID].PerformanceMeasureExpectedCount, 160, AgGridColumnFormatType.Integer);
                    break;
                case ProjectCustomGridColumnEnum.NumberOfReportedPerformanceMeasures:
                    Add($"# Of Reported {MultiTenantHelpers.GetPerformanceMeasureName()} Records", x => projectDetailsDictionary[x.ProjectID].PerformanceMeasureActualCount, 160, AgGridColumnFormatType.Integer);
                    break;
                case ProjectCustomGridColumnEnum.ProjectsStewardOrganizationRelationshipToProject:
                    if (MultiTenantHelpers.HasCanStewardProjectsOrganizationRelationship())
                    {
                        Add(FieldDefinitionEnum.ProjectsStewardOrganizationRelationshipToProject.ToType().ToGridHeaderString(), x => new HtmlLinkObject(projectDetailsDictionary[x.ProjectID].CanStewardProjectsOrganizationDisplayName, projectDetailsDictionary[x.ProjectID].CanStewardProjectsOrganizationID.HasValue ? OrganizationModelExtensions.SummaryUrlTemplate.ParameterReplace(projectDetailsDictionary[x.ProjectID].CanStewardProjectsOrganizationID.Value) : string.Empty).ToJsonObjectForAgGrid(), 150, AgGridColumnFilterType.HtmlLinkJson);
                    }
                    break;
                case ProjectCustomGridColumnEnum.ProjectPrimaryContact:
                    Add(FieldDefinitionEnum.ProjectPrimaryContact.ToType().ToGridHeaderString(),
                        x => projectDetailsDictionary[x.ProjectID].PrimaryContactPersonID.HasValue ?
                            new UserViewFeature().HasPermissionForPersonID(currentFirmaSession, projectDetailsDictionary[x.ProjectID].PrimaryContactPersonID.Value, sitkaAdminPersonIDs).HasPermission ? new HtmlLinkObject(projectDetailsDictionary[x.ProjectID].PrimaryContactPersonFullNameFirstLast,PersonModelExtensions.DetailUrlTemplate.ParameterReplace(projectDetailsDictionary[x.ProjectID].PrimaryContactPersonID.Value)).ToJsonObjectForAgGrid() : new HtmlLinkObject(projectDetailsDictionary[x.ProjectID].PrimaryContactPersonFullNameFirstLast, string.Empty).ToJsonObjectForAgGrid() 
                            : !string.IsNullOrEmpty(projectDetailsDictionary[x.ProjectID].ExternalProjectPrimaryContactPersonFullName) ? new HtmlLinkObject(projectDetailsDictionary[x.ProjectID].ExternalProjectPrimaryContactPersonFullName, string.Empty).ToJsonObjectForAgGrid(): new HtmlLinkObject(string.Empty, string.Empty).ToJsonObjectForAgGrid(),
                        150, AgGridColumnFilterType.HtmlLinkJson);
                    break;
                case ProjectCustomGridColumnEnum.ProjectPrimaryContactEmail:
                    var userHasEmailViewingPermissions = new LoggedInAndNotUnassignedRoleUnclassifiedFeature().HasPermissionByFirmaSession(currentFirmaSession);
                    if (userHasEmailViewingPermissions)
                    {
                        Add(FieldDefinitionEnum.ProjectPrimaryContactEmail.ToType().ToGridHeaderString(),
                            x => projectDetailsDictionary[x.ProjectID].PrimaryContactPersonID.HasValue ? new HtmlLinkObject(projectDetailsDictionary[x.ProjectID].PrimaryContactPersonEmail, $"mailto:{projectDetailsDictionary[x.ProjectID].PrimaryContactPersonEmail}").ToJsonObjectForAgGrid() : new HtmlLinkObject(string.Empty, string.Empty).ToJsonObjectForAgGrid(),
                            200, AgGridColumnFilterType.HtmlLinkJson);
                    }
                    break;
                case ProjectCustomGridColumnEnum.PlanningDesignStartYear:
                    Add(FieldDefinitionEnum.PlanningDesignStartYear.ToType().ToGridHeaderString(), x => ProjectModelExtensions.GetPlanningDesignStartYear(x), 90, AgGridColumnFilterType.SelectFilterStrict);
                    break;
                case ProjectCustomGridColumnEnum.ImplementationStartYear:
                    Add(FieldDefinitionEnum.ImplementationStartYear.ToType().ToGridHeaderString(), x => ProjectModelExtensions.GetImplementationStartYear(x), 120, AgGridColumnFilterType.SelectFilterStrict);
                    break;
                case ProjectCustomGridColumnEnum.CompletionYear:
                    Add(FieldDefinitionEnum.CompletionYear.ToType().ToGridHeaderString(), x => ProjectModelExtensions.GetCompletionYear(x), 95, AgGridColumnFilterType.SelectFilterStrict);
                    break;
                case ProjectCustomGridColumnEnum.PrimaryTaxonomyLeaf:
                    var gridHeaderString = MultiTenantHelpers.GetTenantAttributeFromCache().EnableSecondaryProjectTaxonomyLeaf
                        ? FieldDefinitionEnum.TaxonomyLeafDisplayNameForProject.ToType().ToGridHeaderString()
                        : FieldDefinitionEnum.TaxonomyLeaf.ToType().ToGridHeaderString();
                    Add(gridHeaderString, x => new HtmlLinkObject(projectDetailsDictionary[x.ProjectID].TaxonomyLeafDisplayName,TaxonomyLeafModelExtensions.DetailUrlTemplate.ParameterReplace(projectDetailsDictionary[x.ProjectID].TaxonomyLeafID)).ToJsonObjectForAgGrid(), 240, AgGridColumnFilterType.HtmlLinkJson);
                    break;
                case ProjectCustomGridColumnEnum.SecondaryTaxonomyLeaf:
                    if (MultiTenantHelpers.GetTenantAttributeFromCache().EnableSecondaryProjectTaxonomyLeaf)
                    {
                        Add(FieldDefinitionEnum.SecondaryProjectTaxonomyLeaf.ToType().ToGridHeaderStringPlural()
                            , x => x.SecondaryProjectTaxonomyLeafs.Select(y => new HtmlLinkObject(taxonomyLeafDictionary[y.TaxonomyLeafID].GetDisplayName(), taxonomyLeafDictionary[y.TaxonomyLeafID].GetDetailUrl()) ).ToJsonArrayForAgGrid(), 300, AgGridColumnFilterType.HtmlLinkListJson);
                    }
                    break;
                case ProjectCustomGridColumnEnum.NumberOfReportedExpenditures:
                    if (MultiTenantHelpers.ReportFinancialsAtProjectLevel())
                    {
                        Add($"# Of {FieldDefinitionEnum.ReportedExpenditure.ToType().GetFieldDefinitionLabel()} Records", x => projectDetailsDictionary[x.ProjectID].ProjectFundingSourceExpenditureCount, 100, AgGridColumnFormatType.Integer);
                    }
                    break;
                case ProjectCustomGridColumnEnum.FundingType:
                    if (MultiTenantHelpers.ReportFinancialsAtProjectLevel())
                    {
                        Add(FieldDefinitionEnum.FundingType.ToType().ToGridHeaderString(), x => x.FundingType != null ? x.FundingType.FundingTypeDisplayName : "", 300, AgGridColumnFilterType.SelectFilterStrict);
                    }
                    break;
                case ProjectCustomGridColumnEnum.EstimatedTotalCost:
                    if (MultiTenantHelpers.ReportFinancialsAtProjectLevel())
                    {
                        Add(FieldDefinitionEnum.EstimatedTotalCost.ToType().ToGridHeaderString(), x => x.GetEstimatedTotalRegardlessOfFundingType(), 110, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
                    }
                    break;
                case ProjectCustomGridColumnEnum.SecuredFunding:
                    if (MultiTenantHelpers.ReportFinancialsAtProjectLevel())
                    {
                        Add(FieldDefinitionEnum.SecuredFunding.ToType().ToGridHeaderString(), x => x.GetSecuredFunding(), 110, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
                    }
                    break;
                case ProjectCustomGridColumnEnum.TargetedFunding:
                    if (MultiTenantHelpers.ReportFinancialsAtProjectLevel())
                    {
                        Add(FieldDefinitionEnum.TargetedFunding.ToType().ToGridHeaderString(), x => x.GetTargetedFunding(), 100, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
                    }
                    break;
                case ProjectCustomGridColumnEnum.NoFundingSourceIdentified:
                    if (MultiTenantHelpers.ReportFinancialsAtProjectLevel())
                    {
                        Add(FieldDefinitionEnum.NoFundingSourceIdentified.ToType().ToGridHeaderString(), x => x.GetNoFundingSourceIdentifiedAmount(), 120, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
                    }
                    break;
                case ProjectCustomGridColumnEnum.ProjectDescription:
                    Add(FieldDefinitionEnum.ProjectDescription.ToType().ToGridHeaderString(), x => x.ProjectDescription, 200);
                    break;
                case ProjectCustomGridColumnEnum.NumberOfPhotos:
                    Add("# of Photos", x => projectDetailsDictionary[x.ProjectID].ProjectImageCount, 60, AgGridColumnFormatType.Integer);
                    break;
                case ProjectCustomGridColumnEnum.ProjectID:
                    Add(FieldDefinitionEnum.ProjectID.ToType().ToGridHeaderString(), x => x.ProjectID.ToString(), 140);
                    break;
                case ProjectCustomGridColumnEnum.ProjectLastUpdated:
                    Add(FieldDefinitionEnum.ProjectLastUpdated.ToType().ToGridHeaderString(), x => x.LastUpdatedDate, 140, AgGridColumnFormatType.DateTime);
                    break;
                case ProjectCustomGridColumnEnum.ProjectStatus:
                    if (MultiTenantHelpers.GetTenantAttributeFromCache().EnableStatusUpdates && userHasEditProjectAsAdminPermissions)
                    {
                        Add(FieldDefinitionEnum.Status.ToType().ToGridHeaderString()
                            , x => MakeProjectStatusAddLinkAndText(x, currentFirmaSession, projectDetailsDictionary[x.ProjectID], projectLabel, hasProjectApprovalPermissionBySession, statusUpdateLabel)
                            , 100
                            , AgGridColumnFilterType.SelectFilterHtmlStrict
                        );
                    }
                    break;
                case ProjectCustomGridColumnEnum.FinalStatusUpdateStatus:
                    if (MultiTenantHelpers.GetTenantAttributeFromCache().EnableStatusUpdates && userHasEditProjectAsAdminPermissions)
                    {
                        Add(FieldDefinitionEnum.FinalStatusUpdateStatus.ToType().ToGridHeaderString()
                            , x => projectDetailsDictionary[x.ProjectID].FinalStatusReportStatusDescription
                            , 100
                            , AgGridColumnFilterType.SelectFilterStrict
                        );
                    }
                    break;
                case ProjectCustomGridColumnEnum.GeospatialAreaName:
                    break;
                case ProjectCustomGridColumnEnum.CustomAttribute:
                    break;
                case ProjectCustomGridColumnEnum.ClassificationSystem:
                    break;
                case ProjectCustomGridColumnEnum.ProjectCategory:
                    if (MultiTenantHelpers.GetTenantAttributeFromCache().EnableProjectCategories)
                    {
                        Add(FieldDefinitionEnum.ProjectCategory.ToType().ToGridHeaderString(), x => x.ProjectCategory.ProjectCategoryDisplayName, 140, AgGridColumnFilterType.SelectFilterStrict);
                    }
                    break;
                case ProjectCustomGridColumnEnum.Solicitation:
                    if (MultiTenantHelpers.GetTenantAttributeFromCache().EnableSolicitations)
                    {
                        Add(FieldDefinitionEnum.Solicitation.ToType().ToGridHeaderString(), x => x.Solicitation != null ? x.Solicitation.SolicitationName : string.Empty, 140, AgGridColumnFilterType.SelectFilterStrict);
                    }
                    break;
                case ProjectCustomGridColumnEnum.FundingSources:
                    if (MultiTenantHelpers.ReportFinancialsAtProjectLevel())
                    {
                        Add(FieldDefinitionEnum.FundingSource.ToType().ToGridHeaderStringPlural(), x => x.GetFundingSourcesAsLinksForAgGrid(false), 300, AgGridColumnFilterType.HtmlLinkListJson);
                    }
                    break;
                case ProjectCustomGridColumnEnum.Organizations:
                    Add(FieldDefinitionEnum.Organization.ToType().ToGridHeaderStringPlural(), x => x.GetAssociatedOrganizationsAsLinksForAgGrid(), 300, AgGridColumnFilterType.HtmlLinkListJson);
                    break;
                case ProjectCustomGridColumnEnum.SourceOfRecord:
                    Add("Source of Record", x => x.ExternalID == null ? MultiTenantHelpers.GetTenantShortDisplayName() : MultiTenantHelpers.GetTenantAttributeFromCache().ProjectExternalSourceOfRecordName, 140, AgGridColumnFilterType.SelectFilterStrict);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddProjectCustomGridCustomAttributeField(ProjectCustomGridConfiguration projectCustomGridConfiguration, Dictionary<int, List<vProjectCustomAttributeValue>> projectCustomAttributeDictionary)
        {
            var projectCustomAttributeType = projectCustomGridConfiguration.ProjectCustomAttributeType;
            var isCurrency = projectCustomGridConfiguration.ProjectCustomAttributeType.MeasurementUnitTypeID == MeasurementUnitType.Dollars.MeasurementUnitTypeID;
            var gridHeaderHtmlString = ("<div>" 
                                    + LabelWithSugarForExtensions.GenerateHelpIconImgTag(projectCustomAttributeType.ProjectCustomAttributeTypeName, projectCustomAttributeType.ProjectCustomAttributeTypeDescription.ToHTMLFormattedString(), projectCustomAttributeType.GetDescriptionUrl(), 300, LabelWithSugarForExtensions.DisplayStyle.HelpIconOnly) 
                                    + projectCustomAttributeType.ProjectCustomAttributeTypeName 
                                    + "</div>").ToHTMLFormattedString();
            if (isCurrency)
            {
                Add($"{gridHeaderHtmlString}", a => TryParseDecimalCustomAttributeValue(a, projectCustomAttributeType, projectCustomAttributeDictionary), 150, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
            }
            else
            {
                Add($"{gridHeaderHtmlString}", a => a.GetProjectCustomAttributesValue(projectCustomAttributeType, projectCustomAttributeDictionary), 160, AgGridColumnFilterType.Text);
            }
        }

        private static decimal? TryParseDecimalCustomAttributeValue(ProjectFirmaModels.Models.Project project, ProjectFirmaModels.Models.ProjectCustomAttributeType projectCustomAttributeType, Dictionary<int, List<vProjectCustomAttributeValue>> projectCustomAttributeDictionary)
        {
            if (Decimal.TryParse(project.GetProjectCustomAttributesValue(projectCustomAttributeType, projectCustomAttributeDictionary).ToString(), out var value))
            {
                return value;
            }

            if (projectCustomAttributeType.ProjectCustomAttributeGroup.ProjectCustomAttributeGroupProjectCategories.All(x => x.ProjectCategoryID != project.ProjectCategoryID))
            {
                return null;
            }
            return 0;
        }

        private void AddProjectCustomGridGeospatialAreaField(ProjectCustomGridConfiguration projectCustomGridConfiguration, Dictionary<int, vGeospatialArea> geospatialAreas, Dictionary<int, List<ProjectGeospatialArea>> projectGeospatialAreaDictionary)
        {
            var geospatialAreaType = projectCustomGridConfiguration.GeospatialAreaType;
            var gridWidth = 350;
            if (geospatialAreaType.GeospatialAreaTypeID == 24) // NCRP Counties
                gridWidth = 80;
            if (geospatialAreaType.GeospatialAreaTypeID == 25) // NCRP Watershed - HUC 8
                gridWidth = 205;
            Add($"{geospatialAreaType.GeospatialAreaTypeNamePluralized}", a => a.GetProjectGeospatialAreaNamesAsHyperlinksForAgGrid(geospatialAreaType, geospatialAreas, projectGeospatialAreaDictionary), gridWidth, AgGridColumnFilterType.HtmlLinkListJson);
        }

        private void AddProjectCustomGridClassificationSystemField(ProjectCustomGridConfiguration projectCustomGridConfiguration, Dictionary<int, ProjectFirmaModels.Models.Classification> classificationsDictionary, Dictionary<int, List<ProjectClassification>> projectClassificationsDictionary)
        {
            var classificationSystem = projectCustomGridConfiguration.ClassificationSystem;
            Add($"{classificationSystem.ClassificationSystemNamePluralized}", a => a.GetProjectClassificationsAsHyperlinksForAgGrid(classificationSystem, classificationsDictionary, projectClassificationsDictionary), 350, AgGridColumnFilterType.HtmlLinkListJson);
        }

        public ProjectCustomGridSpec(FirmaSession currentFirmaSession,
            List<ProjectCustomGridConfiguration> projectCustomGridConfigurations,
            ProjectCustomGridTypeEnum projectCustomGridTypeEnum,
            Dictionary<int, vProjectDetail> projectDetailsDictionary,
            ProjectFirmaModels.Models.Tenant tenant): this(currentFirmaSession, projectCustomGridConfigurations, projectCustomGridTypeEnum, projectDetailsDictionary, tenant, true)
        {
        }

        public ProjectCustomGridSpec(FirmaSession currentFirmaSession,
            List<ProjectCustomGridConfiguration> projectCustomGridConfigurations,
            ProjectCustomGridTypeEnum projectCustomGridTypeEnum,
            Dictionary<int, vProjectDetail> projectDetailsDictionary,
            ProjectFirmaModels.Models.Tenant tenant, bool shouldShowDeleteColumn)
        {
            bool userHasTagManagePermissions = new FirmaAdminFeature().HasPermissionByFirmaSession(currentFirmaSession);
            bool userHasDeletePermissionsAndShouldShowColumn = new ProjectDeleteFeature().HasPermissionByFirmaSession(currentFirmaSession) && shouldShowDeleteColumn;
            bool userHasEditProjectAsAdminPermissions = new ProjectEditAsAdminFeature().HasPermissionByFirmaSession(currentFirmaSession);
            bool userHasReportDownloadPermissions = new ReportTemplateGenerateReportsFeature().HasPermissionByFirmaSession(currentFirmaSession);
            var geospatialAreas = HttpRequestStorage.DatabaseEntities.vGeospatialAreas.Where(x => x.TenantID == tenant.TenantID).ToDictionary(x => x.GeospatialAreaID);
            var projectCustomAttributes = HttpRequestStorage.DatabaseEntities.vProjectCustomAttributeValues.Where(x => x.TenantID == tenant.TenantID)
                .GroupBy(x => x.ProjectID)
                .ToDictionary(grp => grp.Key, y => y.ToList());
            var classifications = HttpRequestStorage.DatabaseEntities.Classifications.ToDictionary(x => x.ClassificationID);

            var projectGeospatialAreas = HttpRequestStorage.DatabaseEntities.ProjectGeospatialAreas
                .GroupBy(x => x.ProjectID).ToDictionary(grp => grp.Key, y => y.ToList());
            var projectClassifications = HttpRequestStorage.DatabaseEntities.ProjectClassifications
                .GroupBy(x => x.ProjectID).ToDictionary(grp => grp.Key, y => y.ToList());
            var taxonomyLeafs = HttpRequestStorage.DatabaseEntities.TaxonomyLeafs.ToDictionary(x => x.TaxonomyLeafID);
            var projectLabel = FieldDefinitionEnum.Project.ToType().GetFieldDefinitionLabel();
            bool hasProjectApprovalPermissionBySession =
                new ProjectApproveFeature().HasPermissionByFirmaSession(currentFirmaSession);
            var statusUpdateLabel = FieldDefinitionEnum.StatusUpdate.ToType().GetFieldDefinitionLabel();
            var sitkaAdminPersonIDs =
                HttpRequestStorage.DatabaseEntities.AllPeople.Where(x =>
                    x.RoleID == ProjectFirmaModels.Models.Role.ESAAdmin.RoleID).Select(x => x.PersonID).ToList();

            // Mandatory fields before
            AddMandatoryFieldsBefore(userHasTagManagePermissions, userHasReportDownloadPermissions, userHasDeletePermissionsAndShouldShowColumn, projectCustomGridTypeEnum);
            
            // Implement configured fields here
            AddConfiguredFields(currentFirmaSession
                , projectCustomGridConfigurations
                , userHasEditProjectAsAdminPermissions
                , projectDetailsDictionary
                , geospatialAreas
                , classifications
                , taxonomyLeafs
                , projectGeospatialAreas
                , projectCustomAttributes
                , projectClassifications
                , projectLabel
                , hasProjectApprovalPermissionBySession
                , statusUpdateLabel
                , sitkaAdminPersonIDs);

            // Mandatory fields appearing AFTER configurable fields
            AddMandatoryFieldsAfter(userHasTagManagePermissions);
        }

        private void AddConfiguredFields(FirmaSession currentFirmaSession
            , List<ProjectCustomGridConfiguration> projectCustomGridConfigurations
            , bool userHasEditProjectAsAdminPermissions
            , Dictionary<int, vProjectDetail> projectDetailsDictionary
            , Dictionary<int, ProjectFirmaModels.Models.vGeospatialArea> geospatialAreaDictionary
            , Dictionary<int, ProjectFirmaModels.Models.Classification> classificationDictionary
            , Dictionary<int, ProjectFirmaModels.Models.TaxonomyLeaf> taxonomyLeafDictionary
            , Dictionary<int, List<ProjectFirmaModels.Models.ProjectGeospatialArea>> projectGeospatialAreaDictionary
            , Dictionary<int, List<ProjectFirmaModels.Models.vProjectCustomAttributeValue>> projectCustomAttributeDictionary
            , Dictionary<int, List<ProjectFirmaModels.Models.ProjectClassification>> projectClassificationDictionary
            , string projectLabel
            , bool hasProjectApprovalPermissionBySession
            , string statusUpdateLabel
            , List<int> sitkaAdminPersonIDs)
        {
            foreach (var projectCustomGridConfiguration in projectCustomGridConfigurations.OrderBy(x => x.SortOrder))
            {
                if (projectCustomGridConfiguration.ProjectCustomAttributeType != null)
                {
                    if (projectCustomGridConfiguration.ProjectCustomAttributeType.HasViewPermission(currentFirmaSession))
                    {
                        AddProjectCustomGridCustomAttributeField(projectCustomGridConfiguration, projectCustomAttributeDictionary);
                    }
                }
                else if (projectCustomGridConfiguration.GeospatialAreaType != null)
                {
                    AddProjectCustomGridGeospatialAreaField(projectCustomGridConfiguration, geospatialAreaDictionary, projectGeospatialAreaDictionary);
                }
                else if (projectCustomGridConfiguration.ClassificationSystem != null)
                {
                    AddProjectCustomGridClassificationSystemField(projectCustomGridConfiguration, classificationDictionary, projectClassificationDictionary);
                }
                else
                {
                    AddProjectCustomGridField(currentFirmaSession, projectCustomGridConfiguration,
                        userHasEditProjectAsAdminPermissions
                        , projectDetailsDictionary
                        , taxonomyLeafDictionary
                        , projectLabel
                        , hasProjectApprovalPermissionBySession
                        , statusUpdateLabel
                        , sitkaAdminPersonIDs);
                }
            }
        }

        private static string MakeFactSheetUrlJson(ProjectFirmaModels.Models.Project project)
        {

            // Trusted markup (icon + sr-only label), so the project name embedded in it must be encoded here.
            return HtmlLinkObject.WithHtmlDisplayText($"{FirmaAgGridHtmlHelpers.FactSheetIcon.ToString().Replace("\"", "'")}<span class='sr-only'>Download the Fact Sheet for {HttpUtility.HtmlEncode(project.ProjectName)}</span>", project.GetFactSheetUrl()).ToJsonObjectForAgGrid();
            
        }

        private void AddMandatoryFieldsBefore(bool userHasTagManagePermissions, bool userHasReportDownloadPermissions,
            bool userHasDeletePermissions, ProjectCustomGridTypeEnum projectCustomGridTypeEnum)
        {
            switch (projectCustomGridTypeEnum)
            {
                case ProjectCustomGridTypeEnum.Default:
                case ProjectCustomGridTypeEnum.Full:
                    if (userHasTagManagePermissions)
                    {
                        BulkTagModalDialogForm = new BulkTagModalDialogForm(
                            SitkaRoute<TagController>.BuildUrlFromExpression(x => x.BulkTagProjects(null)),
                            $"Tag Checked {FieldDefinitionEnum.Project.ToType().GetFieldDefinitionLabelPluralized()}",
                            $"Tag {FieldDefinitionEnum.Project.ToType().GetFieldDefinitionLabelPluralized()}");
                    }
                    if (userHasTagManagePermissions)
                    {
                        AddCheckBoxColumn();
                        Add("ProjectIDForModal", x => x.ProjectID, 0);
                    }
                    if (userHasDeletePermissions)
                    {
                        Add("delete", x => AgGridHtmlHelpers.MakeDeleteIconAndLinkBootstrap(x.GetDeleteUrl(), true), 30,
                            AgGridColumnFilterType.None);
                    }
                    Add("download fact sheet",
                        x => MakeFactSheetUrlJson(x), 30,
                        AgGridColumnFilterType.HtmlLinkJsonWithNoFilter);
                    break;
                case ProjectCustomGridTypeEnum.Reports:
                    if (userHasReportDownloadPermissions)
                    {
                        GenerateReportModalDialogForm = new SelectProjectsModalDialogForm(
                            SitkaRoute<ReportsController>.BuildUrlFromExpression(x =>
                                x.SelectReportToGenerateFromSelectedProjects()), $"Generate Reports", $"Confirm Report Generation");
                    }
                    AddCheckBoxColumn();
                    Add("ProjectIDForModal", x => x.ProjectID, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(projectCustomGridTypeEnum), projectCustomGridTypeEnum, null);
            }
        }

        private void AddMandatoryFieldsAfter(bool userHasTagManagePermissions)
        {
            if (userHasTagManagePermissions)
            {
                Add("Tags",
                    x => !x.ProjectTags.Any()
                        ? string.Empty
                        : x.ProjectTags.Select(y => y.Tag).GetDisplayNamesAsUrlListForAgGrid(), 100,
                    AgGridColumnFilterType.HtmlLinkListJson);
            }
        }
    }
}
