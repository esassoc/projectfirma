/*-----------------------------------------------------------------------
<copyright file="HttpRequestStorage.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using System.Net;
using System.Security.Principal;
using System.Web;
using LtInfo.Common;
using ProjectFirmaModels.Models;
using LtInfo.Common.DesignByContract;
using Microsoft.Owin.Security;
using ProjectFirma.Web.Models;
using Person = ProjectFirmaModels.Models.Person;

namespace ProjectFirma.Web.Common
{
    public class HttpRequestStorage : SitkaHttpRequestStorage
    {
        static HttpRequestStorage()
        {
            LtInfoEntityTypeLoaderFactoryFunction = () => MakeNewContext(false);
        }

        protected override List<string> BackingStoreKeys
        {
            get { return new List<string>(); }
        }

        public static IPrincipal GetHttpContextUserThroughOwin()
        {
            Check.EnsureNotNull(HttpContext.Current, "Null HttpContext.Current!");
            return GetHttpContextAuthenticationThroughOwin().User;
        }

        public static IAuthenticationManager GetHttpContextAuthenticationThroughOwin()
        {
            return HttpContext.Current.GetOwinContext().Authentication;
        }

        public static FirmaSession FirmaSession
        {
            get => GetValueOrDefault(FirmaSessionKey, FirmaSessionModelExtensions.GetAnonymousFirmaSession);
            set => SetValue(FirmaSessionKey, value);
        }

        public static Person Person
        {
            get => FirmaSession.Person;
        }

        public static Tenant Tenant
        {
            get
            {
                return GetValueOrDefault(TenantKey,
                    () =>
                    {
                        var httpContext = HttpContext.Current;
                        if (httpContext != null)
                        {
                            var tenantFromHostUrl = MultiTenantHelpers.GetTenantFromHostUrl(httpContext.Request.Url);
                            if (!tenantFromHostUrl.TenantEnabled)
                            {
                                // 410 Gone: a client hitting a disabled tenant is expected and not App Alerts email worthy (see NonActionableErrorClassifier)
                                throw new SitkaDisplayErrorExceptionWithHttpCode(HttpStatusCode.Gone, $"Tenant {tenantFromHostUrl.TenantName} is disabled");
                            }
                            return tenantFromHostUrl;
                        }
                        else
                        {
                            return FirmaWebConfiguration.DefaultTenant;
                        }
                    });
            }
            set => SetValue(TenantKey, value);
        }


        public static DatabaseEntities DatabaseEntities
        {
            get { return (DatabaseEntities) LtInfoEntityTypeLoader; }
        }

        private static DatabaseEntities MakeNewContext(bool autoDetectChangesEnabled)
        {
            var databaseEntities = new DatabaseEntities(Tenant.TenantID);
            databaseEntities.Configuration.AutoDetectChangesEnabled = autoDetectChangesEnabled;
            return databaseEntities;
        }

        public static void StartContextForTest()
        {
            var context = MakeNewContext(true);
            SetValue(TenantKey, Tenant.SitkaTechnologyGroup);
            SetValue(DatabaseContextKey, context);
        }

        public static void EndContextForTest()
        {
            if (!BackingStore.Contains(DatabaseContextKey))
            {
                return;
            }
            var databaseEntities = BackingStore[DatabaseContextKey] as DatabaseEntities;
            if (databaseEntities != null)
            {
                databaseEntities.Dispose();
                BackingStore[DatabaseContextKey] = null;
            }
            BackingStore.Remove(DatabaseContextKey);

            if (!BackingStore.Contains(TenantKey))
            {
                return;
            }
            var tenant = BackingStore[TenantKey] as Tenant;
            if (tenant != null)
            {
                BackingStore[TenantKey] = null;
            }
            BackingStore.Remove(TenantKey);

            if (!BackingStore.Contains(PersonKey))
            {
                return;
            }
            var person = BackingStore[PersonKey] as Person;
            if (person != null)
            {
                BackingStore[PersonKey] = null;
            }
            BackingStore.Remove(PersonKey);
        }
    }
}
