/*-----------------------------------------------------------------------
<copyright file="LayerGeoJson.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using System.Drawing;
using System.Linq;
using GeoJSON.Net.Feature;
using LtInfo.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProjectFirma.Web.Models;

namespace ProjectFirmaModels.Models
{
    /// <summary>
    /// example: Jurisdiction layer or Road layer
    /// </summary>
    public class LayerGeoJson
    {
        public string LayerName { get; }
        public FeatureCollection GeoJsonFeatureCollection { get; }
        public string MapServerUrl { get; }
        public string MapServerLayerName { get; }
        public string TooltipUrlTemplate { get; }
        public string LayerColor { get; }
        public decimal LayerOpacity { get; }
        public LayerInitialVisibility.LayerInitialVisibilityEnum LayerInitialVisibility { get; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LayerGeoJsonType LayerType { get; }
        public string LayerIconImageLocation { get; }
        public bool HasCustomPopups { get; }

        /// <summary>
        /// Placement of this layer in the map layer control, for tenants that have configured layer
        /// grouping. Null for everyone else. Set after construction by whatever knows the source row.
        /// </summary>
        public string MapLayerGroupName { get; set; }
        public int? SortOrder { get; set; }

        /// <summary>
        /// Constructor for LayerGeoJson with Vector Type
        /// </summary>
        public LayerGeoJson(string layerName, FeatureCollection geoJsonFeatureCollection, string layerColor, decimal layerOpacity, LayerInitialVisibility.LayerInitialVisibilityEnum layerInitialVisibility)
        {
            LayerName = layerName;
            GeoJsonFeatureCollection = geoJsonFeatureCollection;
            LayerColor = layerColor.StartsWith("#") ? layerColor : GetColorString(layerColor);
            LayerOpacity = layerOpacity;
            LayerInitialVisibility = layerInitialVisibility;
            LayerType = LayerGeoJsonType.Vector;
            HasCustomPopups = geoJsonFeatureCollection.Features.Any(x => x.Properties.ContainsKey("PopupUrl"));
        }
        /// <summary>
        /// Constructor for LayerGeoJson with WMS Type
        /// </summary>
        public LayerGeoJson(string layerName, string mapServerUrl, string mapServerLayerName, string tooltipUrlTemplate, string layerColor, decimal layerOpacity, LayerInitialVisibility.LayerInitialVisibilityEnum layerInitialVisibility)
        {
            LayerName = layerName;
            MapServerUrl = mapServerUrl;
            MapServerLayerName = mapServerLayerName;
            TooltipUrlTemplate = tooltipUrlTemplate;
            LayerColor = layerColor;
            LayerOpacity = layerOpacity;
            LayerInitialVisibility = layerInitialVisibility;
            LayerType = LayerGeoJsonType.Wms;
        }

        /// <summary>
        /// Constructor for LayerGeoJson with WMS Type
        /// </summary>
        public LayerGeoJson(string layerName, string mapServerUrl, string mapServerLayerName, string layerColor, decimal layerOpacity, LayerInitialVisibility.LayerInitialVisibilityEnum layerInitialVisibility, string mapLayerIconImageLocation)
        {
            LayerName = layerName;
            MapServerUrl = mapServerUrl;
            MapServerLayerName = mapServerLayerName;
            LayerColor = layerColor;
            LayerOpacity = layerOpacity;
            LayerInitialVisibility = layerInitialVisibility;
            LayerType = LayerGeoJsonType.Wms;
            LayerIconImageLocation = mapLayerIconImageLocation;
        }

        public string GetGeoJsonFeatureCollectionAsJsonString()
        {
            return JsonTools.SerializeObject(GeoJsonFeatureCollection);
        }

        private static string GetColorString(string colorName)
        {
            var color = Color.FromName(colorName);
            return $"#{color.R:x2}{color.G:x2}{color.B:x2}";
        }
    }
}
