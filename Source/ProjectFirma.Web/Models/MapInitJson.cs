/*-----------------------------------------------------------------------
<copyright file="MapInitJson.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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

using GeoJSON.Net.Feature;
using LtInfo.Common.GeoJson;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Controllers;
using ProjectFirmaModels.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProjectFirma.Web.Models
{
    public class MapInitJson
    {
        public string MapDivID;
        public BoundingBox BoundingBox;
        public int ZoomLevel;
        public List<LayerGeoJson> Layers;
        public List<ExternalMapLayerSimple> ExternalMapLayerSimples;
        public readonly bool TurnOnFeatureIdentify;
        public bool AllowFullScreen = true;
        public bool DisablePopups = false;
        public string RequestSupportUrl;
        public string MapServiceUrl;
        public string ProjectDetailedLocationsPublicApprovedGeoServerLayerName;
        public string ProjectFieldDefinitionLabel;
        public bool AutoDisplayProjectDetailedLocationsByZoom;
        public List<MapLayerGroupConfig> MapLayerGroupConfigs;

        /// <summary>
        /// Name the hardcoded street labels overlay is registered under in ProjectFirmaMaps.js.
        /// </summary>
        public const string StreetLabelsLayerName = "Street Labels";

        // Sort orders for the three layers that are not backed by a database row. They lead their groups,
        // ahead of the row-backed layers, whose SortOrder values start after these.
        private const int MappedProjectsSortOrder = 1;
        private const int ProjectDetailedLocationsSortOrder = 2;
        private const int StreetLabelsSortOrder = 3;

        public MapInitJson(string mapDivID, int zoomLevel, List<LayerGeoJson> layers, List<ExternalMapLayerSimple> externalMapLayers, BoundingBox boundingBox, bool turnOnFeatureIdentify)
        {
            MapDivID = mapDivID;
            ZoomLevel = zoomLevel;
            Layers = layers;
            ExternalMapLayerSimples = externalMapLayers;
            BoundingBox = boundingBox;
            TurnOnFeatureIdentify = turnOnFeatureIdentify;
            RequestSupportUrl = SitkaRoute<HelpController>.BuildUrlFromExpression(x => x.RequestSupport());
            MapServiceUrl = MultiTenantHelpers.MapServiceUrl();
            ProjectDetailedLocationsPublicApprovedGeoServerLayerName =
                $"{MultiTenantHelpers.GetTenantAttributeFromCache().GeoServerNamespace}:ProjectDetailedLocationsPublicApproved";
            ProjectFieldDefinitionLabel = FieldDefinitionEnum.Project.ToType().GetFieldDefinitionLabel();
            MapLayerGroupConfigs = BuildMapLayerGroupConfigs(layers, externalMapLayers);
        }

        /// <summary>
        /// Where each layer sits in the layer control, for tenants that have configured grouping. Returns
        /// an empty list when nothing is configured, which keeps the map on the ungrouped layer control.
        /// </summary>
        private static List<MapLayerGroupConfig> BuildMapLayerGroupConfigs(List<LayerGeoJson> layers, List<ExternalMapLayerSimple> externalMapLayers)
        {
            var tenantAttribute = MultiTenantHelpers.GetTenantAttributeFromCache();
            var mapLayerGroupConfigs = new List<MapLayerGroupConfig>();

            if (!string.IsNullOrWhiteSpace(tenantAttribute.ProjectDataMapLayerGroupName))
            {
                mapLayerGroupConfigs.Add(new MapLayerGroupConfig(GetMappedProjectsLayerName(), tenantAttribute.ProjectDataMapLayerGroupName, MappedProjectsSortOrder));
                mapLayerGroupConfigs.Add(new MapLayerGroupConfig(GetProjectDetailedLocationsLayer().LayerName, tenantAttribute.ProjectDataMapLayerGroupName, ProjectDetailedLocationsSortOrder));
            }

            if (!string.IsNullOrWhiteSpace(tenantAttribute.ReferenceMapLayerGroupName))
            {
                mapLayerGroupConfigs.Add(new MapLayerGroupConfig(StreetLabelsLayerName, tenantAttribute.ReferenceMapLayerGroupName, StreetLabelsSortOrder));
            }

            if (layers != null)
            {
                mapLayerGroupConfigs.AddRange(layers.Where(x => !string.IsNullOrWhiteSpace(x.MapLayerGroupName))
                    .Select(x => new MapLayerGroupConfig(x.LayerName, x.MapLayerGroupName, x.SortOrder)));
            }

            if (externalMapLayers != null)
            {
                mapLayerGroupConfigs.AddRange(externalMapLayers.Where(x => !string.IsNullOrWhiteSpace(x.MapLayerGroupName))
                    .Select(x => new MapLayerGroupConfig(x.DisplayName, x.MapLayerGroupName, x.SortOrder)));
            }

            return mapLayerGroupConfigs;
        }

        /// <summary>
        /// Must match the name the project locations layer is added to the control under in
        /// ProjectLocationsMap.cshtml.
        /// </summary>
        public static string GetMappedProjectsLayerName()
        {
            return $"Mapped {FieldDefinitionEnum.Project.ToType().GetFieldDefinitionLabelPluralized()}";
        }

        /// <summary>
        /// Summary maps with no editing should use this constructor
        /// </summary>
        public MapInitJson(string mapDivID, int zoomLevel, List<LayerGeoJson> layers, List<ExternalMapLayerSimple> externalMapLayers, BoundingBox boundingBox) : this(mapDivID, zoomLevel, layers, externalMapLayers, boundingBox, true)
        {
        }

        public static List<ExternalMapLayerSimple> GetExternalMapLayerSimples()
        {
            return HttpRequestStorage.DatabaseEntities.ExternalMapLayers.Where(x => x.IsActive && x.DisplayOnAllProjectMaps).ToList().Select(x => new ExternalMapLayerSimple(x)).ToList();
        }

        public static List<ExternalMapLayerSimple> GetExternalMapLayerSimplesForFullProjectMap()
        {
            return HttpRequestStorage.DatabaseEntities.ExternalMapLayers.Where(x => x.IsActive).ToList().Select(x => new ExternalMapLayerSimple(x)).ToList();
        }

        public static List<LayerGeoJson> GetConfiguredGeospatialAreaMapLayersAndProjectDetailedLocationsLayer(bool alwaysHideLayers = false)
        {
            var geospatialAreaTypes = HttpRequestStorage.DatabaseEntities.GeospatialAreaTypes.Where(x => x.DisplayOnAllProjectMaps).OrderBy(x => x.GeospatialAreaTypeName)
                .ToList();
            var layerGeoJsons = new List<LayerGeoJson>();
            foreach (var geospatialAreaType in geospatialAreaTypes.Where(x => x.DisplayOnAllProjectMaps))
            {
                layerGeoJsons.Add(geospatialAreaType.GetGeospatialAreaWmsLayerGeoJson("#59ACFF", 0.2m,
                    alwaysHideLayers || !geospatialAreaType.OnByDefaultOnOtherMaps ? LayerInitialVisibility.LayerInitialVisibilityEnum.Hide : LayerInitialVisibility.LayerInitialVisibilityEnum.Show));
            }
            layerGeoJsons.Add(GetProjectDetailedLocationsLayer());
            return layerGeoJsons;
        }

        public static List<LayerGeoJson> GetAllGeospatialAreaMapLayersAndProjectDetailedLocationsLayerForFullProjectMap()
        {
            var layerGeoJsons = GetAllGeospatialAreaMapLayersForFullProjectMap();
            layerGeoJsons.Add(GetProjectDetailedLocationsLayer());
            return layerGeoJsons;

        }

        public static List<LayerGeoJson> GetAllGeospatialAreaMapLayersForFullProjectMap()
        {
            var geospatialAreaTypes = HttpRequestStorage.DatabaseEntities.GeospatialAreaTypes.OrderBy(x => x.GeospatialAreaTypeName)
                .ToList();
            var layerGeoJsons = new List<LayerGeoJson>();
            foreach (var geospatialAreaType in geospatialAreaTypes)
            {
                layerGeoJsons.Add(geospatialAreaType.GetGeospatialAreaWmsLayerGeoJson("#59ACFF", 0.2m,
                    LayerInitialVisibility.GetInitialVisibility(geospatialAreaType.OnByDefaultOnProjectMap)));
            }

            return layerGeoJsons;
        }

        public static LayerGeoJson GetProjectDetailedLocationsLayer()
        {
            return new LayerGeoJson($"{FieldDefinitionEnum.ProjectLocation.ToType().GetFieldDefinitionLabelPluralized()} - Detail", MultiTenantHelpers.MapServiceUrl(),
                "ProjectDetailedLocationsPublicApproved", "#59ACFF", .2m,
                LayerInitialVisibility.LayerInitialVisibilityEnum.Hide, "");
        }

        // This is used when viewing a single geospatial area type so it should always by on by default, regardless of GeospatialAreaType map settings
        public static List<LayerGeoJson> GetGeospatialAreaMapLayersForGeospatialAreaType(GeospatialAreaType geospatialAreaType)
        {
            var layerGeoJsons = new List<LayerGeoJson>
            {
                geospatialAreaType.GetGeospatialAreaWmsLayerGeoJson("#59ACFF", 0.2m,
                    LayerInitialVisibility.LayerInitialVisibilityEnum.Show)
            };

            return layerGeoJsons;
        }

        public static List<LayerGeoJson> GetProjectLocationSimpleMapLayer(IProject project, bool userCanViewPrivateLocations)
        {
            var layerGeoJsons = new List<LayerGeoJson>();
            if (project.HasProjectLocationPoint(userCanViewPrivateLocations))
            {
                layerGeoJsons.Add(new LayerGeoJson($"{FieldDefinitionEnum.ProjectLocation.ToType().GetFieldDefinitionLabel()} - Simple",
                    new FeatureCollection(new List<Feature>
                    {
                        DbGeometryToGeoJsonHelper.FromDbGeometry(project.GetProjectLocationPoint(userCanViewPrivateLocations))
                    }),
                    "#838383", 1, LayerInitialVisibility.LayerInitialVisibilityEnum.Show));
            }
            return layerGeoJsons;
        }

        public static List<LayerGeoJson> GetProjectLocationSimpleAndDetailedMapLayers(ProjectUpdate projectUpdate, FirmaSession currentFirmaSession)
        {
            var layerGeoJsons = new List<LayerGeoJson>();
            var userCanViewPrivateLocations = currentFirmaSession.UserCanViewPrivateLocations(projectUpdate);
            if (projectUpdate.HasProjectLocationPoint(userCanViewPrivateLocations) )
            {
                layerGeoJsons.AddRange(GetProjectLocationSimpleMapLayer(projectUpdate, currentFirmaSession.UserCanViewPrivateLocations(projectUpdate)));                
            }
            var detailedLocationGeoJsonFeatureCollection = projectUpdate.DetailedLocationToGeoJsonFeatureCollection(userCanViewPrivateLocations);
            if (detailedLocationGeoJsonFeatureCollection.Features.Any())
            {
                layerGeoJsons.Add(new LayerGeoJson($"{FieldDefinitionEnum.ProjectLocation.ToType().GetFieldDefinitionLabel()} - Detail", detailedLocationGeoJsonFeatureCollection, "#838383", 1, LayerInitialVisibility.LayerInitialVisibilityEnum.Show));
            }
            return layerGeoJsons;
        }

        public static List<LayerGeoJson> GetProjectLocationSimpleAndDetailedMapLayers(Project project, FirmaSession currentFirmaSession)
        {
            var layerGeoJsons = new List<LayerGeoJson>();
            var userCanViewPrivateLocations = currentFirmaSession.UserCanViewPrivateLocations(project);

            if (project.HasProjectLocationPoint(userCanViewPrivateLocations))
            {
                layerGeoJsons.AddRange(GetProjectLocationSimpleMapLayer(project, userCanViewPrivateLocations));                
            }
            var detailedLocationGeoJsonFeatureCollection = project.DetailedLocationToGeoJsonFeatureCollection(userCanViewPrivateLocations);
            if (detailedLocationGeoJsonFeatureCollection.Features.Any())
            {
                layerGeoJsons.Add(new LayerGeoJson($"{FieldDefinitionEnum.ProjectLocation.ToType().GetFieldDefinitionLabel()} - Detail", detailedLocationGeoJsonFeatureCollection, "#838383", 1, LayerInitialVisibility.LayerInitialVisibilityEnum.Show));
            }
            layerGeoJsons.Add(GetProjectDetailedLocationsLayer());
            return layerGeoJsons;
        }
    }
}
