/*-----------------------------------------------------------------------
<copyright file="UserProjectGridSpec.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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

using System.Collections.Generic;
using System.Linq;
using LtInfo.Common;
using LtInfo.Common.AgGridWrappers;
using LtInfo.Common.Views;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Models;
using ProjectFirma.Web.Security;
using ProjectFirmaModels.Models;

namespace ProjectFirma.Web.Views.Project
{
    public class UserProjectGridSpec : GridSpec<ProjectFirmaModels.Models.Project>
    {
        public UserProjectGridSpec(FirmaSession currentFirmaSession, Person person)
        {
            var userHasTagManagePermissions = new FirmaAdminFeature().HasPermissionByFirmaSession(currentFirmaSession);
            
            Add("Fact Sheet", x => UrlTemplate.MakeHrefStringWithHtmlLinkText(x.GetFactSheetUrl(), FirmaAgGridHtmlHelpers.FactSheetIcon.ToString()), 30, AgGridColumnFilterType.None);
            Add(FieldDefinitionEnum.ProjectName.ToType().ToGridHeaderString(), x => new HtmlLinkObject(x.ProjectName, x.GetDetailUrl()).ToJsonObjectForAgGrid(), 300, AgGridColumnFilterType.HtmlLinkJson);

            Add($"Contact Type(s)", x => string.Join(", ", person.GetListOfContactTypeStringsForProject(x)), 300, AgGridColumnFilterType.Html);

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
            if (MultiTenantHelpers.ReportFinancialsAtProjectLevel())
            {
                Add(FieldDefinitionEnum.FundingType.ToType().ToGridHeaderString(), x => x.FundingType?.FundingTypeDisplayName ?? string.Empty, 300, AgGridColumnFilterType.SelectFilterStrict);
                Add(FieldDefinitionEnum.EstimatedTotalCost.ToType().ToGridHeaderString(), x => x.GetEstimatedTotalRegardlessOfFundingType(), 110, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
                Add(FieldDefinitionEnum.SecuredFunding.ToType().ToGridHeaderString(), x => x.GetSecuredFunding(), 110, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
                Add(FieldDefinitionEnum.TargetedFunding.ToType().ToGridHeaderString(), x => x.GetTargetedFunding(), 100, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
                Add(FieldDefinitionEnum.NoFundingSourceIdentified.ToType().ToGridHeaderString(), x => x.GetNoFundingSourceIdentifiedAmount(), 110, AgGridColumnFormatType.Currency, AgGridColumnAggregationType.Total);
            }

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