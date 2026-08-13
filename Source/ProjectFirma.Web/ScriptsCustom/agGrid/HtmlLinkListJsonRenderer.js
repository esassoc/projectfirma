


function HtmlLinkListJsonRenderer(params) {

    if (!params.value) {
        return "";
    }

    var jsonObj = JSON.parse(params.value);
    var returnString = "";
    for (var i = 0; i < jsonObj.length; i++) {
        if (i > 0) {
            returnString += ", ";
        }
        var item = jsonObj[i];
        // See HtmlLinkJsonRenderer.js for htmlLinkEscapeHtml; DisplayText/Link are user-entered data.
        var displayHtml = item.DisplayTextIsHtml ? item.DisplayText : htmlLinkEscapeHtml(item.DisplayText);
        if (item.Link && item.DisplayText) {
            returnString += `<a href="${htmlLinkEscapeHtml(item.Link)}">${displayHtml}</a>`;
        } else if (item.DisplayText) {
            returnString += displayHtml;
        }
    }


    return returnString;
}


function HtmlLinkListJsonFormatter(params) {

    if (!params.value) {
        return "";
    }

    var jsonObj = JSON.parse(params.value);
    //console.log(jsonObj);
    var returnString = "";
    for (var i = 0; i < jsonObj.length; i++) {
        if (i > 0) {
            returnString += ", ";
        }
        var item = jsonObj[i];
        if (item.DisplayText) {
            returnString += item.DisplayText;
        }
    }

    return returnString;
}
