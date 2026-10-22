/*-----------------------------------------------------------------------
<copyright file="NonActionableErrorClassifier.cs" company="Environmental Science Associates">
Copyright (c) Environmental Science Associates. All rights reserved.
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
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace LtInfo.Common
{
    /// <summary>
    /// Decides whether a request error is worth an App Alerts email (ERROR) or should only be written to the log file (WARN).
    /// Non-actionable errors are ones caused by the client (bad input, expired forms, permissions, broken links) or by crawlers, not by a problem with the site.
    /// </summary>
    public static class NonActionableErrorClassifier
    {
        private static readonly string[] CrawlerUserAgentFragments = { "bot", "crawler", "spider", "slurp" };

        public static bool ShouldLogAsWarning(SitkaRequestInfo requestInfo)
        {
            return IsFromCrawler(requestInfo.UserAgent) || IsNonActionableException(requestInfo.OriginalException);
        }

        public static bool IsFromCrawler(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return false;
            }
            var lowerUserAgent = userAgent.ToLowerInvariant();
            return CrawlerUserAgentFragments.Any(lowerUserAgent.Contains);
        }

        /// <summary>
        /// True if anywhere in the exception chain there is an exception caused by the client rather than the site
        /// </summary>
        public static bool IsNonActionableException(Exception exception)
        {
            for (var currentException = exception; currentException != null; currentException = currentException.InnerException)
            {
                if (currentException is HttpRequestValidationException ||   // "A potentially dangerous Request.Form value was detected"
                    currentException is HttpAntiForgeryException ||         // expired or tampered form token
                    currentException is ThreadAbortException ||             // Response.Redirect / Response.End
                    currentException is SitkaRecordNotAuthorizedException ||
                    currentException is SitkaRecordNotFoundException)
                {
                    return true;
                }

                var displayErrorWithHttpCode = currentException as SitkaDisplayErrorExceptionWithHttpCode;
                if (displayErrorWithHttpCode != null && (int)displayErrorWithHttpCode.HttpStatusCode >= 400 && (int)displayErrorWithHttpCode.HttpStatusCode < 500)
                {
                    return true;
                }

                var httpException = currentException as HttpException;
                if (httpException != null)
                {
                    var httpCode = httpException.GetHttpCode();
                    if (httpCode >= 400 && httpCode < 500)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
