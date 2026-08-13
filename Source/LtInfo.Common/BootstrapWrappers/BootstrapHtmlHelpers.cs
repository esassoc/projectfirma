/*-----------------------------------------------------------------------
<copyright file="BootstrapHtmlHelpers.cs" company="Environmental Science Associates">
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
using System.Collections.Generic;
using System.Web;

namespace LtInfo.Common.BootstrapWrappers
{
    public static class BootstrapHtmlHelpers
    {
        public static HtmlString MakeGlyphIcon(string glyphIconName)
        {
            return new HtmlString(string.Format("<span role=\"img\" title=\"{0}\" class=\"glyphicon {0}\"></span>", glyphIconName));
        }

        public static HtmlString MakeGlyphIconWithHiddenText(string glyphIconName, string text)
        {
            return new HtmlString(string.Format("<span role=\"img\" title=\"{0}\"  class=\"glyphicon {0}\"></span><span style='display:none'>{1}</span>", glyphIconName, HttpUtility.HtmlEncode(text)));
        }

        public static HtmlString MakeGlyphIconWithScreenReaderOnlyText(string glyphIconName, string screenReaderOnlyText)
        {
            return new HtmlString(string.Format("<span role=\"img\" title=\"{0}\"  class=\"glyphicon {0}\"></span><span class=\"sr-only\">{1}</span>", glyphIconName, HttpUtility.HtmlEncode(screenReaderOnlyText)));
        }

        public static HtmlString MakeGlyphIcon(string glyphIconName, string title)
        {
            return new HtmlString(string.Format("<span role=\"img\"  title=\"{0}\" class=\"glyphicon {1}\"></span>", HttpUtility.HtmlAttributeEncode(title), glyphIconName));
        }

        //TODO: This is fragile when quotes or apostrophes are passed in to any of the string parameters.
        public static HtmlString MakeModalDialogAlertButton(string alertText, string alertTitle, string closeButtonText, string linkText, List<string> cssClasses)
        {
            if (cssClasses == null)
                cssClasses = new List<string>();

            cssClasses.Add("btn");
            return MakeModalDialogAlertLink(alertText, alertTitle, closeButtonText, linkText, cssClasses);
        }

        public static HtmlString MakeModalDialogAlertLink(string alertText, string alertTitle, string closeButtonText, string linkText, List<string> cssClasses)
        {
            return
                new HtmlString(string.Format("<a href=\"javascript:void(0)\" class=\"{0}\" onclick=\"createBootstrapAlert({1}, {2}, {3})\">{4}</a>",
                    string.Join(" ", cssClasses),
                    alertText.ToHTMLFormattedString().ToString().ToJS(),
                    alertTitle.ToHTMLFormattedString().ToString().ToJS(),
                    closeButtonText.ToHTMLFormattedString().ToString().ToJS(),
                    linkText.ToHTMLFormattedString()));
        }

        public static HtmlString MakeModalDialogAlertLinkFromUrl(string url, string alertTitle, string closeButtonText, string linkText, List<string> cssClasses, string onJavascriptReadyFunction)
        {
            var javascripReadyFunctionAsParameter = !string.IsNullOrWhiteSpace(onJavascriptReadyFunction) ? string.Format("function() {{{0}();}}", onJavascriptReadyFunction) : "null";
            return
                new HtmlString(string.Format("<a href=\"{0}\" class=\"{1}\" onclick=\"return createBootstrapAlertFromUrl(this, {2}, {3}, {4});\">{5}</a>",
                    url,
                    string.Join(" ", cssClasses),
                    alertTitle.ToHTMLFormattedString().ToString().ToJS(),
                    closeButtonText.ToHTMLFormattedString().ToString().ToJS(),
                    javascripReadyFunctionAsParameter,
                    linkText.ToHTMLFormattedString()));
        }

        public static string RequiredIcon = "<span class=\"requiredFieldIcon glyphicon glyphicon-flash\" style=\"color: #800020; font-size: 8px; \"></span>";
    }
}
