/*-----------------------------------------------------------------------
<copyright file="MapLayerGroupConfig.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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

namespace ProjectFirma.Web.Models
{
    /// <summary>
    /// Placement of a single layer within the map layer control, keyed by the name the layer is
    /// registered under. A tenant with no configured groups sends an empty list, which is the signal
    /// for the map to keep using the ungrouped, insertion-ordered layer control.
    /// </summary>
    public class MapLayerGroupConfig
    {
        public string LayerName { get; set; }
        public string MapLayerGroupName { get; set; }
        public int? SortOrder { get; set; }

        public MapLayerGroupConfig(string layerName, string mapLayerGroupName, int? sortOrder)
        {
            LayerName = layerName;
            MapLayerGroupName = mapLayerGroupName;
            SortOrder = sortOrder;
        }
    }
}
