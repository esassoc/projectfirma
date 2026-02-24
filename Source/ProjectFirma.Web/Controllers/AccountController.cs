/*-----------------------------------------------------------------------
<copyright file="AccountController.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using LtInfo.Common.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Security;
using ProjectFirma.Web.Security.Shared;
using ProjectFirmaModels.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ProjectFirma.Web.Controllers
{
    public class AccountController : SitkaController
    {
        protected string HomeUrl
        {
            // Fully-qualified due to ambiguous reference.
            get { return SitkaRoute<HomeController>.BuildAbsoluteUrlHttpsFromExpression(c => c.Index()); }
        }

        protected override bool IsCurrentUserAnonymous()
        {
            return HttpRequestStorage.FirmaSession.IsAnonymousUser();
        }

        protected override string LoginUrl
        {
            get
            {
                return FirmaHelpers.GenerateAbsoluteLogInUrl();
            } 
        }

        [AllowAnonymous]
        public ActionResult BeginLogin()
        {
            var referrer = Request.UrlReferrer?.AbsoluteUri;
            var safeReturnUrl = FirmaHelpers.ValidateReturnUrl(referrer);

            if (!string.IsNullOrEmpty(safeReturnUrl))
            {
                Response.Cookies.Add(new HttpCookie("ReturnURL", safeReturnUrl)
                {
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddMinutes(30),
                });
            }

            var returnFromSitkaUrl = SitkaRoute<AccountController>.BuildAbsoluteUrlHttpsFromExpression(c => c.LogOn(""));

            var initiateFromTenantDomainUrl = "https://" + FirmaWebConfiguration.DefaultTenantCanonicalHostName;
            
            var sitka = initiateFromTenantDomainUrl + "/Account/LogOn"
                                                    + "?returnTo=" + HttpUtility.UrlEncode(returnFromSitkaUrl);

            return Redirect(sitka);
        }
        
        [LoggedInUnclassifiedFeature]
        [CrossAreaRoute]
        public ActionResult LogOn(string returnTo)
        {
            SitkaHttpApplication.Logger.Info($"AccountController - LogOn() - AuthType:{FirmaWebConfiguration.AuthenticationType}PersonID:{HttpRequestStorage.FirmaSession.PersonID}, email?:{HttpRequestStorage.FirmaSession.Person?.Email}");
            //look up cookie and return to url we were previously on, otherwise homepage.
            if (!string.IsNullOrWhiteSpace(returnTo))
            {
                var validatedReturnUrl = FirmaHelpers.ValidateReturnUrl(returnTo);
                if (!string.IsNullOrWhiteSpace(validatedReturnUrl))
                {
                    return Redirect(validatedReturnUrl);
                }
            }

            var returnUrl = Request.Cookies["ReturnURL"];
            if (!string.IsNullOrWhiteSpace(returnUrl?.Value))
            {
                returnUrl.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(returnUrl);

                var returnUrlVal = HttpUtility.UrlDecode(returnUrl.Value);
                var validatedReturnUrl = FirmaHelpers.ValidateReturnUrl(returnUrlVal);
                if (!string.IsNullOrEmpty(validatedReturnUrl))
                {
                    return Redirect(validatedReturnUrl);
                }
            }

            // placeholder route - since url is secured, authorize filter should redirect to SSO flow (since no EM.Common.Keystone.AllowAnonymous attribute)
            return Redirect(HomeUrl);
        }

        [AnonymousUnclassifiedFeature]
        public ActionResult LogOff()
        {
            if (HttpRequestStorage.FirmaSession.IsAnonymousUser())
            {
                return Redirect("/");
            }

            // If we are impersonating, we drop back to the original user instead of fully logging out.
            var currentFirmaSession = HttpRequestStorage.FirmaSession;
            SitkaHttpApplication.Logger.Info($"AccountController - LogOff() - AuthType:{FirmaWebConfiguration.AuthenticationType}, PersonID:{currentFirmaSession.PersonID}, email?:{currentFirmaSession.Person?.Email}");
            if (currentFirmaSession.IsImpersonating())
            {
                var previousPageUri = Request.UrlReferrer;
                currentFirmaSession.ResumeOriginalUser(previousPageUri, out string statusMessage);
                HttpRequestStorage.DatabaseEntities.SaveChangesWithNoAuditing(currentFirmaSession.Person.TenantID);
                //TaurusSecurityLogger.Info(this.TaurusSession, statusMessage);
                SetInfoForDisplay(statusMessage);
                return Redirect("/");
            }

            // Otherwise, we just log off normally
            switch (FirmaWebConfiguration.AuthenticationType)
            {
                case AuthenticationType.KeystoneAuth:
                    Request.GetOwinContext().Authentication.SignOut();
                    break;
                case AuthenticationType.Auth0Auth:
                    Request.GetOwinContext().Authentication.SignOut(
                        CookieAuthenticationDefaults.AuthenticationType
                    );
                    break;
                case AuthenticationType.LocalAuth:
                    Request.GetOwinContext().Authentication.SignOut();
                    Request.GetOwinContext().Authentication.SignOut(FirmaOwinStartup.CookieAuthenticationType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var currentPerson = HttpRequestStorage.Person;
            currentFirmaSession.Delete(HttpRequestStorage.DatabaseEntities);
            HttpRequestStorage.DatabaseEntities.SaveChanges(currentPerson);

            if (FirmaWebConfiguration.AuthenticationType == AuthenticationType.Auth0Auth)
            {
                var returnUrl = SitkaRoute<AccountController>.BuildAbsoluteUrlHttpsFromExpression(c => c.LogOffAuth0());
                SitkaHttpApplication.Logger.Info($"AccountController - LogOff() - returnUrl: {returnUrl}");
                var sitkaLogout = "https://" + FirmaWebConfiguration.DefaultTenantCanonicalHostName + "/Account/LogOffAndReturn"
                                  + "?returnURL=" + HttpUtility.UrlEncode(returnUrl);
                SitkaHttpApplication.Logger.Info($"AccountController - LogOff() - sitkaLogout: {sitkaLogout}");
                return Redirect(sitkaLogout);
            }
            return Redirect("/");
        }

        [AllowAnonymous]
        public ActionResult LogOffAuth0()
        {
            SitkaHttpApplication.Logger.Info($"AccountController - LogOffAuth0() - Request.Url: {Request?.Url} ");
            // Clear local cookie
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }


        [AllowAnonymous]
        public ActionResult LogOffAndReturn(string returnURL)
        {
            // Clear local cookie
            Request.GetOwinContext().Authentication.SignOut(
                CookieAuthenticationDefaults.AuthenticationType
            );
            SitkaHttpApplication.Logger.Info($"AccountController - LogOffAndReturn() - returnURL: {returnURL}");
            SitkaHttpApplication.Logger.Info($"AccountController - LogOffAndReturn() - FirmaHelpers.ValidateReturnUrl(returnURL): {FirmaHelpers.ValidateReturnUrl(returnURL)}");
            var safeReturn = FirmaHelpers.ValidateReturnUrl(returnURL)
                             ?? "https://" + FirmaWebConfiguration.DefaultTenantCanonicalHostName;

            return Redirect(safeReturn);
        }

        [AnonymousUnclassifiedFeature]
        public ActionResult SignoutCleanup(string sid)
        {
            SitkaHttpApplication.Logger.Info($"AccountController - SignoutCleanup() - AuthType:{FirmaWebConfiguration.AuthenticationType}");
            var cp = (ClaimsPrincipal)HttpContext.User;
            var sidClaim = cp.FindFirst("sid");
            if (sidClaim != null && sidClaim.Value == sid)
            {
                Request.GetOwinContext().Authentication.SignOut(FirmaOwinStartup.CookieAuthenticationType);
            }

            return Content(string.Empty);
        }

        protected override ISitkaDbContext SitkaDbContext => HttpRequestStorage.DatabaseEntities;

        // also need to decide how to handle account verification status and also implement hash on db row to avoid unnecessary updates
        [AnonymousUnclassifiedFeature]
        public ContentResult NotAuthorized()
        {
            return Content("Not Authorized");
        }
    }
}
