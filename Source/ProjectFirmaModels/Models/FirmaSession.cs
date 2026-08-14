using System;
using System.Linq;
using System.Web;
using LtInfo.Common;
using LtInfo.Common.DesignByContract;

namespace ProjectFirmaModels.Models
{
    public partial class FirmaSession : IAuditableEntity
    {
        private DatabaseEntities DatabaseEntities;

        public Role Role
        {
            get
            {
                if (Person == null)
                {
                    // Anonymous always has the Unassigned role
                    return Role.Unassigned;
                }

                // Otherwise, we use the actual Person's role.
                return this.Person.Role;
            }
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <returns></returns>
        public static FirmaSession MakeEmptyFirmaSession(DatabaseEntities databaseEntities, Tenant tenant)
        {
            Check.EnsureNotNull(tenant, "Tenant must not be null. Should be current Tenant.");
            Check.EnsureNotNull(tenant, "DatabaseEntities must be provided.");

            // I'd prefer just to write this as a real constructor, but it's already in EF generated code, so we resort to this.
            var newFirmaSession = new FirmaSession();
            newFirmaSession.FirmaSessionGuid = Guid.NewGuid();
            var currentDateTime = DateTime.Now;
            newFirmaSession.CreateDate = currentDateTime;
            newFirmaSession.LastActivityDate = currentDateTime;
            newFirmaSession.TenantID = tenant.TenantID;
            newFirmaSession.SetDatabaseEntities(databaseEntities);

            return newFirmaSession;
        }

        /// <summary>
        /// Constructor for a new FirmaSession for a given Person
        /// </summary>
        /// <param name="databaseEntities"></param>
        /// <param name="person"></param>
        public FirmaSession(DatabaseEntities databaseEntities, Person person)
        {
            Check.EnsureNotNull(person, "Do not call this if Person is null");
            Check.Ensure(person.TenantID > 0, $"Person does not have a TenantID set (TenantID = {person.TenantID})");
            Check.EnsureNotNull(person.Tenant, "Person does not have a Tenant set");

            FirmaSessionGuid = Guid.NewGuid();
            var currentDateTime = DateTime.Now;
            CreateDate = currentDateTime;
            LastActivityDate = currentDateTime;
            Person = person;

            TenantID = person.Tenant.TenantID;
            SetDatabaseEntities(databaseEntities);
        }

        /// <summary>
        /// Call with caution!
        /// </summary>
        /// <param name="databaseEntities"></param>
        public void SetDatabaseEntities(DatabaseEntities databaseEntities)
        {
            this.DatabaseEntities = databaseEntities;
        }

        public string GetAuditDescriptionString()
        {
            return string.Format($"{this.FirmaSessionGuid}");
        }

        public bool IsAnonymousUser()
        {
            return Person == null;
        }

        public bool IsAnonymousOrUnassigned()
        {
            return IsAnonymousUser() || this.Role == Role.Unassigned;
        }

        public bool IsSitkaAdministrator()
        {
            return this.Person != null && this.Role == Role.ESAAdmin;
        }

        public bool IsAdministrator()
        {
            return this.Person != null && (this.Role == Role.Admin || IsSitkaAdministrator());
        }

        public string GetFullNameFirstLast()
        {
            if (this.Person == null)
            {
                return "(Anonymous)";
            }

            return this.Person.GetFullNameFirstLast();
        }

        public string GetFullNameFirstLastWithAsteriskIfImpersonating()
        {
            if (this.Person == null)
            {
                return "(Anonymous)";
            }

            string asteriskIfImpersonating = this.IsImpersonating() ? "*" : string.Empty;
            var fullName = this.Person.GetFullNameFirstLast();
            return string.Format($"{fullName}{asteriskIfImpersonating}");
        }

        #region Impersonation

        public void ImpersonateUser(Person personToImpersonate,
                                    Uri optionalPreviousPageUri,
                                    out string impersonationStatusMessage,
                                    out string impersonationStatusWarning)
        {
            Check.EnsureNotNull(this.PersonID, "Anonymous users can't impersonate authentic users.");
            Check.EnsureNotNull(personToImpersonate, "You can't impersonate an Anonymous user.");

            impersonationStatusWarning = null;
            string currentImpersonationString = string.Empty;
            // Keep track of who we started as -- unless we are *already* impersonating,
            // in which case we keep our original identity through each new impersonation.
            if (OriginalPerson == null)
            {
                OriginalPerson = Person;
            }
            else
            {
                // If we are trying to impersonate *ourself*, we instead resume our original session.
                // (there are other checks elsewhere to prevent and warn about this, but this is a last-ditch exit.)
                if (OriginalPerson.PersonID == personToImpersonate.PersonID)
                {
                    ResumeOriginalUser(optionalPreviousPageUri, out impersonationStatusMessage);
                    return;
                }

                currentImpersonationString = $" (currently impersonating {HttpUtility.HtmlEncode(Person.GetFullNameFirstLast())})";
            }

            var lastPageLinkHtml = MakeLastPageLinkHtml(optionalPreviousPageUri);

            // Switch to the new user we want to impersonate
            Person = personToImpersonate;
            // These status messages are HTML by contract -- they carry lastPageLinkHtml -- and are rendered with
            // Html.Raw out of TempData by SiteLayout / Home Index. So the person names interpolated into them must
            // be encoded here, at the point they become HTML.
            impersonationStatusMessage = $"Logon {HttpUtility.HtmlEncode(OriginalPerson.GetFullNameFirstLast())}{currentImpersonationString} switching to impersonate Logon {HttpUtility.HtmlEncode(personToImpersonate.GetFullNameFirstLast())}.{lastPageLinkHtml}";
            //_logger.InfoFormat(impersonationStatusMessage);

            if (!personToImpersonate.IsActive)
            {
                impersonationStatusWarning = $"Logon {HttpUtility.HtmlEncode(personToImpersonate.GetFullNameFirstLast())} is inactive. Impersonation will allow you to act as this person, but be aware of potential issues due to the account being inactive.";
            }
        }

        public static string MakeLastPageLinkHtml(Uri optionalPreviousPageUri)
        {
            // Work out what the last page was, if possible, so we can offer the user a link to it 
            // so they can return to what they were doing once they've succeeded in impersonating.
            //
            // We can't just take them to the last page since there is zero guarantee they have rights to be there.
            // So we leave it up to the user, but make it easier for them.
            string lastPageLinkHtml = String.Empty;
            if (optionalPreviousPageUri != null)
            {
                string lastPageUrl = optionalPreviousPageUri.ToString();
                if (GeneralUtility.IsNullOrEmptyOrOnlyWhitespace(lastPageUrl) == false)
                {
                    string lastPageLinkText = " Return to last page";
                    var linkToLastPage = BuildLastUrlLinkFromUrl(lastPageUrl, lastPageLinkText, lastPageLinkText);
                    lastPageLinkHtml = $"<br/><br/>{linkToLastPage}.";
                }
            }

            return lastPageLinkHtml;
        }

        public static string BuildLastUrlLinkFromUrl(string url, string linkText, string titleText)
        {
            // url comes from Request.UrlReferrer and lands in a quoted attribute, so attribute-encode it.
            return $"<a href=\"{HttpUtility.HtmlAttributeEncode(url)}\" title=\"{HttpUtility.HtmlAttributeEncode(titleText)}\">{HttpUtility.HtmlEncode(linkText)}</a>";
        }

        public void ResumeOriginalUser(Uri optionalPreviousPageUri, out string impersonationStatusMessage)
        {
            var lastPageLinkHtml = MakeLastPageLinkHtml(optionalPreviousPageUri);

            Check.EnsureNotNull(OriginalPerson, "FirmaSession {0} is not impersonating; it is not valid to call ResumeOriginalUser()");
            impersonationStatusMessage = $"Logon {HttpUtility.HtmlEncode(OriginalPerson.GetFullNameFirstLast())} resuming their original session; ceasing impersonation of Logon {HttpUtility.HtmlEncode(Person.GetFullNameFirstLast())}.{lastPageLinkHtml}";
            //_logger.InfoFormat(impersonationStatusMessage);
            // Swap back
            Person = OriginalPerson;
            // Clear impersonation data
            OriginalPerson = null;
            OriginalPersonID = null;
        }

        /// <summary>
        /// Is the current FirmaSession impersonating another user? 
        /// </summary>
        /// <returns></returns>
        public bool IsImpersonating()
        {
            // You can also use this extension directly.
            return FirmaSessionExtensions.IsImpersonating(this);
        }

        #endregion Impersonation

        public const string AnonymousDisplayString = "(anonymous)";

        public string UserDisplayName
        {
            get
            {
                return IsAnonymousOrUnassigned()
                    ? AnonymousDisplayString:
                    Person.GetFullNameFirstLast();
            }
        }

        private LastSQLServerDatabaseBackup _lastSqlServerDatabaseBackup;
        public LastSQLServerDatabaseBackup LastSqlServerDatabaseBackupInfo
        {
            get
            {
                if (_lastSqlServerDatabaseBackup == null)
                {
                    // We are expecting only one entry in this table, so this should be safe/correct.
                    _lastSqlServerDatabaseBackup = this.DatabaseEntities?.LastSQLServerDatabaseBackups.Single();
                }

                return _lastSqlServerDatabaseBackup;
            }
        }


    }
}