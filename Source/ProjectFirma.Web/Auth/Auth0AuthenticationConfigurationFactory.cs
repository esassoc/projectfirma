using LtInfo.Common;
using LtInfo.Common.Email;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Controllers;
using ProjectFirma.Web.Models;
using ProjectFirmaModels;
using ProjectFirmaModels.Models;
using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using SameSiteMode = Microsoft.Owin.SameSiteMode;

namespace ProjectFirma.Web.Auth
{
  
    public class Auth0AuthenticationConfigurationFactory
    {
        public CookieAuthenticationOptions CreateAuth0CookieAuthenticationOptions()
        {
            return new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString("/LogOn"),

                // Configure SameSite as needed for your app. Lax works well for most scenarios here but
                // you may want to set SameSiteMode.None for HTTPS
                CookieSameSite = SameSiteMode.None,

                // More information on why the CookieManager needs to be set can be found here: 
                // https://github.com/aspnet/AspNetKatana/wiki/System.Web-response-cookie-integration-issues
                CookieManager = new SameSiteCookieManager(new SystemWebCookieManager())
            };
        }

        public OpenIdConnectAuthenticationOptions CreateAuth0OpenIdConnectAuthenticationOptions(string canonicalHostNameForEnvironment)
        {
            // Configure Auth0 parameters
            string auth0Domain = ConfigurationManager.AppSettings["auth0:Domain"];
            string auth0ClientId = ConfigurationManager.AppSettings["auth0:ClientId"];

            return new OpenIdConnectAuthenticationOptions
            {

                AuthenticationType = "Auth0",
                Authority = $"https://{auth0Domain}",
                ClientId = auth0ClientId,
                Scope = "openid profile email",
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                },
                
                // More information on why the CookieManager needs to be set can be found here: 
                // https://docs.microsoft.com/en-us/aspnet/samesite/owin-samesite
                CookieManager = new SameSiteCookieManager(new SystemWebCookieManager()),
                //BackchannelCertificateValidator = new AllowAllCertificatesValidator(),
                // Configure Auth0's Logout URL by hooking into the RedirectToIdentityProvider notification, 
                // which is getting triggered before any redirect to Auth0 happens.
                Notifications = CreateAuth0OpenIdConnectAuthenticationNotifications(auth0Domain, auth0ClientId, canonicalHostNameForEnvironment)
            };
        }

        private OpenIdConnectAuthenticationNotifications CreateAuth0OpenIdConnectAuthenticationNotifications(string auth0Domain, string auth0ClientId, string canonicalHostNameForEnvironment)
        {
            return new OpenIdConnectAuthenticationNotifications
            {
                AuthenticationFailed = (context) =>
                {
                    SitkaHttpApplication.Logger.Info(
                        $"Owin Startup - Configuration - AuthenticationFailed AuthType:{FirmaWebConfiguration.AuthenticationType}");
                    if ((context.Exception.Message.StartsWith("OICE_20004") ||
                         context.Exception.Message.Contains("IDX10311")))
                    {
                        context.SkipToNextMiddleware();
                        return Task.FromResult(0);
                    }

                    return Task.FromResult(0);
                },
                SecurityTokenValidated = n =>
                {
                    SitkaHttpApplication.Logger.Info(
                        $"In SecurityTokenValidated: TenantID {HttpRequestStorage.Tenant.TenantID}, Url: {n.Request.Uri.ToString()}, AuthType:{FirmaWebConfiguration.AuthenticationType}");

                    var claimsIdentity = n.AuthenticationTicket.Identity;
                    claimsIdentity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                    if (n.ProtocolMessage.Code != null)
                    {
                        claimsIdentity.AddClaim(new Claim("code", n.ProtocolMessage.Code));
                    }

                    if (n.ProtocolMessage.AccessToken != null)
                    {
                        claimsIdentity.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));
                    }

                    //map name claim to default name type
                    claimsIdentity.AddClaim(new Claim(
                        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                        claimsIdentity.FindFirst(Auth0OpenIDClaimTypes.Name).Value.ToString()));

                    if (claimsIdentity.IsAuthenticated) // we have a token and we can determine the person.
                    {
                        Auth0OpenIDUtilities.OpenIDClaimHandler(SyncLocalAccountStore, claimsIdentity);
                    }

                    return Task.FromResult(0);
                },

                RedirectToIdentityProvider = notification =>
                {
                    var request = notification.Request;
                    var response = notification.Response;
                    string redirectUri = $"https://{canonicalHostNameForEnvironment}/Account/LogOn"; // this has to match the keystone client redirect uri;
                    string postLogoutRedirectUri = $"https://{canonicalHostNameForEnvironment}/Account/LogOff";

                    if (!string.IsNullOrEmpty(redirectUri))
                    {
                        notification.ProtocolMessage.RedirectUri = redirectUri;
                    }

                    if (!string.IsNullOrEmpty(redirectUri))
                    {
                        notification.ProtocolMessage.PostLogoutRedirectUri = postLogoutRedirectUri;
                    }

                    //// Auth Request
                    if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.AuthenticationRequest)
                    {

                        // Detect "plumbing" hits (Auth0 return / login endpoint / OIDC callback-ish)
                        var req = notification.OwinContext.Request;

                        bool looksLikeAuth0Return =
                            !string.IsNullOrEmpty(req.Query["iss"]) ||
                            !string.IsNullOrEmpty(req.Query["code"]) ||
                            !string.IsNullOrEmpty(req.Query["state"]);

                        // Don't compute returnTo when it's Auth0 returning control to us.
                        // - ReturnUrl was explicitly provided, OR
                        // - we don't already have one AND this isn't an auth plumbing request
                        //var explicitReturnUrl = req.Query["ReturnUrl"];
                        bool shouldSet = !looksLikeAuth0Return;

                        if (shouldSet)
                        {
                            var returnTo = req.Query["returnTo"];
                            if (!string.IsNullOrWhiteSpace(returnTo))
                            {
                                var safeReturnTo = FirmaHelpers.ValidateReturnUrl(returnTo);
                                if (!string.IsNullOrEmpty(safeReturnTo))
                                {
                                    notification.OwinContext.Response.Cookies.Append(
                                        "ReturnURL",
                                        safeReturnTo,
                                        new Microsoft.Owin.CookieOptions
                                        {
                                            HttpOnly = true,
                                            Secure = req.Scheme == "https",
                                            SameSite = Microsoft.Owin.SameSiteMode.Lax, // Lax usually works best
                                        }
                                    );
                                }
                            }
                        }
                    }

                    // Handle logout
                    if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                    {
                        var logoutUri = $"https://{auth0Domain}/v2/logout?client_id={auth0ClientId}";

                        var postLogoutUri = notification.ProtocolMessage.PostLogoutRedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            if (postLogoutUri.StartsWith("/"))
                            {
                                postLogoutUri = $"{request.Scheme}://{request.Host}{request.PathBase}{postLogoutUri}";
                            }

                            logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                        }

                        response.Redirect(logoutUri);
                        notification.HandleResponse();
                    }

                    return Task.FromResult(0);
                }
            };
        }
        
        /// <summary>
        /// Gets a value from a named cookie using a specified key.
        /// </summary>
        public static string GetCookieValue(IOwinRequest request, string cookieName, string key)
        {
            if (request == null || string.IsNullOrEmpty(cookieName) || string.IsNullOrEmpty(key))
                return null;

            var cookie = request.Cookies[cookieName];
            if (string.IsNullOrEmpty(cookie))
                return null;

            var parsed = HttpUtility.ParseQueryString(cookie);
            return parsed[key];
        }
        
        public IAuth0User SyncLocalAccountStore(IAuth0UserClaims auth0UserClaims,
            IIdentity userIdentity)
        {
            SitkaHttpApplication.Logger.DebugFormat(
                "In SyncLocalAccountStore - User '{0}', Authenticated = '{1}'",
                userIdentity.Name,
                userIdentity.IsAuthenticated);

            var sendNewUserNotification = false;
            
            int tenantId = HttpRequestStorage.Tenant.TenantID;

            Person person = HttpRequestStorage.DatabaseEntities.People.GetPersonByAuth0IdAndTenant(auth0UserClaims.Subject, tenantId, false);

            // It can be useful to have the EXACT same time when looking for/at records later.
            var currentDateTime = DateTime.Now;
            if (person == null)
            {
                
                var personWithSameEmail = HttpRequestStorage.DatabaseEntities.People.GetPersonByEmailAndTenant(auth0UserClaims.Email, tenantId);
                if (personWithSameEmail != null)
                {
                    if (personWithSameEmail.Auth0ID == null)
                    {
                        person = personWithSameEmail;
                        person.Auth0ID = auth0UserClaims.Subject;
                        if (person.Organization == null ||
                            person.Organization.OrganizationID == GetUnknownOrganizationId())
                        {
                            person.OrganizationID = ComputeOrganizationID(auth0UserClaims.Email);
                        }
                    }
                    else
                    {
                        throw new Exception(
                            "Cannot create user. A user with the same email address is already in the system.");
                    }
                }
                else
                {
                    person = HandleNewUser(auth0UserClaims, currentDateTime, out sendNewUserNotification);
                }
            }
            else
            {
                // existing user - sync values
                SitkaHttpApplication.Logger.DebugFormat("In SyncLocalAccountStore - syncing local profile for User '{0}'", auth0UserClaims.UserGuid);
            }

            person.FirstName = auth0UserClaims.FirstName;
            person.LastName = auth0UserClaims.LastName;
            person.Email = auth0UserClaims.Email;
            person.LoginName = auth0UserClaims.Email;

            FirmaOwinStartup.MakeFirmaSessionForPersonLoggingIn(person, currentDateTime);

            if (sendNewUserNotification)
            {
                SendNewUserCreatedMessage(person, auth0UserClaims.LoginName);
            }

            return HttpRequestStorage.Person;
        }
        private static int GetUnknownOrganizationId()
        {
            var unknownOrganization = HttpRequestStorage.DatabaseEntities.Organizations.GetUnknownOrganization();
            return unknownOrganization.OrganizationID;
        }

        private Person HandleNewUser(IAuth0UserClaims auth0UserClaims, DateTime currentDateTime,
            out bool sendNewUserNotification)
        {
            Person person;
            // new user - provision with limited role
            SitkaHttpApplication.Logger.DebugFormat(
                "In SyncLocalAccountStore - creating local profile for User '{0}'", auth0UserClaims.UserGuid);
            var organizationId = ComputeOrganizationID(auth0UserClaims.Email);
            person = new Person(
                auth0UserClaims.FirstName,
                auth0UserClaims.LastName,
                auth0UserClaims.Email,
                Role.Unassigned.RoleID,
                currentDateTime,
                true,
                organizationId,
                false,
                auth0UserClaims.LoginName);
            person.Auth0ID = auth0UserClaims.Subject;
            person.TenantID = HttpRequestStorage.Tenant.TenantID;
            HttpRequestStorage.DatabaseEntities.AllPeople.Add(person);
            sendNewUserNotification = true;
            return person;
        }

        private int ComputeOrganizationID(string UserEmail)
        {
            var emailDomain = GetDomainFromEmail(UserEmail);
            var matchedOrganization = HttpRequestStorage.DatabaseEntities.Organizations.GetOrganizationByDomain(emailDomain);
            var organization = matchedOrganization ?? HttpRequestStorage.DatabaseEntities.Organizations.GetUnknownOrganization();
            return organization.OrganizationID;
        }

        private static string GetDomainFromEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var parts = email.Trim().Split('@');
            return parts.Length == 2 ? parts[1] : null;
        }

        private static void SendNewUserCreatedMessage(Person person, string loginName)
        {
            var subject =
                $"{MultiTenantHelpers.GetToolDisplayName()} User added: {person.GetFullNameFirstLast()} ({person.GetOrganizationDescriptor()})";
            var message = $@"
                <div style='font-size: 12px; font-family: Arial'>
                        <strong>User added:</strong> {person.GetFullNameFirstLast()}<br />
                        <strong>Organization</strong> {person.GetOrganizationDescriptor()} <br />
                        <strong>Added on:</strong> {DateTime.Now}<br />
                        <strong>Email:</strong> {person.Email}<br />
                        <br />
                        <p>
                            You may want to <a href=""{SitkaRoute<UserController>.BuildAbsoluteUrlFromExpression(x => x.Detail(person.PersonID))}"">assign this user roles</a> to allow them to work with specific areas of the site. Or you can leave the user with an unassigned role if they don't need special privileges.
                        </p>
                        <br />
                    <div style='font-size: 10px; color: gray'>
                    OTHER DETAILS:<br />
                    LOGIN: {loginName}<br />
                    <br />
                    </div>
                    <div>{$"- {MultiTenantHelpers.GetToolDisplayName()} team"}<br/><br/><img src=""cid:tool-logo"" width=""160"" /></div>
                    <div>You received this email because you are set up as a point of contact for support - if that's not correct, let us know: {FirmaWebConfiguration.SitkaSupportEmail}.</div>
                </div>
                ";

            SendMessageImpl(person, subject, message);
        }

        private static void SendMessageImpl(Person person, string subject, string message)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(FirmaWebConfiguration.DoNotReplyEmail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            var tenantAttribute = MultiTenantHelpers.GetTenantAttributeFromCache();
            var toolLogo = tenantAttribute.TenantSquareLogoFileResourceInfo ??
                           tenantAttribute.TenantBannerLogoFileResourceInfo;
            var htmlView = AlternateView.CreateAlternateViewFromString(message, null, "text/html");
            htmlView.LinkedResources.Add(
                new LinkedResource(new MemoryStream(toolLogo.FileResourceData.Data), "img/jpeg") { ContentId = "tool-logo" });
            mailMessage.AlternateViews.Add(htmlView);

            // Reply-To Header
            mailMessage.ReplyToList.Add(person.Email);

            // TO field
            var supportPersons = HttpRequestStorage.DatabaseEntities.People.GetPeopleWhoReceiveSupportEmails();
            foreach (var supportPerson in supportPersons)
            {
                mailMessage.To.Add(supportPerson.Email);
            }

            SitkaSmtpClient.Send(mailMessage);
        }
    }
}