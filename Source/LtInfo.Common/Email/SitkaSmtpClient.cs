/*-----------------------------------------------------------------------
<copyright file="SitkaSmtpClient.cs" company="Environmental Science Associates">
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
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using log4net;
using LtInfo.Common.Views;

namespace LtInfo.Common.Email
{
    /// <summary>
    /// Allows for tests to intercept emails, put the context from <see cref="SetToMockMode"/>
    /// </summary>
    public static class SitkaSmtpClient
    {
        public static readonly List<MailMessage> MockMailMessages;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SitkaSmtpClient));
        private static bool _isInRealMode;

        static SitkaSmtpClient()
        {
            MockMailMessages = new List<MailMessage>();
            SetToRealMode();
        }

        private static void SetToMockMode()
        {
            MockMailMessages.Clear();
            _isInRealMode = false;
        }

        private static void SetToRealMode()
        {
            _isInRealMode = true;
        }

        /// <summary>
        /// Wrap this into a Using block to start and stop context
        /// </summary>
        public static MockModeContext MockContextStart()
        {
            return new MockModeContext(SetToMockMode, SetToRealMode);
        }

        /// <summary>
        /// Sends an email including mock mode and address redirection  <see cref="SitkaWebConfiguration.SitkaEmailRedirect"/>, then calls onward to <see cref="SendDirectly"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="linkedResources"></param>
        public static void Send(MailMessage message, IEnumerable<LinkedResource> linkedResources = null)
        {
            if (_isInRealMode)
            {
                var messageWithAnyAlterations = AlterMessageIfInRedirectMode(message);
                var messageAfterAlterationsAndCreatingAlternateViews = CreateAlternateViewsIfNeeded(messageWithAnyAlterations, linkedResources);
                SendDirectly(messageAfterAlterationsAndCreatingAlternateViews);
            }
            else
            {
                MockMailMessages.Add(message);
            }
        }

        private static MailMessage CreateAlternateViewsIfNeeded(MailMessage message, IEnumerable<LinkedResource> linkedResources )
        {
            if (!message.IsBodyHtml)
            {
                return message;
            }
            // Define the plain text alternate view and add to message
            const string plainTextBody = "You must use an email client that supports HTML messages";

            var plainTextView = AlternateView.CreateAlternateViewFromString(plainTextBody, null, MediaTypeNames.Text.Plain);

            message.AlternateViews.Add(plainTextView);

            // Define the html alternate view with embedded image and
            // add to message. To reference images attached as linked
            // resources from your HTML message body, use "cid:contentID"
            // in the <img> tag...
            string htmlBody = message.Body;

            message.Body = null;

            var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

            if (linkedResources != null )
            {
                foreach (var linkedResource in linkedResources)
                {
                    htmlView.LinkedResources.Add(linkedResource);
                }
            }
            message.AlternateViews.Add(htmlView);


            return message;
        }

        
        /// <summary>
        /// Sends an email message at a lower level than <see cref="Send"/>, skipping mock mode and address redirection  <see cref="SitkaWebConfiguration.SitkaEmailRedirect"/>
        /// Encapsulates all the access to <see cref="SmtpClient"/> so that the setting <see cref="SitkaWebConfiguration.IsEmailEnabled" /> can disable emailing as needed
        /// Also will use the <see cref="SitkaWebConfiguration.MailLogBcc" /> to send a BCC to another address
        /// </summary>
        /// <param name="mailMessage"></param>
        public static void SendDirectly(MailMessage mailMessage)
        {
            if (!String.IsNullOrWhiteSpace(SitkaWebConfiguration.MailLogBcc))
            {
                mailMessage.Bcc.Add(SitkaWebConfiguration.MailLogBcc);
            }

            var humanReadableDisplayOfMessage = GetHumanReadableDisplayOfMessage(mailMessage);
            if (SitkaWebConfiguration.IsEmailEnabled)
            {
                var smtpClient = new SmtpClient();
                smtpClient.Send(mailMessage);
                _logger.Info(string.Format("Email sent to SMTP server \"{0}\", Details:\r\n{1}", smtpClient.Host, humanReadableDisplayOfMessage));
            }
            else
            {
                _logger.Warn(string.Format("Skipping sending email because configuration setting IsEmailEnabled={0}, normally the site should be running with it on, are you sure you want emails disabled? (did not cause a user visible exception)\r\nMail Message that did not get sent:\r\n{1}", SitkaWebConfiguration.IsEmailEnabled, humanReadableDisplayOfMessage));
            }
        }

        /// <summary>
        /// Alter message TO, CC, BCC if the setting <see cref="SitkaWebConfiguration.SitkaEmailRedirect"/> is set
        /// Appends the real to the body
        /// </summary>
        /// <param name="realMailMessage"></param>
        /// <returns></returns>
        private static MailMessage AlterMessageIfInRedirectMode(MailMessage realMailMessage)
        {
            var redirectEmail = SitkaWebConfiguration.SitkaEmailRedirect;
            var isInRedirectMode = !GeneralUtility.IsNullOrEmptyOrOnlyWhitespace(redirectEmail);

            if (!isInRedirectMode)
            {
                return realMailMessage;
            }

            ClearOriginalAddressesAndAppendToBody(realMailMessage, "To", realMailMessage.To);
            ClearOriginalAddressesAndAppendToBody(realMailMessage, "CC", realMailMessage.CC);
            ClearOriginalAddressesAndAppendToBody(realMailMessage, "BCC", realMailMessage.Bcc);

            realMailMessage.To.Add(redirectEmail);

            return realMailMessage;
        }

        private static void ClearOriginalAddressesAndAppendToBody(MailMessage realMailMessage, string addressType, ICollection<MailAddress> addresses)
        {
            var newline = realMailMessage.IsBodyHtml ? "<br />" : Environment.NewLine;
            var separator = newline + "\t";

            var toExpected = addresses.Aggregate(String.Empty, (s, mailAddress) => s + Environment.NewLine + "\t" + mailAddress.ToString());
            if (!GeneralUtility.IsNullOrEmptyOrOnlyWhitespace(toExpected))
            {
                var toAppend = String.Format("{0}{1}Actual {2}:{3}", newline, separator, addressType, realMailMessage.IsBodyHtml ? toExpected.HtmlEncodeWithBreaks() : toExpected);
                realMailMessage.Body += toAppend;

                for (var i = 0; i < realMailMessage.AlternateViews.Count; i++)
                {
                    var stream = realMailMessage.AlternateViews[i].ContentStream;
                    using (var reader = new StreamReader(stream))
                    {
                        var alternateBody = reader.ReadToEnd();
                        alternateBody += toAppend;
                        var newAlternateView = AlternateView.CreateAlternateViewFromString(alternateBody, null, realMailMessage.AlternateViews[i].ContentType.MediaType);
                        realMailMessage.AlternateViews[i].LinkedResources.ToList().ForEach(x => newAlternateView.LinkedResources.Add(x));
                        realMailMessage.AlternateViews[i] = newAlternateView;
                    }
                }
                

            }
            addresses.Clear();
        }

        public static string GetRfc2822FormattedAddress(string emailAddress, string firstName, string lastName)
        {
            var name = String.Format("{0} {1}", firstName, lastName);
            return GetRfc2822FormattedAddress(emailAddress, name);
        }

        public static string GetRfc2822FormattedAddress(string emailAddress, string name)
        {
            // RFC 2821 -  The domain part of email address is case insensitive but the email address is case sensitive
            var safeName = Regex.Replace(name, @"[""\<\>]", String.Empty);
            safeName = GeneralUtility.AsciiToHex(safeName);

            return String.Format("\"{0}\" <{1}>", safeName, emailAddress);
        }

        private static string GetHumanReadableDisplayOfMessage(MailMessage mm)
        {
            var currentDateFormattedForEmail = DateTime.Now.ToString("ddd dd MMM yyyy HH:mm:ss zzz");
            var messageString = string.Format(@"Date: {0}
From: {1}
To: {2}
Reply-To: {3}
CC: {4}
Bcc: {5}
Subject: {6}

{7}", currentDateFormattedForEmail, mm.From, FlattenMailAddresses(mm.To), FlattenMailAddresses(mm.ReplyToList), FlattenMailAddresses(mm.CC), FlattenMailAddresses(mm.Bcc), mm.Subject, mm.Body);

            return messageString;
        }

        private static string FlattenMailAddresses(IEnumerable<MailAddress> addresses)
        {
            return String.Join("; ", addresses.Select(x => x.ToString()));
        }
    }
}
