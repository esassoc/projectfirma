using Hangfire.Dashboard;
using Microsoft.Owin;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Controllers;

namespace ProjectFirma.Web.ScheduledJobs
{
    public class HangfireFirmaWebAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // The Hangfire dashboard runs as OWIN middleware outside the MVC pipeline, so
            // HttpRequestStorage.FirmaSession is never populated here (it stays the anonymous
            // session, Person == null). Resolve the session straight from the authenticated OWIN
            // principal the same way the MVC pipeline does (ClaimsIdentityHelper), which branches
            // per auth mode (Auth0 / Keystone / Local). The old code used the Keystone-only helper
            // directly, so admins were 403'd under Auth0. Admin is still required.
            var owinContext = new OwinContext(context.GetOwinEnvironment());
            var firmaSession = ClaimsIdentityHelper.FirmaSessionFromClaimsIdentity(owinContext.Authentication, HttpRequestStorage.Tenant);
            return firmaSession?.IsAdministrator() ?? false;
        }
    }
}
