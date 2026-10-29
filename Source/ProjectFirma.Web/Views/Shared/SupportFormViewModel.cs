/*-----------------------------------------------------------------------
<copyright file="SupportFormViewModel.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using ProjectFirma.Web.Common;
using ProjectFirmaModels.Models;
using LtInfo.Common;
using LtInfo.Common.Models;
using ProjectFirma.Web.Models;

namespace ProjectFirma.Web.Views.Shared
{
    public class SupportFormViewModel : FormViewModel, IValidatableObject
    {
        [Required]
        [DisplayName("Name")]
        [StringLength(SupportRequestLog.FieldLengths.RequestPersonName)]
        public String RequestPersonName { get; set; }

        [Required]
        [DisplayName("Email Address")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^([\\w-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([\\w-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$",
            ErrorMessage = "Email supplied does not appear to be formatted correctly")]
        [StringLength(SupportRequestLog.FieldLengths.RequestPersonEmail)]
        public String RequestPersonEmail { get; set; }

        [Required]
        [DisplayName("Description")]
        [StringLength(SupportRequestLog.FieldLengths.RequestDescription)]
        public string RequestDescription { get; set; }

        [DisplayName("Organization")]
        [StringLength(SupportRequestLog.FieldLengths.RequestPersonOrganization)]
        public string RequestPersonOrganization { get; set; }

        [DisplayName("Phone")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(SupportRequestLog.FieldLengths.RequestPersonPhone)]
        public string RequestPersonPhone { get; set; }

        [Required]
        [DisplayName("Subject")]
        public SupportRequestTypeEnum? SupportRequestTypeEnum { get; set; }

        public string CurrentPageUrl { get; set; }

        // Needed by model binder
        public SupportFormViewModel()
        {
        }

        public SupportFormViewModel(string currentPageUrl, SupportRequestTypeEnum? supportRequestTypeEnum)
        {
            CurrentPageUrl = currentPageUrl;
            SupportRequestTypeEnum = supportRequestTypeEnum;
        }

        public void UpdateModel(SupportRequestLog supportRequestLog, FirmaSession currentFirmaSession)
        {
            supportRequestLog.RequestPersonName = RequestPersonName;
            supportRequestLog.RequestPersonEmail = RequestPersonEmail;
            supportRequestLog.RequestPersonOrganization = RequestPersonOrganization;
            supportRequestLog.RequestPersonPhone = RequestPersonPhone;
            // ReSharper disable once PossibleInvalidOperationException
            supportRequestLog.SupportRequestTypeID = (int) SupportRequestTypeEnum.Value;
            supportRequestLog.RequestDescription = RequestDescription;
            supportRequestLog.RequestDate = DateTime.Now;
            if (!currentFirmaSession.IsAnonymousUser())
            {
                supportRequestLog.RequestPersonID = currentFirmaSession.PersonID;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            var httpContext = HttpContext.Current;
            if (!httpContext.User.Identity.IsAuthenticated) // this means we are anonymous
            {
                var ipAddress = string.Empty;
                if (httpContext != null)
                {
                    ipAddress = httpContext.Request.UserHostAddress;
                }
                var gRecaptchaResponse = httpContext.Request.Form["g-recaptcha-response"];
                if (
                    !RecaptchaValidator.IsValidResponse(gRecaptchaResponse,
                        ipAddress,
                        FirmaWebConfiguration.RecaptchaPrivateKey,
                        FirmaWebConfiguration.RecaptchaValidatorUrl,
                        SitkaLogger.Instance.LogDetailedWarningMessage))
                {
                    errors.Add(new ValidationResult("Your Captcha response is incorrect."));
                }
            }
            return errors;
        }
    }
}
