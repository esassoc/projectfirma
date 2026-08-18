/*-----------------------------------------------------------------------
<copyright file="ExternalMapLayerSimple.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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

using ProjectFirmaModels.Models;

namespace ProjectFirma.Web.Models
{
    public  class ExternalMapLayerSimple 
    {
        public int ExternalMapLayerID { get; set; }
        public int TenantID { get; set; }
        public string DisplayName { get; set; }
        public string LayerUrl { get; set; }
        public string LayerDescription { get; set; }
        public string FeatureNameField { get; set; }
        public bool DisplayOnAllProjectMaps { get; set; }
        public bool LayerIsOnByDefault { get; set; }
        public bool IsActive { get; set; }
        public bool IsTiledMapService { get; set; }
        public int? MapLegendImageFileResourceInfoID { get; set; }
        public string MapLayerGroupName { get; set; }
        public int? SortOrder { get; set; }

        public ExternalMapLayerSimple(ExternalMapLayer externalMapLayer)
        {
            MapLayerGroupName = externalMapLayer.MapLayerGroupName;
            SortOrder = externalMapLayer.SortOrder;
            ExternalMapLayerID = externalMapLayer.ExternalMapLayerID;
            TenantID = externalMapLayer.TenantID;
            DisplayName = externalMapLayer.MapLegendImageFileResourceInfoID.HasValue ? $"<span>{externalMapLayer.DisplayName} <img src='{externalMapLayer.MapLegendImageFileResourceInfo.GetFileResourceUrl()}' height='20px' /></span>" : externalMapLayer.DisplayName;
            LayerUrl = externalMapLayer.LayerUrl;
            LayerDescription = externalMapLayer.LayerDescription;
            FeatureNameField = externalMapLayer.FeatureNameField;
            DisplayOnAllProjectMaps = externalMapLayer.DisplayOnAllProjectMaps;
            LayerIsOnByDefault = externalMapLayer.LayerIsOnByDefault;
            IsActive = externalMapLayer.IsActive;
            IsTiledMapService = externalMapLayer.IsTiledMapService;
            MapLegendImageFileResourceInfoID = externalMapLayer.MapLegendImageFileResourceInfoID;
        }
    }
}
