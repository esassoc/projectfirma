/*-----------------------------------------------------------------------
<copyright file="FirmaWebConfiguration.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using System.IO;
using System.Linq;
using System.Web.Configuration;
using LtInfo.Common;
using LtInfo.Common.DesignByContract;
using ProjectFirmaModels.Models;

namespace ProjectFirma.Web.Common
{
    public class FirmaWebConfiguration : SitkaWebConfiguration
    {
        public static readonly int MaximumAllowedUploadFileSize = Int32.Parse(SitkaConfiguration.GetRequiredAppSetting("MaximumAllowedUploadFileSize"));
        public static readonly int MaximumAllowedUploadImageSize = Int32.Parse(SitkaConfiguration.GetRequiredAppSetting("MaximumAllowedUploadImageSize"));
        public static readonly string SitkaSupportEmail = SitkaConfiguration.GetRequiredAppSettingNotNullNotEmptyNotWhitespace("SitkaSupportEmail");
        public static readonly string DoNotReplyEmail = SitkaConfiguration.GetRequiredAppSettingNotNullNotEmptyNotWhitespace("DoNotReplyEmail");
        public static readonly string Ogr2OgrExecutable = SitkaConfiguration.GetRequiredAppSetting("Ogr2OgrExecutable");
        public static readonly string OgrInfoExecutable = SitkaConfiguration.GetRequiredAppSetting("OgrInfoExecutable");
        public static readonly int DefaultSupportPersonID = Int32.Parse(SitkaConfiguration.GetRequiredAppSetting("DefaultSupportPersonID"));

        public static readonly TimeSpan HttpRuntimeExecutionTimeout = ((HttpRuntimeSection)WebConfigurationManager.GetSection("system.web/httpRuntime")).ExecutionTimeout;

        public static readonly string PsInfoUrl = SitkaConfiguration.GetRequiredAppSetting("PsInfoUrl");
        public static readonly string PsInfoDataCenterUrl = SitkaConfiguration.GetRequiredAppSetting("PsInfoDataCenterUrl");
        public static readonly string VitalSignsUrl = SitkaConfiguration.GetRequiredAppSetting("VitalSignsUrl");
        public static readonly string NEPAtlasUrl = SitkaConfiguration.GetRequiredAppSetting("NEPAtlasUrl");
        public static readonly string ActionAgendaUrl = SitkaConfiguration.GetRequiredAppSetting("ActionAgendaUrl");
        public static readonly string OngoingProgramsUrl = SitkaConfiguration.GetRequiredAppSetting("OngoingProgramsUrl");
        public static readonly string PSARDashboardUrl = SitkaConfiguration.GetRequiredAppSetting("PSARDashboardUrl");
        public static readonly string SpatialHubUrl = SitkaConfiguration.GetRequiredAppSetting("SpatialHubUrl");
        public static readonly string PsInfoPostOrganizationUrl = SitkaConfiguration.GetRequiredAppSetting("PsInfoPostOrganizationUrl");
        public static readonly string PsInfoApiKey = SitkaConfiguration.GetRequiredAppSetting("PsInfoApiKey");


        public static readonly string KeystoneUserProfileUrl = SitkaConfiguration.GetRequiredAppSetting("KeystoneUserProfileUrl");
        public static readonly string KeystoneInviteUserUrl = SitkaConfiguration.GetRequiredAppSetting("KeystoneInviteUserUrl");
        public static readonly Guid KeystoneWebServiceApplicationGuid = Guid.Parse(SitkaConfiguration.GetRequiredAppSetting("KeystoneWebServiceApplicationGuid"));
        
        public static readonly string KeystoneOpenIDUrl = SitkaConfiguration.GetRequiredAppSetting("KeystoneOpenIDUrl");
        public static readonly string ProjectFirmaKeystoneApiClientCertificatePfxBase64 = SitkaConfiguration.GetRequiredAppSetting("ProjectFirmaKeystoneApiClientCertificatePfxBase64");
        public static readonly string ProjectFirmaKeystoneApiClientCertificatePfxPassword = SitkaConfiguration.GetRequiredAppSetting("ProjectFirmaKeystoneApiClientCertificatePfxPassword");
        public static readonly DirectoryInfo LogFileFolder = ParseLogFileFolder();

        public static readonly string RecaptchaPublicKey = SitkaConfiguration.GetRequiredAppSetting("RecaptchaPublicKey");
        public static readonly string RecaptchaPrivateKey = SitkaConfiguration.GetRequiredAppSetting("RecaptchaPrivateKey");
        public static readonly string RecaptchaValidatorUrl = SitkaConfiguration.GetRequiredAppSettingNotNullNotEmptyNotWhitespace("RecaptchaValidatorUrl");

        public static readonly bool ImpersonationAllowedInEnvironment = Boolean.Parse(SitkaConfiguration.GetRequiredAppSetting("ImpersonationAllowed"));

        public static readonly FirmaEnvironment FirmaEnvironment = FirmaEnvironment.MakeFirmaEnvironment(SitkaConfiguration.GetRequiredAppSetting("FirmaEnvironment"));

        public static readonly int DefaultTenantID = int.Parse(SitkaConfiguration.GetRequiredAppSetting("DefaultTenantID"));
        public static readonly bool RedirectToDefaultTenantCanonicalHostName = Boolean.Parse(SitkaConfiguration.GetRequiredAppSetting("RedirectToDefaultTenantCanonicalHostName"));
        public static readonly bool RedirectToHttps = Boolean.Parse(SitkaConfiguration.GetRequiredAppSetting("RedirectToHttps"));

        public static readonly Tenant DefaultTenant = Tenant.All.Single(x => x.TenantID == DefaultTenantID);
        public static readonly string DefaultTenantCanonicalHostName = FirmaEnvironment.GetCanonicalHostNameForEnvironment(DefaultTenant);

        public static readonly AuthenticationType AuthenticationType = SitkaConfiguration.GetRequiredAppSetting("AuthenticationType").ParseAsEnum<AuthenticationType>();

        public static List<string> CanonicalHostNames => Tenant.All.OrderBy(x => x.TenantID).Select(x => FirmaEnvironment.GetCanonicalHostNameForEnvironment(x)).ToList();

        public static HashSet<string> CanonicalHostNameSet =>
            Tenant.All
                .Select(x => FirmaEnvironment.GetCanonicalHostNameForEnvironment(x))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

        public static string GeoServerUrl = SitkaConfiguration.GetRequiredAppSetting("GeoServerUrl");
        public static bool TenantDropdownEnabled = Boolean.Parse(SitkaConfiguration.GetRequiredAppSetting("TenantDropdownEnabled"));

        // public static readonly string HttpAuthenticationUrlHost = SitkaConfiguration.GetRequiredAppSetting("HttpAuthenticationUrlHost");
        // public static readonly string HttpAuthenticationUsername = SitkaConfiguration.GetRequiredAppSetting("HttpAuthenticationUsername");
        // public static readonly string HttpAuthenticationPassword = SitkaConfiguration.GetRequiredAppSetting("HttpAuthenticationPassword");

        public static readonly string NCRPHomeUrl = SitkaConfiguration.GetRequiredAppSetting("NCRPHomeUrl");
        public static readonly string SSMPHomeUrl = SitkaConfiguration.GetRequiredAppSetting("SSMPHomeUrl");
        public static readonly int SSMPAcresConstructedTarget = Int32.Parse(SitkaConfiguration.GetRequiredAppSetting("SSMPAcresConstructedTarget"));
        public static readonly string TCSHomeUrl = SitkaConfiguration.GetRequiredAppSetting("TCSHomeUrl");

        public static readonly string MapBoxApiKey = SitkaConfiguration.GetRequiredAppSetting("MapBoxApiKey");
        
        public static readonly string LTInfoApiKey = SitkaConfiguration.GetRequiredAppSetting("LTInfoApiKey");
        public static readonly string PendingProjectsUploadExcelTemplate = SitkaConfiguration.GetRequiredAppSetting("PendingProjectsUploadExcelTemplate");

        // Feature Flag Settings
        // FeatureMatchMakerEnabled now has shipped, but leaving this in place so we can see readily how to set up the next Feature.
        //public static readonly bool FeatureMatchMakerEnabled = Boolean.Parse(SitkaConfiguration.GetRequiredAppSetting("FeatureMatchMakerEnabled"));

        public static string GetCanonicalHost(string hostName, bool useApproximateMatch)
        {
            //First search for perfect match
            var canonicalHostNames = CanonicalHostNames;
            var result = canonicalHostNames.FirstOrDefault(h => string.Equals(h, hostName, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(result) || !useApproximateMatch)
            {
                return result;
            }

            var canonicalHost = canonicalHostNames.FirstOrDefault(h => h.EndsWith(hostName, StringComparison.InvariantCultureIgnoreCase));
            if (canonicalHost == null && FirmaEnvironment.FirmaEnvironmentType == FirmaEnvironmentType.Prod)
            {
                if (hostName.Equals("swcdemo.projectfirma.com", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Tenant.IdahoAssociatonOfSoilConservationDistricts.CanonicalHostNameProd;
                }
                // Redirect to default tenant home if domain is bad in Prod 
                return DefaultTenant.CanonicalHostNameProd;
            }

            return canonicalHost;
        }

        public static string GetCanonicalHostForBackgroundJob(int tenantID)
        {
            return Tenant.All.Where(x => x.TenantID == tenantID).Select(x => FirmaEnvironment.GetCanonicalHostNameForEnvironment(x)).SingleOrDefault();
        }



        private static DirectoryInfo ParseLogFileFolder()
        {
            const string appSettingKeyName = "LogFileFolder";
            var logFileFolder = SitkaConfiguration.GetRequiredAppSetting(appSettingKeyName);
            Check.RequireDirectoryExists(logFileFolder, "App setting {0} must be a folder that exists, folder does not exist.");
            return new DirectoryInfo(logFileFolder);
        }
    }

}
