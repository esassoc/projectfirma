/*-----------------------------------------------------------------------
<copyright file="ProjectDashboardChartsViewData.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Controllers;
using ProjectFirma.Web.Models;
using ProjectFirma.Web.Views.Shared;
using ProjectFirmaModels.Models;
using System.Collections.Generic;
using System.Web;

namespace ProjectFirma.Web.Views.Results
{
    public class ProjectDashboardChartsViewData : FirmaUserControlViewData
    {
        public readonly ViewGoogleChartViewData UnderservedCommunitiesViewGoogleChartViewData;
        public readonly ViewGoogleChartViewData ProjectsByOwnerOrgTypeViewGoogleChartViewData;
        public readonly ViewGoogleChartViewData ProjectsByCountyAndTribalLandViewGoogleChartViewData;
        public readonly ViewGoogleChartViewData ProjectsByTribalRegionViewGoogleChartViewData;
        public readonly ViewGoogleChartViewData ProjectsByProjectTypeViewGoogleChartViewData;
        public readonly ViewGoogleChartViewData ProjectStagesViewGoogleChartViewData;
        public readonly ViewGoogleChartViewData ProjectsByTATypeViewGoogleChartViewData;
        public readonly ViewGoogleChartViewData AcresCompletedViaImplementationProjectsOneViewGoogleChartViewData;
        public readonly ViewGoogleChartViewData AcresCompletedViaImplementationProjectsTwoViewGoogleChartViewData;
        public bool UnderservedCommunitiesHasData { get; }
        public double UnderservedTotal { get; }
        public bool ProjectsByOwnerOrgTypeHasData { get; }
        public bool ProjectsByCountyAndTribalLandHasData { get; }
        public bool ProjectsByProjectTypeHasData { get; }
        public bool ProjectStagesHasData { get; }
        public double ProjectStagesTotal { get; }
        public bool FundingOrganizationsHasData { get; }
        public int NumberOfTribalProjects { get; }

        // capacity enhancement and ta
        public int AwardedTAAndCapacityEnhancementProjectCount { get; }
        public decimal TAInvestment { get; }
        public decimal AcresImpactedViaTAProjects { get; }
        public double TotalLeveraged { get; }

        // water supply and quality
        public int ImprovedWaterSupplyOrQualityProjectCount { get; }
        public double WaterQualitySedimentStabilization { get; }
        public double WaterSupplyImprovedReliabilityAFY { get; }
        public double WaterSupplyImprovedReliabilityNumberOfHouseholds { get; }
        public double AvoidedCosts { get; }

        public ProjectDashboardChartsViewData(GoogleChartJson underservedCommunitiesGoogleChart, int disadvantagedCommunityStatusGeospatialAreaTypeID, GoogleChartJson projectsByOwnerOrgTypeGoogleChart, GoogleChartJson projectsByCountyAndTribalLandGoogleChart, GoogleChartJson projectsByTribalRegionGoogleChart, int countyGeospatialAreaTypeID, int tribalLandGeospatialAreaTypeID,
            GoogleChartJson projectsByProjectTypeGoogleChart, int projectTypeClassificationSystemID, GoogleChartJson projectStagesGoogleChart, int numberOfTribalProjects,
            int awardedTAAndCapacityEnhancementProjectCount, decimal ncrpTAInvestment, decimal acresImpactedViaTAProjects, double totalLeveraged,
            int improvedWaterSupplyOrQualityProjectCount, double waterQualitySedimentStabilization, double waterSupplyImprovedAFY, double waterSupplyImprovedHouseholdsImpacted, double avoidedCosts,
            GoogleChartJson projectsByTATypeGoogleChart, GoogleChartJson acresCompletedViaImplementationProjectsChartOne, GoogleChartJson acresCompletedViaImplementationProjectsChartTwo)
        {
            UnderservedCommunitiesViewGoogleChartViewData = new ViewGoogleChartViewData(underservedCommunitiesGoogleChart, underservedCommunitiesGoogleChart.GoogleChartConfiguration.Title, 350, true, true);
            var geospatialAreaTypeIndexUrl = UrlTemplate.MakeHrefString(SitkaRoute<GeospatialAreaController>.BuildUrlFromExpression(c => c.Index(disadvantagedCommunityStatusGeospatialAreaTypeID)), "Disadvantaged Community Status", new Dictionary<string, string> { { "target", "_blank" } });
            UnderservedCommunitiesViewGoogleChartViewData.ChartTitleWithLink = new HtmlString($"<b>{FieldDefinitionEnum.Project.ToType().GetFieldDefinitionLabelPluralized()} by {geospatialAreaTypeIndexUrl}</b>");
            UnderservedCommunitiesHasData = false;
            UnderservedTotal = 0;
            foreach (var slice in ((GooglePieChartConfiguration)underservedCommunitiesGoogleChart.GoogleChartConfiguration).Slices)
            {
                UnderservedCommunitiesHasData = UnderservedCommunitiesHasData || slice.Value > 0;
                UnderservedTotal += slice.Value;
            }

            ProjectsByOwnerOrgTypeViewGoogleChartViewData = new ViewGoogleChartViewData(projectsByOwnerOrgTypeGoogleChart, projectsByOwnerOrgTypeGoogleChart.GoogleChartConfiguration.Title, 400, true);
            ProjectsByOwnerOrgTypeHasData = projectsByOwnerOrgTypeGoogleChart.HasData();

            ProjectsByCountyAndTribalLandViewGoogleChartViewData = new ViewGoogleChartViewData(projectsByCountyAndTribalLandGoogleChart, projectsByCountyAndTribalLandGoogleChart.GoogleChartConfiguration.Title, 350, true);
            var countyIndexUrl = UrlTemplate.MakeHrefString(SitkaRoute<GeospatialAreaController>.BuildUrlFromExpression(c => c.Index(countyGeospatialAreaTypeID)), "County", new Dictionary<string, string> { { "target", "_blank" } });
            var tribalLandIndexUrl = UrlTemplate.MakeHrefString(SitkaRoute<GeospatialAreaController>.BuildUrlFromExpression(c => c.Index(tribalLandGeospatialAreaTypeID)), "Tribal Land");
            ProjectsByCountyAndTribalLandViewGoogleChartViewData.ChartTitleWithLink = new HtmlString($"<b>{FieldDefinitionEnum.Project.ToType().GetFieldDefinitionLabelPluralized()} by {countyIndexUrl}</b>");
            ProjectsByCountyAndTribalLandHasData = projectsByCountyAndTribalLandGoogleChart.HasData();

            ProjectsByTribalRegionViewGoogleChartViewData = new ViewGoogleChartViewData(projectsByTribalRegionGoogleChart, projectsByTribalRegionGoogleChart.GoogleChartConfiguration.Title, 350, true);
            ProjectsByTribalRegionViewGoogleChartViewData.ChartTitleWithLink = new HtmlString($"<b>{FieldDefinitionEnum.Project.ToType().GetFieldDefinitionLabelPluralized()} by Tribal Region</b>");

            ProjectsByProjectTypeViewGoogleChartViewData = new ViewGoogleChartViewData(projectsByProjectTypeGoogleChart, projectsByProjectTypeGoogleChart.GoogleChartConfiguration.Title, 350, true);
            var projectTypesIndexUrl = UrlTemplate.MakeHrefString(SitkaRoute<ProgramInfoController>.BuildUrlFromExpression(c => c.ClassificationSystem(projectTypeClassificationSystemID)), "Project Types", new Dictionary<string, string> { { "target", "_blank" } });
            ProjectsByProjectTypeViewGoogleChartViewData.ChartTitleWithLink = new HtmlString($"<b>{FieldDefinitionEnum.Project.ToType().GetFieldDefinitionLabelPluralized()} by {projectTypesIndexUrl}</b>");
            ProjectsByProjectTypeHasData = projectsByProjectTypeGoogleChart.HasData();

            ProjectStagesViewGoogleChartViewData = new ViewGoogleChartViewData(projectStagesGoogleChart, projectStagesGoogleChart.GoogleChartConfiguration.Title, 350, true, true);
            ProjectStagesHasData = false;
            ProjectStagesTotal = 0;
            foreach (var slice in ((GooglePieChartConfiguration)projectStagesGoogleChart.GoogleChartConfiguration).Slices)
            {
                ProjectStagesHasData = ProjectStagesHasData || slice.Value > 0;
                ProjectStagesTotal += slice.Value;
            }

            NumberOfTribalProjects = numberOfTribalProjects;
            AwardedTAAndCapacityEnhancementProjectCount = awardedTAAndCapacityEnhancementProjectCount;
            TAInvestment = ncrpTAInvestment;
            AcresImpactedViaTAProjects = acresImpactedViaTAProjects;
            TotalLeveraged = totalLeveraged;

            ImprovedWaterSupplyOrQualityProjectCount = improvedWaterSupplyOrQualityProjectCount;
            WaterQualitySedimentStabilization = waterQualitySedimentStabilization;
            WaterSupplyImprovedReliabilityAFY = waterSupplyImprovedAFY;
            WaterSupplyImprovedReliabilityNumberOfHouseholds = waterSupplyImprovedHouseholdsImpacted;
            AvoidedCosts = avoidedCosts;

            ProjectsByTATypeViewGoogleChartViewData = new ViewGoogleChartViewData(projectsByTATypeGoogleChart, projectsByTATypeGoogleChart.GoogleChartConfiguration.Title, 350, true);
            AcresCompletedViaImplementationProjectsOneViewGoogleChartViewData = new ViewGoogleChartViewData(acresCompletedViaImplementationProjectsChartOne, acresCompletedViaImplementationProjectsChartOne.GoogleChartConfiguration.Title, 350, false);
            AcresCompletedViaImplementationProjectsTwoViewGoogleChartViewData = new ViewGoogleChartViewData(acresCompletedViaImplementationProjectsChartTwo, acresCompletedViaImplementationProjectsChartTwo.GoogleChartConfiguration.Title, 350, false);

        }
    }
}