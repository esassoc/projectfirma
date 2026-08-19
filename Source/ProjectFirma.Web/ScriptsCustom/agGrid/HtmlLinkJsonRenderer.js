// Escapes a string for safe use as HTML text content or inside a double-quoted attribute.
// DisplayText/Link come from user-entered data (person names, project names, etc.), so they must
// never be interpolated into markup unescaped. Opt out only via DisplayTextIsHtml, which callers
// set from C# when the display text is trusted markup they built themselves (glyph icons, etc.).
function htmlLinkEscapeHtml(str) {
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');
}

function HtmlLinkJsonRenderer(params) {
    if (!params.value) {
        return "";
    }
    var jsonObj = JSON.parse(params.value);

    // Trusted markup only when the server explicitly says so; otherwise escape.
    var displayHtml = jsonObj.DisplayTextIsHtml
        ? jsonObj.DisplayText
        : htmlLinkEscapeHtml(jsonObj.DisplayText);

    // Decoration (e.g. a map legend color swatch) the server built and kept out of DisplayText so that sorting,
    // filtering and export see only the plain text. Trusted markup by contract; see HtmlLinkObject.WithHtmlPrefix.
    var prefixHtml = jsonObj.DisplayHtmlPrefix || "";

    if (jsonObj.Link && jsonObj.DisplayText) {
        // Derive a plain-text aria-label from the DisplayText (strip any HTML such as leading <span>)
        var tmp = document.createElement('div');
        tmp.innerHTML = displayHtml;
        var ariaText = tmp.textContent || tmp.innerText || '';

        var ariaLabelValue = htmlLinkEscapeHtml(ariaText);

        return `<a href="${htmlLinkEscapeHtml(jsonObj.Link)}" tabindex="0" aria-label="${ariaLabelValue}" class="ag-grid-link">${prefixHtml}${displayHtml}</a>`;
    } else if (jsonObj.DisplayText) {
        return prefixHtml + displayHtml;
    }
    return "";
}


function HtmlLinkJsonFormatter(params) {
    if (!params.value) {
        return "";
    }
    var jsonObj = JSON.parse(params.value);
    if (jsonObj.DisplayText) {
        return jsonObj.DisplayText;
    }
    return "";
}
