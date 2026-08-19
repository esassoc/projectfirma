/*-----------------------------------------------------------------------
<copyright file="HtmlLinkObject.cs" company="Environmental Science Associates">
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
using System.Web.Helpers;

namespace LtInfo.Common.AgGridWrappers
{
    public class HtmlLinkObject
    {
        public string DisplayText { get; set; }

        public string Link { get; set; }

        /// <summary>
        /// When false (the default), the ag-grid renderers HTML-escape <see cref="DisplayText"/> before putting it in the DOM.
        /// Set this only via <see cref="WithHtmlDisplayText"/>, for display text that is trusted markup built in our own code.
        /// Note that <see cref="DisplayText"/> is also used for grid sorting/filtering/export, so it is deliberately stored
        /// unencoded here and escaped at the point it becomes HTML.
        /// </summary>
        public bool DisplayTextIsHtml { get; set; }

        /// <summary>
        /// Optional trusted markup the ag-grid renderers put immediately before <see cref="DisplayText"/> in the cell,
        /// for decoration such as a map legend color swatch. It is deliberately kept out of <see cref="DisplayText"/>
        /// so sorting, filtering and CSV export still see only the plain text. Set this only via
        /// <see cref="WithHtmlPrefix"/>; any user-entered data embedded in it must already be HTML encoded.
        /// </summary>
        public string DisplayHtmlPrefix { get; set; }

        public HtmlLinkObject(string displayText, string link)
        {
            DisplayText = displayText;
            Link = link;
            DisplayTextIsHtml = false;
        }

        /// <summary>
        /// For links whose display text is trusted markup (glyph icons, etc.). Any user-entered data embedded in
        /// <paramref name="displayTextHtml"/> must already be HTML encoded by the caller.
        /// </summary>
        public static HtmlLinkObject WithHtmlDisplayText(string displayTextHtml, string link)
        {
            return new HtmlLinkObject(displayTextHtml, link) { DisplayTextIsHtml = true };
        }

        /// <summary>
        /// For links that need decorative markup (a color swatch, an icon) ahead of otherwise plain display text.
        /// <paramref name="displayText"/> stays plain so it drives sorting, filtering and export; only
        /// <paramref name="displayHtmlPrefix"/> is emitted as markup, and any user-entered data inside it must
        /// already be HTML encoded by the caller.
        /// </summary>
        public static HtmlLinkObject WithHtmlPrefix(string displayHtmlPrefix, string displayText, string link)
        {
            return new HtmlLinkObject(displayText, link) { DisplayHtmlPrefix = displayHtmlPrefix };
        }
    }

    public static class HtmlLinkObjectModelExtensions
    {
        public static string ToJsonObjectForAgGrid(this HtmlLinkObject htmlLinkObject)
        {
            return Json.Encode(htmlLinkObject);
        }

        public static string ToJsonArrayForAgGrid(this IEnumerable<HtmlLinkObject> htmlLinkObjects)
        {
            return Json.Encode(htmlLinkObjects);
        }

    }
}
