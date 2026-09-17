/*-----------------------------------------------------------------------
<copyright file="MultiTenantHelpers.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.Entity.Spatial;
using System.Linq;
using LtInfo.Common;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.ModalDialog;
using MoreLinq;
using ProjectFirma.Web.Controllers;
using ProjectFirma.Web.Models;
using ProjectFirma.Web.Security;
using ProjectFirmaModels.Models;
using ProjectFirma.Web.Views;

namespace ProjectFirma.Web.Common
{
    public static class MultiTenantHelpers
    {
        private static List<TenantAttribute> TenantAttributeCache = new List<TenantAttribute>();

        public static void ClearTenantAttributeCacheForAllTenants()
        {
            TenantAttributeCache = new List<TenantAttribute>();
        }

        private static readonly EnglishPluralizationService PluralizationService = new EnglishPluralizationService();

        public static Tenant GetTenantFromHostUrl(Uri urlHost)
        {
            var tenant = Tenant.All.Where(x => urlHost.Host.Equals(FirmaWebConfiguration.FirmaEnvironment.GetCanonicalHostNameForEnvironment(x), StringComparison.InvariantCultureIgnoreCase)).SingleOrFallback(() => FirmaWebConfiguration.DefaultTenant);
            Check.RequireNotNull(tenant, $"[GetTenantFromHostUrl] Could not determine tenant from host \"{urlHost}\"");
            return tenant;
        }

        public static TenantAttribute GetTenantAttributeFromCache()
        {
            var tenantAttribute = TenantAttributeCache.SingleOrDefault(ta => ta.TenantID == HttpRequestStorage.DatabaseEntities.TenantID);
            if (tenantAttribute == null)
            {
                tenantAttribute = HttpRequestStorage.DatabaseEntities.TenantAttributes
                    .Include(x => x.PrimaryContactPerson)
                    .Include(x => x.PrimaryContactPerson.Organization)
                    .Include(x => x.TenantBannerLogoFileResourceInfo.FileResourceDatas)
                    .Include(x => x.TenantFactSheetLogoFileResourceInfo.FileResourceDatas)
                    .Include(x => x.TenantSquareLogoFileResourceInfo.FileResourceDatas)
                    .Include(x => x.TenantStyleSheetFileResourceInfo.FileResourceDatas)
                    .SingleOrDefault();
                TenantAttributeCache.Add(tenantAttribute);
            }
            Check.EnsureNotNull(tenantAttribute, $"You need to add a Tenant Attribute table entry for TenantID {HttpRequestStorage.DatabaseEntities.TenantID}");
            return tenantAttribute;
        }

        public static string GetTaxonomySystemName()
        {
            return FieldDefinitionEnum.TaxonomySystemName.ToType().GetFieldDefinitionLabel();
        }

        public static string GetTaxonomyLeafDisplayNameForProject()
        {
            return FieldDefinitionEnum.TaxonomyLeafDisplayNameForProject.ToType().GetFieldDefinitionLabel();
        }

        public static string GetPerformanceMeasureName()
        {
            return FieldDefinitionEnum.PerformanceMeasure.ToType().GetFieldDefinitionLabel();
        }

        public static string GetPerformanceMeasureNamePluralized()
        {
            return PluralizationService.Pluralize(GetPerformanceMeasureName());
        }

        public static string GetTenantShortDisplayName()
        {
            return GetTenantAttributeFromCache().TenantShortDisplayName;
        }

        public static string GetTenantName()
        {
            return HttpRequestStorage.Tenant.TenantName;
        }

        public static string GetToolDisplayName()
        {
            return GetTenantAttributeFromCache().ToolDisplayName;
        }

        public static string GetTenantSquareLogoUrl()
        {
            return GetTenantAttributeFromCache().TenantSquareLogoFileResourceInfo != null
                ? GetTenantAttributeFromCache().TenantSquareLogoFileResourceInfo.GetFileResourceUrl()
                : "/Content/img/ProjectFirma_Logo_Square.png";
        }

        public static string GetTenantSquareLogScaledAsIconoUrl()
        {
            if (HttpRequestStorage.Tenant == Tenant.NCRPProjectTracker)
            {
                return GetTenantAttributeFromCache().TenantSquareLogoFileResourceInfo != null
                    ? GetTenantAttributeFromCache().TenantSquareLogoFileResourceInfo
                        .FileResourceUrlScaledThumbnail(375)
                    : "/Content/img/ProjectFirma_Logo_Square.png";
            }
            return GetTenantAttributeFromCache().TenantSquareLogoFileResourceInfo != null
                ? GetTenantAttributeFromCache().TenantSquareLogoFileResourceInfo
                    .FileResourceUrlScaledThumbnail(100)
                : "/Content/img/ProjectFirma_Logo_Square.png";
        }

        public static string GetTenantBannerLogoUrl()
        {
            var tenantBannerLogoFileResource = GetTenantAttributeFromCache().TenantBannerLogoFileResourceInfo;
            return tenantBannerLogoFileResource != null
                ? tenantBannerLogoFileResource.GetFileResourceUrl()
                : "/Content/img/ProjectFirma_Logo_2016_FNL.width-600.png";
        }

        public static string GetTenantBannerLogoScaledAsIconUrl()
        {
            return GetTenantAttributeFromCache().TenantBannerLogoFileResourceInfo != null
                ? GetTenantAttributeFromCache().TenantBannerLogoFileResourceInfo
                    .FileResourceUrlScaledThumbnail(32)
                : "/Content/img/ProjectFirma_Logo_2016_FNL.width-600.png";
        }

        public static string GetTenantFactSheetLogoUrl()
        {
            return GetTenantAttributeFromCache().TenantFactSheetLogoFileResourceInfo != null
                ? GetTenantAttributeFromCache().TenantFactSheetLogoFileResourceInfo.GetFileResourceUrl()
                : "/Content/img/ProjectFirma_Logo_Square.png";
        }

        public static string GetTenantStyleSheetUrl()
        {
            return GetTenantAttributeFromCache().TenantStyleSheetFileResourceInfo != null
                ? new SitkaRoute<TenantController>(c => c.Style(HttpRequestStorage.Tenant.TenantName))
                    .BuildUrlFromExpression()
                : "~/Content/Bootstrap/firma/base.theme.css";
        }

        public static DbGeometry GetDefaultBoundingBox()
        {
            return GetTenantAttributeFromCache().DefaultBoundingBox;
        }

        public static int GetMinimumYear()
        {
            return GetTenantAttributeFromCache().MinimumYear;
        }

        public static List<TaxonomyTier> GetTopLevelTaxonomyTiers()
        {
            var taxonomyLevel = GetTaxonomyLevel();
            if (taxonomyLevel == TaxonomyLevel.Trunk)
            {
                return HttpRequestStorage.DatabaseEntities.TaxonomyTrunks.ToList().Select(x => new TaxonomyTier(x)).ToList();
            }

            if (taxonomyLevel == TaxonomyLevel.Branch)
            {
                return HttpRequestStorage.DatabaseEntities.TaxonomyBranches.ToList().Select(x => new TaxonomyTier(x)).ToList();
            }

            return new List<TaxonomyTier>();
        }

        public static TaxonomyLevel GetTaxonomyLevel()
        {
            return GetTenantAttributeFromCache().TaxonomyLevel;
        }

        public static TaxonomyLevel GetAssociatePerformanceMeasureTaxonomyLevel()
        {
            return GetTenantAttributeFromCache().AssociatePerfomanceMeasureTaxonomyLevel;
        }

        public static bool IsTaxonomyLevelTrunk()
        {
            return GetTaxonomyLevel() == TaxonomyLevel.Trunk;
        }

        public static bool IsTaxonomyLevelBranch()
        {
            return GetTaxonomyLevel() == TaxonomyLevel.Branch;
        }

        public static bool IsTaxonomyLevelLeaf()
        {
            return GetTaxonomyLevel() == TaxonomyLevel.Leaf;
        }

        public static bool HasIsPrimaryContactOrganizationRelationship()
        {
            return GetIsPrimaryContactOrganizationRelationship() != null;
        }

        public static bool HasCanStewardProjectsOrganizationRelationship()
        {
            return GetCanStewardProjectsOrganizationRelationship() != null;
        }

        public static bool HasAttachmentTypeConfigured()
        {
            return HttpRequestStorage.DatabaseEntities.AttachmentTypes.Any();
        }

        public static OrganizationRelationshipType GetCanStewardProjectsOrganizationRelationship()
        {
            return HttpRequestStorage.DatabaseEntities.OrganizationRelationshipTypes.SingleOrDefault(x => x.CanStewardProjects);
        }

        public static OrganizationRelationshipType GetOrganizationRelationshipTypeToReportInAccomplishmentsDashboard()
        {
            return HttpRequestStorage.DatabaseEntities.OrganizationRelationshipTypes.SingleOrDefault(x =>
                x.ReportInAccomplishmentsDashboard);
        }

        public static bool DisplayAccomplishmentDashboard()
        {
            return GetTenantAttributeFromCache().EnableAccomplishmentsDashboard;
        }

        public static bool DisplaySimpleAccomplishmentDashboard()
        {
            return GetTenantAttributeFromCache().EnableSimpleAccomplishmentsDashboard;
        }

        public static bool TrackAccomplishments()
        {
            return GetTenantAttributeFromCache().TrackAccomplishments;
        }

        public static bool SetTargetsByGeospatialArea()
        {
            return GetTenantAttributeFromCache().SetTargetsByGeospatialArea;
        }

        public static OrganizationRelationshipType GetIsPrimaryContactOrganizationRelationship()
        {
            return HttpRequestStorage.DatabaseEntities.OrganizationRelationshipTypes.SingleOrDefault(x => x.IsPrimaryContact);
        }

        public static bool ShowProposalsToThePublic()
        {
            return GetTenantAttributeFromCache().ShowProposalsToThePublic;
        }

        public static bool ShowLeadImplementerLogoOnFactSheet()
        {
            return GetTenantAttributeFromCache().ShowLeadImplementerLogoOnFactSheet;
        }

        public static bool ShowPhotoCreditOnFactSheet()
        {
            return GetTenantAttributeFromCache().ShowPhotoCreditOnFactSheet;
        }

        public static List<ClassificationSystem> GetClassificationSystems()
        {
            return HttpRequestStorage.DatabaseEntities.ClassificationSystems.ToList();
        }

        public static bool HasSingleClassificationSystem => (GetClassificationSystems().Count == 1);

        public static List<CustomPage> GetCustomPages()
        {
            return HttpRequestStorage.DatabaseEntities.CustomPages.ToList();
        }

        public static List<CustomPage> GetCustomPages(FirmaMenuItem firmaMenuItem)
        {
            return HttpRequestStorage.DatabaseEntities.CustomPages
                .Where(x => x.FirmaMenuItemID == firmaMenuItem.FirmaMenuItemID).OrderBy(x => x.SortOrder).ThenBy(x => x.CustomPageDisplayName).ToList();
        }

        public static AccomplishmentsDashboardFundingDisplayType GetAccomplishmentsDashboardFundingDisplayType()
        {
            return GetTenantAttributeFromCache().AccomplishmentsDashboardFundingDisplayType;
        }

        public static bool GetAccomplishmentsDashboardIncludeReportingOrganizationType()
        {
            return GetTenantAttributeFromCache()
                .AccomplishmentsDashboardIncludeReportingOrganizationType;
        }

        public static ProjectUpdateSetting GetProjectUpdateConfiguration()
        {
            return GetProjectUpdateConfiguration(HttpRequestStorage.Tenant.TenantID);
        }

        public static ProjectUpdateSetting GetProjectUpdateConfigurationForBackgroundJob(int tenantID)
        {
            return GetProjectUpdateConfiguration(tenantID);
        }

        private static ProjectUpdateSetting GetProjectUpdateConfiguration(int tenantID)
        {
            // need to load the ProjectUpdateSettings to ensure that when this static method is called (e.g. from a background job) it gets the up to date settings
            HttpRequestStorage.DatabaseEntities.AllProjectUpdateSettings.Load();
            var projectUpdateConfiguration = HttpRequestStorage.DatabaseEntities.AllProjectUpdateSettings.SingleOrDefault(x =>
                x.TenantID == tenantID);
            if (projectUpdateConfiguration == null)
            {
                // 3/27/2020 TK - You need to create an entry for your tenant in dbo.ProjectUpdateSetting
                throw new SitkaDisplayErrorException(
                    $"Tenant ID {tenantID} {GetTenantAttributeFromCache().TenantShortDisplayName} does not have a configuration entry for Project Update Settings. Please <a href=\"mailto: {FirmaWebConfiguration.SitkaSupportEmail}?subject=Project Update Settings are not configured\">contact support</a> to get this issue resolved.");
            }
            return projectUpdateConfiguration;
        }

        public static DateTime GetStartDayOfReportingPeriod()
        {
            var projectUpdateConfiguration = GetProjectUpdateConfiguration();
            return projectUpdateConfiguration.ProjectUpdateKickOffDate ?? HttpRequestStorage.Tenant.FiscalYearStartDate;
        }

        public static DateTime GetStartDayOfReportingPeriodForBackgroundJob(int tenantID)
        {
            var projectUpdateConfiguration = GetProjectUpdateConfigurationForBackgroundJob(tenantID);
            return projectUpdateConfiguration.ProjectUpdateKickOffDate.GetValueOrDefault();
        }

        public static DateTime GetEndDayOfReportingPeriod()
        {
            var projectUpdateConfiguration = GetProjectUpdateConfiguration();
            return projectUpdateConfiguration.ProjectUpdateCloseOutDate.GetValueOrDefault();
        }

        public static DateTime GetStartDayOfFiscalYear()
        {
            return HttpRequestStorage.Tenant.FiscalYearStartDate;
        }

        public static bool UseFiscalYears()
        {
            return HttpRequestStorage.Tenant.UseFiscalYears;
        }

        public static string FormatReportingYear(int reportingYear)
        {
            if (UseFiscalYears())
            {
                return $"FY{reportingYear}";
            }
            return reportingYear.ToString();
        }

        public static bool UsesCustomResultsPages(FirmaSession currentFirmaSession)
        {
            return HttpRequestStorage.DatabaseEntities.CustomPages.Any(x => x.FirmaMenuItemID == FirmaMenuItem.Results.FirmaMenuItemID) || UsesCustomFundingStatusPage(currentFirmaSession) || UsesCustomProgressDashboardPage(currentFirmaSession);
        }

        // TODO make this into a check to see if the tenant uses custom funding status pages. For now, it's just the Action Agenda for PSP, so check if the 2 firma page types needed for their custom results page are present for the tenant
        public static bool UsesCustomFundingStatusPage(FirmaSession currentFirmaSession)
        {
            return currentFirmaSession.Tenant == Tenant.ActionAgendaForPugetSound;
        }

        public static void AddFundingStatusMenuItem(LtInfoMenuItem resultsMenu, FirmaSession currentFirmaSession)
        {
            if (UsesCustomFundingStatusPage(currentFirmaSession))
            {
                resultsMenu.AddMenuItem(LtInfoMenuItem.MakeItem(new SitkaRoute<ResultsController>(c => c.FundingStatus()), currentFirmaSession, "Funding Status"));
            }
        }

        public static bool UsesCustomProgressDashboardPage(FirmaSession currentFirmaSession)
        {
            return currentFirmaSession.Tenant == Tenant.SSMPProjectTracker;
        }

        public static bool UsesCustomProjectDashboardPage(FirmaSession currentFirmaSession)
        {
            return currentFirmaSession.Tenant == Tenant.NCRPProjectTracker;
        }

        public static void AddProgressDashboardMenuItem(LtInfoMenuItem resultsMenu, FirmaSession currentFirmaSession)
        {
            if (UsesCustomProgressDashboardPage(currentFirmaSession))
            {
                resultsMenu.AddMenuItem(LtInfoMenuItem.MakeItem(new SitkaRoute<ResultsController>(c => c.ProgressDashboard()), currentFirmaSession, "Progress Dashboard"));
            }
        }

        public static ProjectStewardshipAreaType GetProjectStewardshipAreaType()
        {
            return GetTenantAttributeFromCache().ProjectStewardshipAreaType;
        }

        public static string GetTenantGoogleAnalyticsTrackingCode()
        {
            return GetTenantAttributeFromCache().GoogleAnalyticsTrackingCode;
        }

        public static bool UsesEvaluations()
        {
            return GetTenantAttributeFromCache().EnableEvaluations;
        }
        public static void AddEvaluationsMenuItem(LtInfoMenuItem manageMenu, FirmaSession currentFirmaSession, string menuGroupName)
        {
            if (UsesEvaluations() && new FirmaAdminFeature().HasPermission(currentFirmaSession).HasPermission)
            {

                manageMenu.AddMenuItem(LtInfoMenuItem.MakeItem(new SitkaRoute<EvaluationController>(c => c.Index()), currentFirmaSession, FieldDefinitionEnum.ProjectEvaluation.ToType().GetFieldDefinitionLabelPluralized(), "Group1"));
            }
        }

        public static bool AreGeospatialAreasExternallySourced()
        {
            return GetTenantAttributeFromCache().AreGeospatialAreasExternallySourced;
        }

        public static bool ReportFinancialsAtProjectLevel()
        {
            return GetTenantAttributeFromCache().ReportFinancialsAtProjectLevel;
        }

        public static bool DisplayReportsLink()
        {
            return GetTenantAttributeFromCache().EnableReports;
        }

        public static string GetRelativeUrlForEnvironment(Uri currentUrl,
            FirmaEnvironmentType firmaEnvironmentType)
        {
            var currentTenant = HttpRequestStorage.Tenant;

            string newHost;
            switch (firmaEnvironmentType)
            {
                case FirmaEnvironmentType.Local:
                    newHost = currentTenant.CanonicalHostNameLocal;
                    break;
                case FirmaEnvironmentType.Qa:
                    newHost = currentTenant.CanonicalHostNameQa;
                    break;
                case FirmaEnvironmentType.Prod:
                    newHost = currentTenant.CanonicalHostNameProd;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(firmaEnvironmentType), firmaEnvironmentType, null);
            }

            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = currentUrl.Scheme, 
                Host = newHost, 
                Path = currentUrl.PathAndQuery
            };
            return uriBuilder.ToString();
        }

        private static List<TenantSimple> TenantSimples = new List<TenantSimple>();

        public static List<TenantSimple> GetAllTenantSimples()
        {
            // ReSharper disable twice InconsistentlySynchronizedField
            if (TenantSimples.Any())
            {
                // This CAN return an incomplete set when the app is first booting up. But
                // this tiny window of vulnerability is traded off against never needing a
                // lock in the read-only case, which is 99.9999% of the time this function is called.
                //
                // Since this gets called on every page for every user of every tenant, it seemed
                // wise to optimize the lock away if we could in the majority case.
                //
                // -- SLG 11/03/2020
                return TenantSimples;
            }

            lock (TenantSimples)
            {
                if (!TenantSimples.Any())
                {
                    TenantSimples.AddRange(HttpRequestStorage.DatabaseEntities.AllTenantAttributes.ToList().Select(x => new TenantSimple
                    (
                        x.TenantID,
                        x.TenantShortDisplayName,
                        x.Tenant.CanonicalHostNameLocal,
                        x.Tenant.CanonicalHostNameQa,
                        x.Tenant.CanonicalHostNameProd,
                        x.TenantSquareLogoFileResourceInfo?.GetFileResourceUrl(),
                        x.Tenant.TenantEnabled
                    )));
                }
                return TenantSimples;
            }
        }

        /// <summary>
        /// Tenants that have more than one classification system will return the field definition for Classification.
        /// Otherwise we will return the first classification system name.
        /// </summary>
        public static string GetTenantNameForClassificationForMatchmaker(bool pluralize)
        {
            var allClassificationSystems = HttpRequestStorage.DatabaseEntities.ClassificationSystems.ToList();
            var tenantHasMultipleClassificationSystems = allClassificationSystems.Count > 1;

            if (pluralize)
            {
                return tenantHasMultipleClassificationSystems
                    ? FieldDefinitionEnum.Classification.ToType().GetFieldDefinitionLabelPluralized()
                    : new EnglishPluralizationService().Pluralize(allClassificationSystems.First().ClassificationSystemName);
            }

            return tenantHasMultipleClassificationSystems
                ? FieldDefinitionEnum.Classification.ToType().GetFieldDefinitionLabel()
                : new EnglishPluralizationService().Singularize(allClassificationSystems.First()
                    .ClassificationSystemName);
        }

        /// <summary>
        /// In order to display the appropriate field definition on the matchmaker pages, this
        /// checks the current tenant's taxonomy level and returns an appropriate enum for display.
        /// </summary>
        /// <returns></returns>
        public static FieldDefinitionEnum GetTenantFieldDefinitionEnumForMatchmakerTaxonomy()
        {
            var tenantTaxonomyLevelEnum = GetTenantAttributeFromCache().TaxonomyLevel.ToEnum;
            switch (tenantTaxonomyLevelEnum)
            {
                case TaxonomyLevelEnum.Leaf:
                    return FieldDefinitionEnum.TaxonomyLeaf;
                case TaxonomyLevelEnum.Branch:
                    return FieldDefinitionEnum.TaxonomyBranch;
                case TaxonomyLevelEnum.Trunk:
                    return FieldDefinitionEnum.TaxonomyTrunk;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string GetTenantDisplayNameForMatchmakerSubScoreTypeEnum(MatchmakerSubScoreTypeEnum enumType)
        {
            switch (enumType)
            {
                case MatchmakerSubScoreTypeEnum.Classification:
                    return MultiTenantHelpers.GetTenantNameForClassificationForMatchmaker(true);
                case MatchmakerSubScoreTypeEnum.TaxonomySystem:
                    return MultiTenantHelpers.GetTenantFieldDefinitionEnumForMatchmakerTaxonomy().ToType().GetFieldDefinitionLabelPluralized();
                default:
                    return MatchmakerSubScoreType.ToType(enumType).MatchmakerSubScoreTypeDisplayName;
            }
        }

        public static bool HasSolicitations()
        {
            return GetTenantAttributeFromCache().EnableSolicitations;
        }

        public static string MapServiceUrl()
        {
            var geoServerNamespace = GetTenantAttributeFromCache().GeoServerNamespace;
            return $"{FirmaWebConfiguration.GeoServerUrl}{geoServerNamespace}/wms";
        }

        public static bool EnableDetailedLocationPolygonsOnProjectMap()
        {
            return GetTenantAttributeFromCache().EnableDetailedLocationPolygonsOnProjectMap ?? false;
        }
    }
}
