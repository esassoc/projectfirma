/*-----------------------------------------------------------------------
<copyright file="FirmaHelpers.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using ProjectFirma.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace ProjectFirma.Web.Common
{
    public static class FirmaHelpers
    {
        public static readonly List<string> DefaultColorRange = new List<string>
        {
            "#1f77b4",
            "#ff7f0e",
            "#aec7e8",
            "#ffbb78",
            "#2ca02c",
            "#98df8a",
            "#d62728",
            "#ff9896",
            "#9467bd",
            "#c5b0d5",
            "#022c99",
            "#507e3c",
            "#0d5875",
            "#37aba8",
            "#44cc44",
            "#afa5f2",
            "#d3ffce",
            "#070a41"
        };

        public static string GenerateLogInUrl()
        {
            var logInUrl = string.Empty;
            switch (FirmaWebConfiguration.AuthenticationType)
            {
                case AuthenticationType.KeystoneAuth: 
                    logInUrl = SitkaRoute<AccountController>.BuildUrlFromExpression(c => c.LogOn(""));
                    break;
                case AuthenticationType.Auth0Auth:
                    logInUrl = SitkaRoute<AccountController>.BuildUrlFromExpression(c => c.BeginLogin());
                    break;
                case AuthenticationType.LocalAuth:
                    logInUrl = SitkaRoute<LocalAuthenticationController>.BuildUrlFromExpression(c => c.LocalAuthLogon());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return logInUrl;
        }

        public static string GenerateAbsoluteLogInUrl()
        {
            var logInUrl = string.Empty;
            switch (FirmaWebConfiguration.AuthenticationType)
            {
                case AuthenticationType.KeystoneAuth:
                    logInUrl = SitkaRoute<AccountController>.BuildAbsoluteUrlHttpsFromExpression(c => c.LogOn(""));
                    break;
                case AuthenticationType.Auth0Auth:
                    logInUrl = SitkaRoute<AccountController>.BuildAbsoluteUrlHttpsFromExpression(c => c.BeginLogin());
                    break;
                case AuthenticationType.LocalAuth:
                    logInUrl = SitkaRoute<LocalAuthenticationController>.BuildAbsoluteUrlHttpsFromExpression(c => c.LocalAuthLogon());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return logInUrl;
        }

        public static string GenerateLogOutUrlWithReturnUrl()
        {
            var logOutUrl = SitkaRoute<AccountController>.BuildUrlFromExpression(c => c.LogOff());
            
            var returnUrl = HttpContext.Current.Request.Url.AbsoluteUri;

            return OnErrorOrNotFoundPage(returnUrl) ? logOutUrl : String.Format("{0}?returnUrl={1}", logOutUrl, HttpUtility.UrlEncode(returnUrl));
        }

        public static string GenerateCreateAccountWithReturnUrl(string returnUrl)
        {
            var createAccountUrl = SitkaConfiguration.GetRequiredAppSetting("KeystoneRegisterUrl");
            return $"{createAccountUrl}?RedirectUrl={HttpUtility.UrlEncode(returnUrl)}";
        }

        public static string GenerateForgotPasswordUrlWithReturnUrl(string returnUrl)
        {
            var forgotPasswordUrl = SitkaConfiguration.GetRequiredAppSetting("KeystoneForgotPasswordUrl");
            return $"{forgotPasswordUrl}?RedirectUrl={HttpUtility.UrlEncode(returnUrl)}";
        }

        private static bool OnErrorOrNotFoundPage(string url)
        {
            var returnUrlPathAndQuery = new Uri(url).PathAndQuery;
            var notFoundUrlPathAndQuery = new Uri(SitkaRoute<HomeController>.BuildAbsoluteUrlHttpsFromExpression(x => x.NotFound())).PathAndQuery;
            var errorUrlPathAndQuery = new Uri(SitkaRoute<HomeController>.BuildAbsoluteUrlHttpsFromExpression(x => x.Error())).PathAndQuery;

            var onErrorOrNotFoundPage = returnUrlPathAndQuery.StartsWith(notFoundUrlPathAndQuery, StringComparison.InvariantCultureIgnoreCase) ||
                                        returnUrlPathAndQuery.StartsWith(errorUrlPathAndQuery, StringComparison.InvariantCultureIgnoreCase);
            return onErrorOrNotFoundPage;
        }

        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        public static Color ChangeColorBrightness(this Color color, float correctionFactor)
        {
            var red = (float)color.R;
            var green = (float)color.G;
            var blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        public static List<string> CalculateNumberRanges(List<int> years)
        {
            var yearsInOrder = years.OrderBy(x => x).ToList();
            if (!yearsInOrder.Any())
            {
                return new List<string>();
            }

            return yearsInOrder
                .Select((n, i) => new {number = n, group = n - i})
                .GroupBy(n => n.@group)
                .Select(g =>
                    g.Count() >= 2
                        ? g.First().number + "-" + g.Last().number
                        : String.Join(", ", g.Select(x => x.number))
                )
                .ToList();
        }
        public static List<string> CalculateYearRanges(IEnumerable<int> years)
        {
            var yearsInOrder = years.Distinct().OrderBy(x => x).ToList();
            if (!yearsInOrder.Any())
            {
                return new List<string>();
            }

            return yearsInOrder
                .Select((n, i) => new {number = n, group = n - i})
                .GroupBy(n => n.@group)
                .Select(g =>
                    g.Count() >= 2
                        ? MultiTenantHelpers.FormatReportingYear(g.First().number) + "-" + MultiTenantHelpers.FormatReportingYear(g.Last().number)
                        : String.Join(", ", g.Select(x => MultiTenantHelpers.FormatReportingYear(x.number)))
                )
                .ToList();
        }

        public static string ValidateReturnUrl(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return null;

            // Must be absolute
            if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri))
                return null;

            // Scheme rules:
            // - https always allowed
            // - http allowed only for localhost/dev
            bool schemeOk =
                uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
                (uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                 && uri.Host.Contains("localhost", StringComparison.OrdinalIgnoreCase));

            if (!schemeOk)
                return null;

            // Host must be a known tenant
            if (!IsAllowedTenantHost(uri.Host))
                return null;

            // Normalize: return absolute URL (scheme/host/path + query), strip fragment
            return uri.GetLeftPart(UriPartial.Path) + uri.Query;
        }

        private static bool IsAllowedTenantHost(string host)
        {
            // Local dev
            if (host.EndsWith(".localhost.projectfirma.com", StringComparison.OrdinalIgnoreCase))
                return true;

            // ProjectFirma-managed domains
            if (host.EndsWith(".projectfirma.com", StringComparison.OrdinalIgnoreCase))
                return true;

            return FirmaWebConfiguration.CanonicalHostNameSet.Contains(host);
        }
    }
}
