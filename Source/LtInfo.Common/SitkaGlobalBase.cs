/*-----------------------------------------------------------------------
<copyright file="SitkaGlobalBase.cs" company="Environmental Science Associates">
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using LtInfo.Common.LoggingFilters;
using LtInfo.Common.Views;

namespace LtInfo.Common
{
    public class ViewEngineLocations
    {
        public List<String> ViewLocations;
        public List<String> PartialViewLocations;
        public List<String> AreaViewLocations;
        public List<String> AreaPartialViewLocations;
    }

    public abstract class SitkaGlobalBase : SitkaHttpApplication
    {
        public const string CustomErrorMessageDivID = "customErrorMessageDiv";
        public const string CustomErrorMessageReplaceToken = "CUSTOM ERROR HERE";
        public const string CustomThrowErrorMessageForTesting = "This is a custom message that the user should be able to see!";
        public const string CustomNotAuthorizedErrorMessageForTesting = "This is a custom not authorized that the user should be able to see!";
        public const string CustomNotFoundMessageReplaceToken = "CUSTOM NOT FOUND HERE";
        public const string TestValidatedRequestFormField = "JunkField";

        public abstract string ErrorUrl { get; }
        public abstract string NotFoundUrl { get; }
        public abstract string ErrorHtml { get; }
        public abstract string NotFoundHtml { get; }

        public bool UseAreasInRoutes { get; set; }

        /// <summary>
        /// Pattern applied to <see cref="HttpRequest.Path"/> to set http headers to allow caching for static content. Adjust regex to match all that is considered cacheable content that is cacheable. Note <see cref="HttpRequest.Path"/> doesn't include the query string.
        /// </summary>
        private static readonly Regex CacheableContentRequestPathRegex = new Regex(@"^.*(asset\.axd)|(\.css)|(\.eot)|(\.gif)|(\.ico)|(\.jpg)|(\.js)|(\.pdf)|(\.png)|(\.svg)|(\.swf)|(\.ttf)|(\.woff)|(\.woff2)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected void ApplicationBeginRequest()
        {
        }

        /// <summary>
        /// List of <see cref="HttpStatusCode"/> settings that should be logged in case one slips by the <see cref="ApplicationError"/> trap
        /// </summary>
        private static readonly HashSet<HttpStatusCode> HttpStatusCodesThatShouldBeLogged = new HashSet<HttpStatusCode>
                                                                                             {
                                                                                                 HttpStatusCode.BadRequest,
                                                                                                 // 11/19/21 TK - removing unauthorized to reduce logging spam in App Alerts from the NCRP update to add login to access site.
                                                                                                 //HttpStatusCode.Unauthorized,
                                                                                                 HttpStatusCode.PaymentRequired,
                                                                                                 HttpStatusCode.Forbidden,
                                                                                                 HttpStatusCode.MethodNotAllowed,
                                                                                                 HttpStatusCode.NotAcceptable,
                                                                                                 HttpStatusCode.ProxyAuthenticationRequired,
                                                                                                 HttpStatusCode.RequestTimeout,
                                                                                                 HttpStatusCode.Conflict,
                                                                                                 HttpStatusCode.Gone,
                                                                                                 HttpStatusCode.LengthRequired,
                                                                                                 HttpStatusCode.PreconditionFailed,
                                                                                                 HttpStatusCode.RequestEntityTooLarge,
                                                                                                 HttpStatusCode.RequestUriTooLong,
                                                                                                 HttpStatusCode.UnsupportedMediaType,
                                                                                                 HttpStatusCode.RequestedRangeNotSatisfiable,
                                                                                                 HttpStatusCode.ExpectationFailed,
                                                                                                 HttpStatusCode.InternalServerError,
                                                                                                 HttpStatusCode.NotImplemented,
                                                                                                 HttpStatusCode.BadGateway,
                                                                                                 HttpStatusCode.ServiceUnavailable,
                                                                                                 HttpStatusCode.GatewayTimeout,
                                                                                                 HttpStatusCode.HttpVersionNotSupported
                                                                                             };

        /// <summary>
        /// Add appropriate HTTP caching headers to get browser to round trip to server when using browser back button
        /// ASP.NET pages get the headers.
        /// * Static file types (.css, .gif, .ico, .jpg., .js, etc.) get whatever is configured in the <param name="cacheTimeForStaticContent" />
        /// * Dynamic file types get non caching headers as below:
        /// 
        /// Cache-Control: private, proxy-revalidate
        /// Pragma: no-cache
        /// Expires: Mon, 01 Jan 1990 00:00:00 GMT
        /// </summary>
        protected static void AddCachingHeaders(HttpResponse response, HttpRequest request, TimeSpan cacheTimeForStaticContent)
        {
            AddCachingHeaders(response, request, cacheTimeForStaticContent, false);
        }

        /// <summary>
        /// Overload providing caller the ability to override the caching regardless of the request path
        /// </summary>
        protected static void AddCachingHeaders(HttpResponse response, HttpRequest request, TimeSpan cacheTimeForStaticContent, bool overrideCache)
        {
            // If it's a request that matches content we'd like to allow for caching...
            if (!overrideCache && CacheableContentRequestPathRegex.IsMatch(request.Path))
            {
                // Set up caching headers
                // ----------------------

                // Expires: [some time from now]
                response.Cache.SetExpires(DateTime.Now.Add(cacheTimeForStaticContent));

                // Cache-Control: public
                response.Cache.SetCacheability(HttpCacheability.Public);

                // Cache-Control: proxy-revalidate
                response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                return;
            }

            // Otherwise set up non-caching headers
            // ------------------------------------

            // Code below is trying to setup http headers like these (included here to capture search attempts for these headers):
            //
            // Cache-Control: private, proxy-revalidate
            // Pragma: no-cache
            // Expires: Mon, 01 Jan 1990 00:00:00 GMT

            // Cache-Control: private
            response.Cache.SetCacheability(HttpCacheability.Private);

            // Cache-Control: proxy-revalidate
            response.Cache.SetRevalidation(HttpCacheRevalidation.ProxyCaches);

            // Pragma: no-cache
            // for our HTTP 1.0 friends
            response.AppendHeader("Pragma", "no-cache");

            // Expires: Mon, 01 Jan 1990 00:00:00 GMT
            response.Cache.SetExpires(DateTime.Parse("01 Jan 1990 00:00:00 GMT"));
        }

        protected virtual void SetupCustomViewLocationsForTemplates(ViewEngineLocations viewEngineLocations, Dictionary<string, string> areasDictionary)
        {
            ViewEngines.Engines.Clear();

            if (viewEngineLocations != null)
            {
                var appSpecificEngine = new RazorViewEngine();

                if (viewEngineLocations.AreaViewLocations != null && viewEngineLocations.AreaViewLocations.Count > 0)
                {
                    appSpecificEngine.AreaViewLocationFormats = viewEngineLocations.AreaViewLocations.ToArray();
                }

                if (viewEngineLocations.AreaPartialViewLocations != null && viewEngineLocations.AreaPartialViewLocations.Count > 0)
                {
                    appSpecificEngine.AreaPartialViewLocationFormats = viewEngineLocations.AreaPartialViewLocations.ToArray();
                }

                if (viewEngineLocations.ViewLocations != null && viewEngineLocations.ViewLocations.Count > 0)
                {
                    appSpecificEngine.ViewLocationFormats = viewEngineLocations.ViewLocations.ToArray();
                }

                if (viewEngineLocations.PartialViewLocations != null && viewEngineLocations.PartialViewLocations.Count > 0)
                {
                    appSpecificEngine.PartialViewLocationFormats = viewEngineLocations.PartialViewLocations.ToArray();
                }

                // add the application specific formats first based on the policy that more specific matches should be found before general matches
                ViewEngines.Engines.Add(appSpecificEngine);
            }

            var extraPartialViewLoactions = new List<string> { "~/Views/{1}/{0}.cshtml", "~/Views/Shared/{0}.cshtml", "~/Views/SitkaCommon/{0}.cshtml" };
            if (areasDictionary.Any())
            {
                extraPartialViewLoactions.AddRange(areasDictionary.Keys.SelectMany(x => new List<string> {string.Format("~/Areas/{0}/Views/{{1}}/{{0}}.cshtml", x), string.Format("~/Areas/{0}/Views/Shared/{{0}}.cshtml", x)}));
            }

            var globalLocations = new RazorViewEngine
            {// In addition to the usual Root and "Shared" location, we add a "SitkaCommon" directory. This is where we put views coming from Sitka Common.
                PartialViewLocationFormats = extraPartialViewLoactions.ToArray()
            };

            ViewEngines.Engines.Add(globalLocations);
        }

        private ReadOnlyCollection<ISitkaLoggingFilter> _loggingFilters;
        private IEnumerable<ISitkaLoggingFilter> LoggingFilters => _loggingFilters ?? (_loggingFilters = new ReadOnlyCollection<ISitkaLoggingFilter>(GetLoggingFilters()));

        private bool IsRequestExemptedFromLogging(SitkaRequestInfo requestInfo)
        {
            return LoggingFilters.Any(sitkaLoggingFilter => sitkaLoggingFilter.ShouldRequestBeFiltered(requestInfo));
        }

        protected virtual List<ISitkaLoggingFilter> GetLoggingFilters()
        {
            var filters = new List<ISitkaLoggingFilter> { new TrendMicroFilter(), new NotReferred404LoggingFilter(), new NagiosLoggingFilter(), new RequireSslLoggingFilter(), new UnsupportedHttpMethodHandler(), new RemoteHostClosedTheConnectionLoggingFilter(), new SemRushFilter() };
            return filters;
        }

        protected void ApplicationError()
        {
            string errorPageHtml;
            var httpStatusCode = (int)HttpStatusCode.InternalServerError;
            var lastStoredError = Server.GetLastError();
            var requestInfo = new SitkaRequestInfo(lastStoredError, HttpContext.Current);

            try
            {
                if (!IsRequestExemptedFromLogging(requestInfo))
                {
                    SitkaLogger.Instance.LogDetailedErrorMessage(requestInfo);
                }

                if (lastStoredError is HttpException)
                {
                    httpStatusCode = ((HttpException)lastStoredError).GetHttpCode();
                }

                if (lastStoredError is SitkaDisplayErrorExceptionWithHttpCode)
                {
                    httpStatusCode = (int)((SitkaDisplayErrorExceptionWithHttpCode)lastStoredError).HttpStatusCode;
                }

                if (httpStatusCode == (int)HttpStatusCode.NotFound || SitkaHttpRequestStorage.NotFoundStoredError != null)
                {
                    return; // we know that applicationend will spit out the not found error page to the client.
                }

                errorPageHtml = GetErrorPageHtml(Request.IsLocal, Request.Cookies.ToCookieCollection(Request.Url.Host), lastStoredError);
            }
            catch (Exception ex)
            {
                // Catch and log anything that goes wrong in this error handler, we don't let it escape so that it doesn't start the error handling over again
                SitkaLogger.Instance.LogDetailedErrorMessage(new Exception("Secondary exception occurred while trying to display error message.", ex));
                errorPageHtml = WrapExceptionDetailsInHtml(String.Format("Secondary exception occurred while trying to display error message, original error message lost.\r\nSecondary Exception Details:\r\n{0}", ex));
            }

            SendErrorResponseToBrowser(errorPageHtml, httpStatusCode);
            Server.ClearError();
        }

        public static string GetMissingPageHtml(CookieCollection cookies, string notFoundHtml)
        {
            // DO NOT pass the "ASP.NET_SessionId" cookie. It forces this request to block on obtaining write access to the user session, which our parent request currently holds. 
            var allCookiesExceptSessionCookie = CookieHelper.GetAllCookiesExceptSessionCookie(cookies);
            return HttpHelper.GetUrl(Instance.NotFoundUrl, allCookiesExceptSessionCookie).Replace(CustomNotFoundMessageReplaceToken, notFoundHtml);
        }

        private void SendErrorResponseToBrowser(string innerError, int httpStatusCode)
        {
            Response.Clear();
            if (Response.StatusCode != httpStatusCode)
            {
                Response.StatusCode = httpStatusCode;
            }
            Response.Write(innerError);
        }

        public string GetErrorPageHtml(bool isLocal, CookieCollection cookies, Exception getLastError)
        {
            try
            {
                var errorMessage = Instance.ErrorHtml;
                if (getLastError != null)
                {
                    var sitkaException = GetSitkaDisplayableExceptionIfAny(getLastError);
                    if (sitkaException != null)
                    {
                        errorMessage = sitkaException.Message;
                        if (SitkaHttpRequestStorage.ShouldHandleCustomErrors)
                        {
                            errorMessage = errorMessage.Replace("\n", "<br/>");
                        }
                    }
                    else if (isLocal)
                    {
                        errorMessage = string.Format("{0}\r\n{1}", errorMessage, WrapExceptionDetailsInHtml(getLastError.ToString()));
                    }
                }

                if (SitkaHttpRequestStorage.ShouldHandleCustomErrors && !IsThisHttpRequestForTheErrorTemplatePage())
                {
                    // DO NOT pass the "ASP.NET_SessionId" cookie. It forces this request to block on obtaining write access to the user session, which our parent request currently holds. 
                    var allCookiesExceptSessionCookie = CookieHelper.GetAllCookiesExceptSessionCookie(cookies);
                    var errorPage = HttpHelper.GetUrl(Instance.ErrorUrl, allCookiesExceptSessionCookie);
                    return errorPage.Replace(CustomErrorMessageReplaceToken, errorMessage);
                }

                return errorMessage;
            }
            catch (Exception ex)
            {
                // Catch and log anything that goes wrong in this error handler, we don't let it escape so that it doesn't start the error handling over again.
                // If the original error was client-caused (WARN), failing to template its error page isn't email-worthy either
                if (NonActionableErrorClassifier.IsNonActionableException(getLastError))
                {
                    SitkaLogger.Instance.LogDetailedWarningMessage(ex);
                }
                else
                {
                    SitkaLogger.Instance.LogDetailedErrorMessage(ex);
                }
                return WrapExceptionDetailsInHtml(String.Format("Secondary exception occurred while trying to display error message, original error message lost.\r\nSecondary Exception Details:\r\n{0}", ex));
            }
        }

        private bool IsThisHttpRequestForTheErrorTemplatePage()
        {
            // Sometimes we get urls like: /Error/Error.aspx?500;http://example.com/Home.mvc/Error
            // That means the error template page got a secondary error, so we don't want to continue trying to load the error template page
            var errorUri = new UriBuilder(Instance.ErrorUrl);
            var errorPath = errorUri.Path;
            if (Request.Url.PathAndQuery.Contains(errorPath, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            var errorUrlAbsolutePath = GeneralUtility.UrlToAbsolutePath(Instance.ErrorUrl);
            var potentialErrorUrlPaths = new List<string> { errorUrlAbsolutePath };
            var requestAbsolutePath = Request.Url.AbsolutePath;
            return potentialErrorUrlPaths.Contains(requestAbsolutePath, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Wraps text into an html &lt;pre&gt; tag, including CSS formatting for good wrapping and does the Html Encoding also
        /// </summary>
        private static string WrapExceptionDetailsInHtml(string text)
        {
            return String.Format("<pre class=\"stackTrace\" style=\"white-space:pre-wrap;background-color:lightYellow;color:darkRed;padding:10px\">\r\n{0}\r\n</pre>", text.HtmlEncode());
        }

        private static SitkaDisplayErrorException GetSitkaDisplayableExceptionIfAny(Exception lastError)
        {
            if (lastError is SitkaDisplayErrorException)
            {
                return (SitkaDisplayErrorException)lastError;
            }

            if (lastError.InnerException == null)
            {
                return null;
            }

            return GetSitkaDisplayableExceptionIfAny(lastError.InnerException);
        }

        public static SitkaGlobalBase Instance
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return (SitkaGlobalBase)HttpContext.Current.ApplicationInstance;
                }

                throw new NullReferenceException("Cannot call SitkaGlobalBase.Instance when not under an Http Context!");
            }
        }

        public static ActionResult RequestValidationTestUrlImpl(HttpRequestBase request)
        {
            return new ContentResult { Content = request.Form[TestValidatedRequestFormField].HtmlEncode() };
        }

        /// <summary>
        /// HttpRequests that have the SOAPAction HTTP Header Field are web service calls.
        /// From the SOAP standard, <see cref="http://www.w3.org/TR/2000/NOTE-SOAP-20000508/#_Toc478383528" />, "An HTTP client MUST use this header field when issuing a SOAP HTTP Request."
        /// </summary>
        private static bool IsWebServiceHttpRequest(HttpRequest request)
        {
            return request.Headers.AllKeys.Any(x => String.Equals(x, "SOAPAction"));
        }

        protected void ApplicationEndRequest()
        {
            var responseStatusCode = (HttpStatusCode)Response.StatusCode;
            var lastError = Server.GetLastError();

            // Decide about logging the error or not
            // -------------------------------------
            if (HttpStatusCodesThatShouldBeLogged.Contains(responseStatusCode) && SitkaHttpRequestStorage.ShouldLogErrorFromApplicationEnd)
            {
                var requestInfo = new SitkaRequestInfo(lastError, HttpContext.Current);
                if (!IsRequestExemptedFromLogging(requestInfo))
                {
                    if (lastError == null && IsWebServiceHttpRequest(Request))
                    {
                        lastError = SitkaHttpRequestStorage.WcfStoredError;
                    }
                    // One log entry per request, lastError.ToString() already includes the inner exception chain.
                    // Only 5xx responses are App Alerts email worthy (ERROR), 4xx are client problems and only go to the log file (WARN)
                    var message = string.Format("Http Server is returning response status code \"{0}\" and error was not logged via the Application_Error method", Response.Status);
                    if ((int)responseStatusCode < 500 || NonActionableErrorClassifier.ShouldLogAsWarning(requestInfo))
                    {
                        SitkaLogger.Instance.LogDetailedWarningMessage(message, lastError, Context);
                    }
                    else
                    {
                        SitkaLogger.Instance.LogDetailedErrorMessage(message, lastError, Context);
                    }
                }
            }

            // Send the NotFound page as needed
            // --------------------------------
            if (responseStatusCode == HttpStatusCode.NotFound || SitkaHttpRequestStorage.NotFoundStoredError != null)
            {
                var html = Instance.NotFoundHtml;
                if (SitkaHttpRequestStorage.NotFoundStoredError != null)
                {
                    html += string.Format("<p>{0}</p>", SitkaHttpRequestStorage.NotFoundStoredError.Message);
                    SitkaHttpRequestStorage.NotFoundStoredError = null;
                }
                string errorHtml;
                try
                {
                    errorHtml = GetMissingPageHtml(Request.Cookies.ToCookieCollection(Request.Url.Host), html);
                }
                catch (Exception ex)
                {
                    // The templated Not Found page couldn't be fetched (e.g. a disabled tenant answers 410), send the plain one rather than a 500.
                    // WARN not ERROR: the original request was only a 404
                    SitkaLogger.Instance.LogDetailedWarningMessage("Could not fetch the templated Not Found page, sent the plain one instead", ex, Context);
                    errorHtml = html;
                }
                SendErrorResponseToBrowser(errorHtml, (int)HttpStatusCode.NotFound);
            }

            // Release any resources needed only for this one http request, particularly any EntityFramework contexts or other IDisposable items
            SitkaHttpRequestStorage.DisposeItemsAndClearStore();
        }

        /// <summary>
        /// If your page is going to set a weird error status and you don't want it to log automatically, call this function
        /// </summary>
        public static void CancelErrorLoggingFromApplicationEnd()
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                SitkaHttpRequestStorage.ShouldLogErrorFromApplicationEnd = false;
            }
        }

        /// <summary>
        /// If your page is going to set a weird error status and you don't want it to log automatically, call this function
        /// </summary>
        public static void CancelCustomErrorHandling()
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                SitkaHttpRequestStorage.ShouldHandleCustomErrors = false;
            }
        }

        public string GetErrorContentWithoutThrowing(string message)
        {
            var errorException = new SitkaDisplayErrorException(message);
            var errorHtml = GetErrorPageHtml(Request.IsLocal, Request.Cookies.ToCookieCollection(Request.Url.Host), errorException);
            return errorHtml;
        }
    }
}
