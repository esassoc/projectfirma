/*-----------------------------------------------------------------------
<copyright file="PersonModelExtensions.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using LtInfo.Common.DesignByContract;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Controllers;
using ProjectFirma.Web.Security;
using ProjectFirmaModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using System.Data.Entity;

namespace ProjectFirma.Web.Models
{

    /// <summary>
    /// These have been implemented as extension methods on <see cref="Person"/> so we can handle the anonymous user as a null person object
    /// </summary>
    public static class PersonModelExtensions
    {
        public static HtmlString GetFullNameFirstLastAsUrl(this Person person, FirmaSession currentFirmaSession)
        {
            if (new UserViewFeature().HasPermission(currentFirmaSession, person).HasPermission)
            {
                return UrlTemplate.MakeHrefString(person.GetDetailUrl(), person.GetFullNameFirstLast());
            }
            return new HtmlString(HttpUtility.HtmlEncode(person.GetFullNameFirstLast()));
        }

        public static HtmlString GetFullNameFirstLastAndOrgAsUrl(this Person person, FirmaSession currentFirmaSession)
        {
            var userUrl = person.GetFullNameFirstLastAsUrl(currentFirmaSession);
            var orgUrl = person.Organization.GetDisplayNameAsUrl();
            return new HtmlString($"{userUrl} - {orgUrl}");
        }

        public static HtmlString GetFullNameFirstLastAndOrgShortNameAsUrl(this Person person, FirmaSession currentFirmaSession)
        {
            var userUrl = person.GetFullNameFirstLastAsUrl(currentFirmaSession);
            var orgUrl = person.Organization.GetShortNameAsUrl();
            return new HtmlString($"{userUrl} ({orgUrl})");
        }

        public static string GetFullNameFirstLastAndOrg(this Person person) => $"{person.FirstName} {person.LastName} - {person.Organization.GetDisplayName()}";

        public static string GetFullNameFirstLastAndOrgShortName(this Person person) =>
            $"{person.FirstName} {person.LastName} ({person.Organization.GetOrganizationShortNameIfAvailable()})";

        public static string GetOrganizationDescriptor(this Person person)
        {
            return person.Organization.IsUnknown()
                ? "Private Individual"
                : person.Organization.OrganizationName;
        }

        public static string GetEditUrl(this Person person)
        {
            return SitkaRoute<UserController>.BuildUrlFromExpression(t => t.EditRoles(person));
        }

        public static readonly UrlTemplate<int> DeleteUrlTemplate = new UrlTemplate<int>(SitkaRoute<UserController>.BuildUrlFromExpression(t => t.Delete(UrlTemplate.Parameter1Int)));
        public static string GetDeleteUrl(this Person person)
        {
            return DeleteUrlTemplate.ParameterReplace(person.PersonID);
        }

        public static readonly UrlTemplate<int> DetailUrlTemplate = new UrlTemplate<int>(SitkaRoute<UserController>.BuildUrlFromExpression(t => t.Detail(UrlTemplate.Parameter1Int)));
        public static string GetDetailUrl(this Person person)
        {
            return person != null ? DetailUrlTemplate.ParameterReplace(person.PersonID) : null;
        }

        // Really you should prefer the equivalent function on FIrmaSession
        [Obsolete]
        public static bool IsAnonymous(this Person person) => person == null;

        // The Anonymous (null) user has the Unassigned role
        public static bool IsUnassigned(this Person person) => person == null || person.Role == Role.Unassigned;

        public static bool IsSitkaAdministrator(this Person person)
        {
            return person != null && person.Role == Role.ESAAdmin;
        }

        public static bool IsAdministrator(this Person person)
        {
            return person != null && (person.Role == Role.Admin || IsSitkaAdministrator(person));
        }

        public static bool IsApprover(this Person person)
        {
            return person != null && (person.IsAdministrator() || person.IsSitkaAdministrator());
        }
        
        public static bool ShouldReceiveNotifications(this Person person)
        {
            return person != null && person.ReceiveSupportEmails;
        }

        public static string GetKeystoneEditLink(this Person person)
        {
            if (person == null)
            {
                return "[No Keystone Edit Link]";
            }
            return $"{FirmaWebConfiguration.KeystoneUserProfileUrl}{person.PersonGuid}";
        }
        public static string GetAuth0EditLink(this Person person)
        {
            if (person == null)
            {
                return "[No Auth0 Edit Link]";
            }
            return $"{FirmaWebConfiguration.KeystoneUserProfileUrl}{person.PersonGuid}";
        }

        /// <summary>
        /// List of Projects for which this Person is the primary contact
        /// </summary>
        public static List<Project> GetPrimaryContactProjects(this Person person, FirmaSession currentFirmaSession)
        {
            var isPersonTheCurrentUser = !currentFirmaSession.IsAnonymousUser() && currentFirmaSession.PersonID == person.PersonID;
            if (isPersonTheCurrentUser)
            {
                return person.ProjectsWhereYouAreThePrimaryContactPerson.ToList().Where(x => x.ProjectStage != ProjectStage.Terminated).ToList();
            }
            return person.ProjectsWhereYouAreThePrimaryContactPerson.ToList().GetActiveProjectsAndProposals(currentFirmaSession.CanViewProposals(), currentFirmaSession).ToList();
        }

        /// <summary>
        /// List of Projects for which this Person is a contact
        /// </summary>
        public static List<Project> GetProjectsWhereYouAreAContact(this Person person)
        {
            var projectsUserIsAContact = person.ProjectContactsWhereYouAreTheContact.Select(x => x.Project).ToList();
            var projectsUserIsThePrimaryContactPerson = person.ProjectsWhereYouAreThePrimaryContactPerson;
            projectsUserIsAContact.AddRange(projectsUserIsThePrimaryContactPerson);
            return projectsUserIsAContact.Distinct().ToList();
        }

        public static List<Project> GetPrimaryContactUpdatableProjects(this Person person, FirmaSession currentFirmaSession)
        {
            return person.GetPrimaryContactProjects(currentFirmaSession).Where(x => x.IsUpdatableViaProjectUpdateProcess()).ToList();
        }

        public static bool IsPersonAProjectOwnerWhoCanStewardProjects(this Person person)
        {
            // If anonymous, definitely not Project owner or Steward
            if (person == null)
            {
                return false;
            }

            var canStewardProjectsOrganizationRelationship = MultiTenantHelpers.GetCanStewardProjectsOrganizationRelationship();
            if (MultiTenantHelpers.GetProjectStewardshipAreaType() == ProjectStewardshipAreaType.ProjectStewardingOrganizations)
            {
                return Role.ProjectSteward.RoleID == person.RoleID &&
                       canStewardProjectsOrganizationRelationship != null &&
                       canStewardProjectsOrganizationRelationship.OrganizationTypeOrganizationRelationshipTypes.Any(
                           x => x.OrganizationTypeID == person.Organization.OrganizationTypeID);
            }

            return Role.ProjectSteward.RoleID == person.RoleID;
        }


        public static List<HtmlString> GetProjectStewardshipAreaHtmlStringList(this Person person)
        {
            return MultiTenantHelpers.GetProjectStewardshipAreaType()?.GetProjectStewardshipAreaHtmlStringList(person);
        }

        public static bool CanViewPendingProjects(this Person person) => new PendingProjectsViewListFeature().HasPermissionByPerson(person);

        public static bool CanStewardProject(this Person person, Project project)
        {
            return MultiTenantHelpers.GetProjectStewardshipAreaType()?.CanStewardProject(person, project) ?? true;
        }

        public static Person GetPersonByEmail(this IQueryable<Person> people, string email)
        {
            return GetPersonByEmail(people, email, true);
        }

        public static Person GetPersonByEmail(this IQueryable<Person> people, string email, bool requireRecordFound)
        {
            var person = people.SingleOrDefault(x => x.Email == email);
            if (requireRecordFound)
            {
                Check.RequireNotNullThrowNotFound(person, email);
            }
            return person;
        }
        public static Person GetFirstPersonByEmail(this IQueryable<Person> people, string targetEmail)
        {
            if (!IsExistsPersonByEmail(people, targetEmail)) return null;
            return people.First(p => p.Email == targetEmail);
        }

        public static bool IsExistsPersonByEmail(this IQueryable<Person> people, string targetEmail)
        {
            return people.Any(p => p.Email == targetEmail);
        }

        public static Person GetPersonByAuth0Id(this IQueryable<Person> people, string auth0Id)
        {
            return people.SingleOrDefault(x => x.Auth0ID == auth0Id);
        }

        public static Person GetPersonByAuth0IdAndTenant(this IQueryable<Person> people, string auth0Id, int targetTenantId, bool requireRecordFound)
        {
            var person = people.SingleOrDefault(x => x.Auth0ID == auth0Id && x.TenantID == targetTenantId);
            if (requireRecordFound)
            {
                Check.RequireNotNullThrowNotFound(person, auth0Id);
            }
            return person;
        }

        public static Person GetPersonByEmailAndTenant(this IQueryable<Person> people, string email, int targetTenantId)
        {
            return people
                .SingleOrDefault(x => x.Email == email && x.TenantID == targetTenantId);
        }

        public static Person GetPersonByPersonGuid(this IQueryable<Person> people, Guid personGuid)
        {
            return GetPersonByPersonGuid(people, personGuid, false);
        }

        public static Person GetPersonByPersonGuid(this IQueryable<Person> people, Guid personGuid, bool requireRecordFound)
        {
            var person = people.SingleOrDefault(x => x.PersonGuid == personGuid);
            if (requireRecordFound)
            {
                Check.RequireNotNullThrowNotFound(person, personGuid.ToString());
            }
            return person;
        }

        public static Person GetPersonByWebServiceAccessToken(this IQueryable<Person> people, Guid webServiceAccessToken)
        {
            var person = people.SingleOrDefault(x => x.WebServiceAccessToken == webServiceAccessToken);
            return person;
        }

        public static List<Person> GetActivePeople(this IQueryable<Person> people)
        {
            return people.Where(x => x.IsActive).ToList().OrderBy(ht => ht.GetFullNameLastFirst()).ToList();
        }

        public static List<Person> GetPeopleWhoReceiveNotifications(this IQueryable<Person> people)
        {
            return people.ToList().Where(x => x.ShouldReceiveNotifications()).OrderBy(ht => ht.GetFullNameLastFirst()).ToList();
        }

        public static List<Person> GetPeopleWhoReceiveSupportEmails(this IQueryable<Person> people)
        {
            return people.ToList().Where(x => x.ReceiveSupportEmails && x.IsActive).OrderBy(ht => ht.GetFullNameLastFirst()).ToList();
        }

        /// <summary>
        /// Returns a HtmlString of the person's first and last name and their relationship to the provided project (if any)
        /// in a comma separated list inside parentheses
        ///
        /// e.g. "Stewart Gordon (Project Primary Contact, NTA Coordinator, NTA Manager)" if the person is multiple types of contacts on a project
        /// or "Stewart Gordon" if the person isn't any type of contact on the project
        /// </summary>
        /// <param name="person"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static HtmlString GetPersonDisplayNameWithContactTypesListForProject(this Person person, Project project)
        {
            var personContactTypesList = person.GetListOfContactTypeStringsForProject(project);

            if (!personContactTypesList.Any())
            {
                return new HtmlString(person.GetFullNameFirstLast());
            }

            return new HtmlString($"{person.GetFullNameFirstLast()} <span class=\"small\">({string.Join(", ", personContactTypesList)})</span>");
        }

        public static string GetPersonInformationStringForLogging(this Person person)
        {
            return $"PersonID {person.PersonID} {person.FirstName} {person.LastName} - {person.Organization.GetDisplayName()} ({person.Email})";
        }

        public static List<string> GetListOfContactTypeStringsForProject(this Person person, Project project)
        {
            var personContactTypesList = new List<string>();

            // Project primary contact
            if (project.PrimaryContactPerson != null)
            {
                if (person.PersonID == project.PrimaryContactPerson.PersonID)
                {
                    personContactTypesList.Add($"{FieldDefinitionEnum.ProjectPrimaryContact.ToType().GetFieldDefinitionLabel()}");
                }
            }

            // The rest of the project contacts
            var projectContactsThatAreThisPerson = project.ProjectContacts.Where(x => x.Contact.PersonID == person.PersonID);
            foreach (var projectContact in projectContactsThatAreThisPerson)
            {
                personContactTypesList.Add(projectContact.ContactRelationshipType.ContactRelationshipTypeName);
            }

            return personContactTypesList;
        }

        public static bool CanDeletePerson(this Person personToDelete, Person currentPerson)
        {
            var personsDependentObjectNames = personToDelete.DependentObjectNames();

            // We are going to allow full cascade deletion of a Person from the system only if their only dependent objects are in the list below.
            // It is worth being careful when adding any further objects to the list because cascade deletion might delete objects that you weren't aware of. 5/15/2020 SMG [#2148]
            var dependentObjectsThatAreSafeToDelete = new List<string>()
            {
                typeof(AuditLog).Name
            };

            var dependentObjectsThatAreNotSafeToDelete = personsDependentObjectNames.Where(x => dependentObjectsThatAreSafeToDelete.Any(y => y != x)).ToList();

            bool auditLogsAreSafeToDelete = !personToDelete.AuditLogs.Any();
            if (personsDependentObjectNames.Any(x => x == typeof(AuditLog).Name))
            {
                auditLogsAreSafeToDelete = personToDelete.PersonsAuditLogsAreSafeToDelete();
            }

            return !dependentObjectsThatAreNotSafeToDelete.Any() && auditLogsAreSafeToDelete &&
                   personToDelete != currentPerson;
        }

        public static bool PersonsAuditLogsAreSafeToDelete(this Person personToDelete)
        {
            var auditLogTablesThatAreSafeToDelete = new List<string>()
            {
                typeof(Person).Name,
                typeof(FirmaSession).Name
            };

            var auditLogs = personToDelete.AuditLogs.ToList();

            foreach (var auditLog in auditLogs)
            {
                // We have tried to limit this just to clearly deletable records. If you find yourself adding a great deal here,
                // or bending over backwards, we should stop and move to a user-marked-as-deleted model as was done in Gemini.
                // This is worth some pain, but not much more than this.

                // if the audit logs contains anything outside of the approved to delete audit log tables it is not safe to delete
                if (!auditLogTablesThatAreSafeToDelete.Contains(auditLog.TableName))
                {
                    return false;
                }
                // if an audit log entry on the Person table is related to a record other than the personToDelete, it is not safe to delete
                if (auditLog.TableName == "Person" && auditLog.RecordID != personToDelete.PersonID)
                {
                    return false;
                }
            }

            return true;
        }



    }
}
