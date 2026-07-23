/*-----------------------------------------------------------------------
<copyright file="ProjectLocationsMapInitJson.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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

using ProjectFirma.Web.Common;
using ProjectFirma.Web.Models;
using ProjectFirmaModels.Models;
using ProjectFirma.Web.Views.Shared.ProjectLocationControls;

namespace ProjectFirma.Web.Views.Map
{
    public class ProjectLocationsMapInitJson : MapInitJson
    {
        private const int DefaultZoomLevel = 10;
        public ProjectMapCustomization ProjectMapCustomization { get; }
        public LayerGeoJson ProjectLocationsLayerGeoJson { get; }

        public ProjectLocationsMapInitJson(LayerGeoJson projectLocationsLayerGeoJson, ProjectMapCustomization customization, string mapDivID, bool isFullProjectMap)
            : base(mapDivID, DefaultZoomLevel, 
                isFullProjectMap? GetAllGeospatialAreaMapLayersAndProjectDetailedLocationsLayerForFullProjectMap() : GetConfiguredGeospatialAreaMapLayersAndProjectDetailedLocationsLayer(), 
                isFullProjectMap ? GetExternalMapLayerSimplesForFullProjectMap() : GetExternalMapLayerSimples(), BoundingBox.MakeNewDefaultBoundingBox())
        {
            ProjectMapCustomization = customization;
            ProjectLocationsLayerGeoJson = projectLocationsLayerGeoJson;
            // PF-2829: zoom-based disclosure of detailed location polygons only applies to the full project map,
            // where the all-projects detailed locations layer matches the projects being shown
            AutoDisplayProjectDetailedLocationsByZoom = isFullProjectMap && MultiTenantHelpers.EnableDetailedLocationPolygonsOnProjectMap();
        }
    }
}
