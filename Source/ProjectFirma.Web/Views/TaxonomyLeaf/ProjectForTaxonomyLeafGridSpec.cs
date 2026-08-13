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

namespace ProjectFirma.Web.Views.TaxonomyLeaf
{
    public class ProjectForTaxonomyLeafGridSpec : GridSpec<ProjectFirmaModels.Models.Project>
    {
        public ProjectForTaxonomyLeafGridSpec(FirmaSession currentFirmaSession, bool allowTaggingFunctionality, ProjectFirmaModels.Models.TaxonomyLeaf taxonomyLeaf)
        {
            var userHasTagManagePermissions = new FirmaAdminFeature().HasPermissionByFirmaSession(currentFirmaSession);
            if (userHasTagManagePermissions && allowTaggingFunctionality)
            {
                BulkTagModalDialogForm = new BulkTagModalDialogForm(SitkaRoute<TagController>.BuildUrlFromExpression(x => x.BulkTagProjects(null)), $"Tag Checked {FieldDefinitionEnum.Project.ToType().GetFieldDefinitionLabelPluralized()}", $"Tag {FieldDefinitionEnum.Project.ToType().GetFieldDefinitionLabelPluralized()}");
                AddCheckBoxColumn();
                Add("ProjectID", x => x.ProjectID, 0);
            }

            Add("Fact Sheet", x => UrlTemplate.MakeHrefStringWithHtmlLinkText(x.GetFactSheetUrl(), FirmaAgGridHtmlHelpers.FactSheetIcon.ToString()), 30, AgGridColumnFilterType.None);
            Add(FieldDefinitionEnum.ProjectName.ToType().ToGridHeaderString(), x => new HtmlLinkObject(x.ProjectName, x.GetDetailUrl()).ToJsonObjectForAgGrid(), 300, AgGridColumnFilterType.HtmlLinkJson);
            if (MultiTenantHelpers.GetTenantAttributeFromCache().EnableSecondaryProjectTaxonomyLeaf)
            {
                Add($"Relationship to {FieldDefinitionEnum.TaxonomyLeaf.ToType().GetFieldDefinitionLabel()}",
                    x => x.TaxonomyLeafID == taxonomyLeaf.TaxonomyLeafID ? "Primary" : "Secondary",
                    100, AgGridColumnFilterType.SelectFilterStrict);
            }
            if (MultiTenantHelpers.HasCanStewardProjectsOrganizationRelationship())
            {
                Add(FieldDefinitionEnum.ProjectsStewardOrganizationRelationshipToProject.ToType().ToGridHeaderString(), x => x.GetCanStewardProjectsOrganization()?.GetShortNameAsUrlForAgGrid() ?? string.Empty, 150,
                    AgGridColumnFilterType.HtmlLinkJson);
            }
            Add(FieldDefinitionEnum.IsPrimaryContactOrganization.ToType().ToGridHeaderString(), x => x.GetPrimaryContactOrganization().GetShortNameAsUrlForAgGrid(), 150, AgGridColumnFilterType.HtmlLinkJson);
            Add(FieldDefinitionEnum.ProjectStage.ToType().ToGridHeaderString(), x => x.ProjectStage.GetProjectStageDisplayName(), 90, AgGridColumnFilterType.SelectFilterStrict);
            Add(FieldDefinitionEnum.PlanningDesignStartYear.ToType().ToGridHeaderString(), x => ProjectModelExtensions.GetPlanningDesignStartYear(x), 90, AgGridColumnFilterType.SelectFilterStrict);
            Add(FieldDefinitionEnum.ImplementationStartYear.ToType().ToGridHeaderString(), x => ProjectModelExtensions.GetImplementationStartYear(x), 115, AgGridColumnFilterType.SelectFilterStrict);
            Add(FieldDefinitionEnum.CompletionYear.ToType().ToGridHeaderString(), x => ProjectModelExtensions.GetCompletionYear(x), 90, AgGridColumnFilterType.SelectFilterStrict);
            Add(FieldDefinitionEnum.FundingType.ToType().ToGridHeaderString(), x => x.FundingType?.FundingTypeDisplayName, 300, AgGridColumnFilterType.SelectFilterStrict);
            Add(FieldDefinitionEnum.EstimatedTotalCost.ToType().ToGridHeaderString(), x => x.GetEstimatedTotalRegardlessOfFundingType(), 110, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
            Add(FieldDefinitionEnum.SecuredFunding.ToType().ToGridHeaderString(), x => x.GetSecuredFunding(), 110, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
            Add(FieldDefinitionEnum.TargetedFunding.ToType().ToGridHeaderString(), x => x.GetTargetedFunding(), 100, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
            Add(FieldDefinitionEnum.NoFundingSourceIdentified.ToType().ToGridHeaderString(), x => x.GetNoFundingSourceIdentifiedAmount(), 110, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
            foreach (var geospatialAreaType in new List<GeospatialAreaType>())
            {
                Add($"{geospatialAreaType.GeospatialAreaTypeNamePluralized}", a => a.GetProjectGeospatialAreaNamesAsHyperlinksForAgGrid(geospatialAreaType), 350, AgGridColumnFilterType.HtmlLinkListJson);
            }
            Add(FieldDefinitionEnum.ProjectDescription.ToType().ToGridHeaderString(), x => x.ProjectDescription, 300);
            if (userHasTagManagePermissions)
            {
                Add("Tags", x => x.ProjectTags.Select(pt => pt.Tag).ToList().GetDisplayNamesAsUrlListForAgGrid(), 100, AgGridColumnFilterType.HtmlLinkListJson);    
            }
        }
    }
}
