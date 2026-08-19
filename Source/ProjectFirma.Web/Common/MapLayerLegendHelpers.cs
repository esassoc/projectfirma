/*-----------------------------------------------------------------------
<copyright file="MapLayerLegendHelpers.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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

using System.Web;
using ProjectFirma.Web.Models;
using ProjectFirmaModels.Models;

namespace ProjectFirma.Web.Common
{
    /// <summary>
    /// Builds the label a geospatial area layer is registered under in the Leaflet layer control. Leaflet
    /// writes the label through innerHTML, so the label may be markup -- which is how a layer's legend gets in
    /// front of the user. The same string is also the key the layer is stored under in the overlay object and
    /// the key <see cref="MapLayerGroupConfig"/> is matched on, so every caller has to build it the same way;
    /// that is what this helper is for.
    /// </summary>
    public static class MapLayerLegendHelpers
    {
        /// <summary>
        /// The one layer with a hand written legend: TCS Project Tracker's WUI Zone WMS layer. Matched on the
        /// GeoServer layer name because that carries the tenant's namespace, so it cannot collide with another
        /// tenant, and because it is the layer's stable identity rather than its editable display name.
        /// </summary>
        private const string WuiZoneGeoServerLayerName = "TCSProjectTracker:WuiZone";

        /// <summary>
        /// The WUI Zone legend, hand written to match the GeoServer style for
        /// <see cref="WuiZoneGeoServerLayerName"/>. Label, fill and stroke per row; the colors are the ones
        /// that style's GetLegendGraphic draws, so if the style changes these have to change with it.
        /// </summary>
        private static readonly string[,] WuiZoneLegendRows =
        {
            { "WUI_Defense", "#FFDFD6", "#FF5F32" },
            { "WUI_Threat", "#FFF6CC", "#FFD000" },
            { "Not_in_a_WUI_Zone", "#EEF7FF", "#59ACFF" }
        };

        public static string GetGeospatialAreaLayerControlLabel(GeospatialAreaType geospatialAreaType)
        {
            if (geospatialAreaType.GeospatialAreaLayerName == WuiZoneGeoServerLayerName)
            {
                return GetLabelWithHardCodedLegend(geospatialAreaType.GeospatialAreaTypeNamePluralized, WuiZoneLegendRows);
            }

            // Every other legend is an uploaded image, inlined next to the layer name as it always has been.
            if (geospatialAreaType.MapLegendImageFileResourceInfoID.HasValue)
            {
                return $"<span>{geospatialAreaType.GeospatialAreaTypeNamePluralized} <img src='{geospatialAreaType.MapLegendImageFileResourceInfo.GetFileResourceUrl()}' height='20px' /></span>";
            }

            return geospatialAreaType.GeospatialAreaTypeNamePluralized;
        }

        /// <summary>
        /// The layer name, a question mark icon, and the legend itself, hidden until the icon is hovered or
        /// focused. The icon carries the same helpicon and glyphicon classes as the field definition help icon,
        /// so it picks up each tenant's theme color rather than pinning one here; a button rather than an anchor
        /// so bootstrap's a:hover does not override that color on hover. Showing and hiding is pure CSS -- see the map-layer-legend rules in
        /// MapJavascriptIncludes.cshtml -- so there is no script involved, and the legend goes away on its own
        /// when the pointer leaves the layer control and Leaflet collapses it.
        /// </summary>
        private static string GetLabelWithHardCodedLegend(string layerName, string[,] legendRows)
        {
            var encodedLayerName = HttpUtility.HtmlEncode(layerName);
            // No title attribute: the browser's native tooltip for it renders on top of the legend the icon
            // just opened. aria-label carries the same text for screen readers without drawing anything.
            var encodedLabel = HttpUtility.HtmlAttributeEncode($"{layerName} legend");

            var rowsHtml = string.Empty;
            for (var i = 0; i < legendRows.GetLength(0); ++i)
            {
                var label = HttpUtility.HtmlEncode(legendRows[i, 0]);
                var fill = HttpUtility.HtmlAttributeEncode(legendRows[i, 1]);
                var stroke = HttpUtility.HtmlAttributeEncode(legendRows[i, 2]);
                rowsHtml += $"<span class='map-layer-legend-row'><span class='map-layer-legend-swatch' style='background:{fill}; border-color:{stroke};'></span>{label}</span>";
            }

            return $"<span class='map-layer-legend-wrapper'>{encodedLayerName} "
                   + $"<button type='button' class='helpicon glyphicon glyphicon-question-sign map-layer-legend-icon' aria-label='{encodedLabel}' onclick='return false;'></button>"
                   + $"<span class='map-layer-legend'>{rowsHtml}</span></span>";
        }
    }
}
