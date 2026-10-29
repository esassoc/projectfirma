/*-----------------------------------------------------------------------
<copyright file="SitkaLogger.cs" company="Environmental Science Associates">
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
using System.Net;
using System.Web;
using log4net;
using LtInfo.Common.DesignByContract;

namespace LtInfo.Common
{
    public class SitkaLogger
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(SitkaLogger));
        private static SitkaLogger _logger;

        protected SitkaLogger()
        {
        }

        public static void RegisterLogger()
        {
            _logger = new SitkaLogger();
        }

        public static void RegisterLogger(SitkaLogger logger)
        {
            _logger = logger;
        }

        public static SitkaLogger Instance
        {
            get
            {
                Check.RequireNotNull(_logger, "You must call SitkaLogger.RegisterLogger to use the Sitka Logger");
                return _logger;
            }
        }

        public void LogDetailedErrorMessage(Exception exception)
        {
            var context = HttpContext.Current;
            LogDetailedErrorMessage("Unhandled Exception", exception, context);
        }

        public void LogDetailedErrorMessage(string additionalMessage, Exception exception)
        {
            var context = HttpContext.Current;
            LogDetailedErrorMessage(additionalMessage, exception, context);
        }

        /// <summary>
        /// The exception is passed to log4net as an object (not pasted into the message) so App Alerts can group by exception type and message and show the type in the subject
        /// </summary>
        public void LogDetailedErrorMessage(string additionalMessage, Exception exception, HttpContext context)
        {
            Logger.Error(BuildDetailedMessage(additionalMessage, context), exception);
            SitkaGlobalBase.CancelErrorLoggingFromApplicationEnd();
        }

        public void LogDetailedErrorMessage(string additionalMessage)
        {
            var context = HttpContext.Current;
            LogDetailedErrorMessage(additionalMessage, context);
        }

        public void LogDetailedErrorMessage(string additionalMessage, HttpContext context)
        {
            Logger.Error(BuildDetailedMessage(additionalMessage, context));
            SitkaGlobalBase.CancelErrorLoggingFromApplicationEnd();
        }

        private string BuildDetailedMessage(string additionalMessage, HttpContext context)
        {
            var sessionDebugInfo = context != null ? GetUserAndSessionInformationForErrorSafely(context) : string.Empty;
            var requestDebugInfo = context != null ? DebugInfo(context.Request) : string.Empty;
            return string.Format("{1}{0}{0}{2}{0}{0}{3}", Environment.NewLine, additionalMessage, sessionDebugInfo, requestDebugInfo);
        }
        
        public void LogDetailedErrorMessage(SitkaRequestInfo requestInfo)
        {
            // Errors that aren't actionable (bad client input, bots, client disconnects, 4xx) go to WARN so they stay in the log file but don't generate an App Alerts email
            if (NonActionableErrorClassifier.ShouldLogAsWarning(requestInfo))
            {
                Logger.Warn(requestInfo.ToString(), requestInfo.OriginalException);
            }
            else
            {
                Logger.Error(requestInfo.ToString(), requestInfo.OriginalException);
            }
            SitkaGlobalBase.CancelErrorLoggingFromApplicationEnd();
        }

        public void LogDetailedWarningMessage(Exception exception)
        {
            var context = HttpContext.Current;
            LogDetailedWarningMessage("Handled Exception", exception, context);
        }

        public void LogDetailedWarningMessage(string additionalMessage, Exception exception, HttpContext context)
        {
            Logger.Warn(BuildDetailedMessage(additionalMessage, context), exception);
            SitkaGlobalBase.CancelErrorLoggingFromApplicationEnd();
        }

        public void LogDetailedWarningMessage(string additionalMessage)
        {
            var context = HttpContext.Current;
            LogDetailedWarningMessage(additionalMessage, context);
        }

        /// <summary>
        /// Same detail as <see cref="LogDetailedErrorMessage(string, HttpContext)"/> but at WARN, which is below the App Alerts email threshold
        /// </summary>
        public void LogDetailedWarningMessage(string additionalMessage, HttpContext context)
        {
            Logger.Warn(BuildDetailedMessage(additionalMessage, context));
            SitkaGlobalBase.CancelErrorLoggingFromApplicationEnd();
        }

        /// <summary>
        /// Log a detailed, info-only message 
        /// </summary>
        public void LogDetailedInfoMessage(string infoMessage, SitkaDebugInfo debugInfo)
        {
            var s = $"{infoMessage}\r\n{debugInfo}";
            Logger.Info(s);
        }

        public static string DebugInfo(HttpRequest request)
        {
            var requestContent = HttpDebugInfo.GetRequestContent(request);

            var orgContentLength = requestContent.Length;
            var maxContentLength = SitkaWebConfiguration.DebugInfoMaxLength ?? orgContentLength;
            var newContentLength = Math.Min(maxContentLength, orgContentLength);

            requestContent = requestContent.Substring(0, newContentLength);

            var requestTruncated = orgContentLength > newContentLength;
            var truncationWarning = requestTruncated ? string.Format("{0}... Debug Info truncated to {1} characters", Environment.NewLine, newContentLength) : string.Empty;

            var rdns = String.Empty;
            var whoisInfo = String.Empty;
            if (request.UserHostAddress != null)
            {
                var userHostIpAddress = IPAddress.Parse(request.UserHostAddress);
                rdns = DnsUtility.GetReverseDns(userHostIpAddress);
                whoisInfo = CommonUtility.IndentLinesInStringByAmount(WhoisUtility.Lookup(userHostIpAddress), 12, " ");
            }
            return String.Format("IP Address: {1}{0}Hostname: {2}{0}Whois Info: {0}{3}{0}URL: {4}{0}{0}Begin Http Request:-->{0}{5}{6}{0}<--End Http Request", Environment.NewLine, request.UserHostAddress, rdns, whoisInfo, request.Url.AbsoluteUri, requestContent, truncationWarning);
        }

        /// <summary>
        /// Logging an error must never fail because gathering the user details failed (e.g. database down, disabled tenant), otherwise the error and its App Alerts email are silently lost
        /// </summary>
        public string GetUserAndSessionInformationForErrorSafely(HttpContext context)
        {
            try
            {
                return GetUserAndSessionInformationForError(context);
            }
            catch (Exception ex)
            {
                return $"(Could not get user and session information: {ex.GetType().Name}: {ex.Message})";
            }
        }

        /// <summary>
        /// Projects (MM, Taurus, EE, etc.) should implement this for themselves to pick up logging of User & Session information
        /// </summary>
        public virtual string GetUserAndSessionInformationForError(HttpContext context)
        {
            var username = string.Empty;
            if (context != null && context.User != null && context.User.Identity != null)
            {
                username = context.User.Identity.Name;
            }
            return $"Username: {username}";
        }
    }
}
