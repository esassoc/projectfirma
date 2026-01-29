/*-----------------------------------------------------------------------
<copyright file="InviteViewData.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LtInfo.Common.ModalDialog;
using LtInfo.Common.Mvc;
using ProjectFirmaModels.Models;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Controllers;

namespace ProjectFirma.Web.Views.User
{
    public class InviteViewData : FirmaViewData
    {
        public IEnumerable<SelectListItem> OrganizationsSelectList { get; }
        public string CancelUrl { get; }
        public string IndexUrl { get; }
        public string OrganizationIndexUrl { get; }

        public InviteViewData(FirmaSession currentFirmaSession, ProjectFirmaModels.Models.FirmaPage psInfoPage) : base(currentFirmaSession, psInfoPage)
        {
            PageTitle = "Invite User";
            EntityName = "Users";

            CancelUrl = SitkaRoute<UserController>.BuildUrlFromExpression(x => x.Index());
            IndexUrl = SitkaRoute<UserController>.BuildUrlFromExpression(x => x.Index());
            OrganizationIndexUrl = SitkaRoute<OrganizationController>.BuildUrlFromExpression(x => x.Index());

            // ReSharper disable once ConvertClosureToMethodGroup
            var allOrganizations = HttpRequestStorage.DatabaseEntities.Organizations.ToList().OrderBy(x => x.OrganizationName).ToList();
            //var filteredOrganizations = allOrganizations.ToList();
            OrganizationsSelectList = allOrganizations.ToSelectList(x => x.OrganizationID.ToString(), x => x.OrganizationName);
        }

        private string MakeOrganizationNameWithKeystoneGuidWarning(ProjectFirmaModels.Models.Organization organization)
        {
            string noGuidWarning = organization.KeystoneOrganizationGuid == null ? " [No Keystone Organization GUID]" : string.Empty;
            return $"{organization.OrganizationName}{noGuidWarning}";
        }

        public HtmlString GetRequestOrgAddedToKeystoneModalDialogLink(string linkText)
        {
            string requestOrganizationAddedToKeystoneUrl = SitkaRoute<HelpController>.BuildUrlFromExpression(x => x.RequestOrganizationAddedToKeystone());
            HtmlString requestOrgAddedToKeystoneModalDialogLink = ModalDialogFormHelper.ModalDialogFormLink(linkText, requestOrganizationAddedToKeystoneUrl,
                "Request Support", 800, "Submit Request", "Cancel", new List<string>(), null, null);
            return requestOrgAddedToKeystoneModalDialogLink;
        }
    }
}
