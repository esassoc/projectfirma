/*-----------------------------------------------------------------------
<copyright file="SitkaRequestInfo.cs" company="Environmental Science Associates">
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

namespace LtInfo.Common
{
    public class SitkaRequestInfo
    {
        public readonly Exception OriginalException;

        public readonly string SessionInfo;
        public readonly SitkaDebugInfo DebugInfo;
        public readonly Uri UrlReferrer;
        public readonly string UserAgent;
        public readonly string Method;
        public readonly int StatusCode;
        public readonly string StatusDescription;

        /// <summary>
        /// Real constructor to be used in production code
        /// </summary>
        public SitkaRequestInfo(Exception exception, HttpContext context)
        {
            OriginalException = exception;
            UrlReferrer = context.Request.UrlReferrer;
            UserAgent = context.Request.UserAgent;
            Method = context.Request.HttpMethod;
            StatusCode = context.Response.StatusCode;
            StatusDescription = context.Response.StatusDescription;

            DebugInfo = new SitkaDebugInfo(context.Request);
            SessionInfo = SitkaLogger.Instance.GetUserAndSessionInformationForErrorSafely(context);
        }

        /// <summary>
        /// Testing constructor to provide injection of values
        /// </summary>
        public SitkaRequestInfo(Exception exception, string sessionInfo, SitkaDebugInfo debugInfo, Uri referrer, string userAgent, string method)
        {
            OriginalException = exception;
            SessionInfo = sessionInfo;
            DebugInfo = debugInfo;
            UrlReferrer = referrer;
            UserAgent = userAgent;
            Method = method;
        }

        /// <summary>
        /// Testing constructor to provide injection of values
        /// </summary>
        public SitkaRequestInfo(Exception exception, string sessionInfo, SitkaDebugInfo debugInfo, Uri referrer, string userAgent)
            :this(exception, sessionInfo, debugInfo, referrer, userAgent, "GET")
        {
        }

        public override string ToString()
        {
            return string.Format("Unhandled Exception: {0}{1}{0}{0}{2}{0}{0}{3}{0}", Environment.NewLine, OriginalException, SessionInfo, DebugInfo );
        }
    }
    
    public class SitkaDebugInfo
    {
        public readonly IPAddress IpAddress;
        public readonly String Hostname;
        public readonly String WhoIsInfo;
        public readonly Uri Uri;
        public readonly String RequestInfo;

        public SitkaDebugInfo(HttpRequest request)
        {
            var requestContent = HttpDebugInfo.GetRequestContent(request);

            var hostnameByReverseDns = String.Empty;
            var whoisInfo = String.Empty;
            IPAddress userHostIpAddress = null;
            if (request.UserHostAddress != null)
            {
                userHostIpAddress = IPAddress.Parse(request.UserHostAddress);
                hostnameByReverseDns = DnsUtility.GetReverseDns(userHostIpAddress);
                whoisInfo = WhoisUtility.Lookup(userHostIpAddress);
            }

            IpAddress = userHostIpAddress;
            Hostname = hostnameByReverseDns ?? String.Empty;
            WhoIsInfo = whoisInfo ?? String.Empty;
            Uri = request.Url;
            RequestInfo = requestContent ?? String.Empty;
        }

        /// <summary>
        /// Testing constructor to provide injection of values
        /// </summary>
        public SitkaDebugInfo(IPAddress ip, String host, String whois, Uri uri, String requestInfo)
        {
            IpAddress = ip;
            Hostname = host ?? String.Empty;
            WhoIsInfo = whois ?? String.Empty;
            Uri = uri;
            RequestInfo = requestInfo ?? String.Empty;
        }

        public override string ToString()
        {
            return string.Format("IP Address: {1}{0}Hostname: {2}{0}Whois Info: {0}{3}{0}URL: {4}{0}{0}Http Request: {0}{5}", Environment.NewLine, IpAddress, Hostname, CommonUtility.IndentLinesInStringByAmount(WhoIsInfo, 12, " "), Uri, RequestInfo);
        }
    }
}
