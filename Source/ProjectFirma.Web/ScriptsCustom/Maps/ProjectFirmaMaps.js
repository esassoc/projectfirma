/*-----------------------------------------------------------------------
/*-----------------------------------------------------------------------
<copyright file="ProjectFirmaMaps.js" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
var ProjectFirmaMaps = {};
var highlightOverlay; //Used when highlighting all a Project's Detailed Locations; see formatGeospatialAreaResponse
var mapOutsideScope; //Used when highlighting all a Project's Detailed Locations; see formatGeospatialAreaResponse
var projectDetailedLocationsLayer;


/* ====== Main Map ====== */
ProjectFirmaMaps.Map = function (mapInitJson, initialBaseLayerShown)
{
    // Map initialization routine
    var firmaMap = this;
    this.MapDivId = mapInitJson.MapDivID;
    this.MapServiceUrl = mapInitJson.MapServiceUrl;
    this.ProjectDetailedLocationsPublicApprovedGeoServerLayerName = mapInitJson.ProjectDetailedLocationsPublicApprovedGeoServerLayerName;
    this.ProjectFieldDefinitionLabel = mapInitJson.ProjectFieldDefinitionLabel;
    this.AutoDisplayProjectDetailedLocationsByZoom = mapInitJson.AutoDisplayProjectDetailedLocationsByZoom;

    firmaMap.mapLayers = [];
    firmaMap.externalFeatureLayers = mapInitJson.ExternalMapLayerSimples.filter(function(x) {
        return !x.IsTiledMapService;
    });

    // Configure base maps
    var esriAerialUrl = 'https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}';
    var esriAerial = new L.TileLayer(esriAerialUrl, {});
    var esriStreetUrl = 'https://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}';
    var esriStreet = new L.TileLayer(esriStreetUrl, {});
    var esriTerrainUrl = 'https://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}';
    var esriTerrain = new L.TileLayer(esriTerrainUrl, {});
    var streetLabelsLayer = new L.TileLayer('https://services.arcgisonline.com/ArcGIS/rest/services/Reference/World_Transportation/MapServer/tile/{z}/{y}/{x}', {});
    var baseLayers = { 'Aerial': esriAerial, 'Street': esriStreet, 'Terrain': esriTerrain };
    var overlayLayers = { 'Street Labels': streetLabelsLayer };

    var streetLayerGroup;
    if (initialBaseLayerShown === "Hybrid")
    {
         streetLayerGroup = new L.LayerGroup();
        initialBaseLayerShown = "Aerial"; //Since switching to ArcGIS maps, there is no "Hybrid", so use "Aerial" instead.
        streetLabelsLayer.addTo(streetLayerGroup);
    }
    else if (Sitka.Methods.isUndefinedNullOrEmpty(initialBaseLayerShown) || baseLayers[initialBaseLayerShown] == null)
    {
        initialBaseLayerShown = "Terrain";
    } 
    if (streetLayerGroup != null)
    {
        streetLayerGroup.addTo(firmaMap.map);
    }

    // Map options
    var options = {
        scrollWheelZoom: false, // If this is on (default) scrolling down the page will intercept and starting zooming the map
        layers: [baseLayers[initialBaseLayerShown]],
        attributionControl: false,
        fullscreenControl: mapInitJson.AllowFullScreen ? { pseudoFullscreen: true } : false
    };

    ProjectFirmaMaps.Map.prototype.wmsParams = {
        service: "WMS",
        transparent: true,
        version: "1.1.1",
        format: "image/png",
        info_format: "application/json",
        tiled: true
    };

    ProjectFirmaMaps.Map.prototype.wfsParams = {
        service: "WFS",
        version: "2.0",
        request: "GetFeature",
        outputFormat: "application/json",
        SrsName: "EPSG:4326"
    };

    


    // Initialize the map
    firmaMap.map = L.map(firmaMap.MapDivId, options);
    // Hide map from accessibility tree and prevent keyboard access
    var mapDiv = document.getElementById(firmaMap.MapDivId);
    if (mapDiv) {
        mapDiv.setAttribute('aria-hidden', 'true');
        mapDiv.setAttribute('tabindex', '-1');
        mapDiv.addEventListener('keydown', function(e) { e.stopPropagation(); e.preventDefault(); }, true);
        mapDiv.addEventListener('keypress', function(e) { e.stopPropagation(); e.preventDefault(); }, true);
        mapDiv.addEventListener('keyup', function(e) { e.stopPropagation(); e.preventDefault(); }, true);
    }

    // Add layers and map controls

    mapOutsideScope = firmaMap.map;
    firmaMap.map.on("overlayremove", function (e) {
        if (highlightOverlay && e.name.includes("Locations - Detail"))
            highlightOverlay.remove(); //If Detailed Locations is toggled off, also remove the highlight
    });

    // Add external tile layers from ArcGIS Online
    for (var i = 0; i < mapInitJson.ExternalMapLayerSimples.length; ++i) {
        var layerConfig = mapInitJson.ExternalMapLayerSimples[i];
        if (layerConfig.IsTiledMapService) {
            firmaMap.addTiledLayerFromAGOL(layerConfig, overlayLayers);
        }
    }

    // Add external vector layers from ArcGIS Online 
    for (var i = 0; i < mapInitJson.ExternalMapLayerSimples.length; ++i) {
        var layerConfig = mapInitJson.ExternalMapLayerSimples[i];
        if (!layerConfig.IsTiledMapService) {
            firmaMap.addVectorLayerFromAGOL(layerConfig, overlayLayers);
        }
    }

    // Add main layers from geojson
    for (var i = 0; i < mapInitJson.Layers.length; ++i) {
        var currentLayer = mapInitJson.Layers[i];
        switch (currentLayer.LayerType) {
            case "Vector":
                firmaMap.addVectorLayer(currentLayer, overlayLayers);
                break;
            case "Wms":
                firmaMap.addWmsLayer(currentLayer, overlayLayers);
                break;
            default:
                console.error("Invalid LayerType " + currentLayer.LayerType + " not added to map layers.");
        }
    }

    firmaMap.addLayersToMapLayersControl(baseLayers, overlayLayers);

    var modalDialog = jQuery(".modal");
    if (!Sitka.Methods.isUndefinedNullOrEmpty(modalDialog))
    {
        modalDialog.on("shown.bs.modal", function()
        {
            firmaMap.map.invalidateSize();
            firmaMap.setMapBounds(mapInitJson);
        });
    }
    if (!mapInitJson.DisablePopups) {
        firmaMap.map.on("click", function (e) { firmaMap.getFeatureInfo(e); });
    }
    firmaMap.setMapBounds(mapInitJson);
}; // End of constructor

// Utility functions
ProjectFirmaMaps.Map.prototype.addLayersToMapLayersControl = function (baseLayers, overlayLayers) {
    this.layerControl = L.control.layers(baseLayers, overlayLayers);
    this.layerControl.addTo(this.map);
};

ProjectFirmaMaps.Map.prototype.setMapBounds = function (mapInitJson) {
    this.map.fitBounds([
        [mapInitJson.BoundingBox.Northeast.Latitude, mapInitJson.BoundingBox.Northeast.Longitude],
        [mapInitJson.BoundingBox.Southwest.Latitude, mapInitJson.BoundingBox.Southwest.Longitude]
    ]);
};

ProjectFirmaMaps.Map.prototype.removeLayerFromMap = function (layerToRemove) {
    if (!Sitka.Methods.isUndefinedNullOrEmpty(layerToRemove)) {
        this.map.removeLayer(layerToRemove);
    }
};

ProjectFirmaMaps.Map.prototype.addLayerToLayerControl = function (layer, layerName) {
    this.layerControl.addOverlay(layer, layerName);
};

ProjectFirmaMaps.Map.prototype.disableUserInteraction = function () {
    this.map.dragging.disable();
    this.map.touchZoom.disable();
    this.map.doubleClickZoom.disable();
    this.map.scrollWheelZoom.disable();
    this.map.boxZoom.disable();
    this.map.keyboard.disable();
    this.map.attributionControl = false;
    if (this.map.tap) this.map.tap.disable();
    document.getElementById(this.MapDivId).style.cursor = 'default';
    jQuery(".leaflet-control-zoom").css("visibility", "hidden");
    jQuery(".leaflet-control-layers").css("visibility", "hidden");
    this.removeClickEventHandler();
};

ProjectFirmaMaps.Map.prototype.blockMapImpl = function () {
    this.map.dragging.disable();
    this.map.scrollWheelZoom.disable();
    jQuery("#" + this.MapDivId).block(
        {
            message:
                "<span style='font-weight: bold; font-size: 14px; margin: 0 2px'>Location Not Specified</span>",
            css: {
                border: "none",
                cursor: "default"
            },
            overlayCSS: {
                backgroundColor: "#D3D3D3",
                cursor: "default"
            },
            baseZ: 999
        });
};

ProjectFirmaMaps.Map.prototype.blockMap = function () {
    var firmaMap = this;
    this.blockMapImpl();
    var modalDialog = jQuery(".modal");
    modalDialog.on("shown.bs.modal", function () { firmaMap.blockMapImpl(); });
};

ProjectFirmaMaps.Map.prototype.unblockMap = function () {
    this.map.dragging.enable();
    jQuery("#" + this.MapDivId).unblock();
};

// Functions for adding layers
//
// WMS Layers
ProjectFirmaMaps.Map.prototype.addWmsLayer = function (currentLayer, overlayLayers) {
    var layerGroup = new L.LayerGroup(),
        wmsParams = L.Util.extend(this.wmsParams, { layers: currentLayer.MapServerLayerName }),
        wmsLayer = L.tileLayer.wms(currentLayer.MapServerUrl, wmsParams).addTo(layerGroup);

    if (currentLayer.LayerInitialVisibility === 1) {
        layerGroup.addTo(this.map);
    }
    wmsLayer.on("click", function (e) { firmaMap.getFeatureInfo(e); });

    if (currentLayer.LayerName.includes("Locations - Detail")) {
        projectDetailedLocationsLayer = layerGroup
    }

    overlayLayers[currentLayer.LayerName] = layerGroup;
    this.mapLayers.push(wmsLayer);
};

// Vector Layers
ProjectFirmaMaps.Map.prototype.addVectorLayer = function (currentLayer, overlayLayers) {
    var firmaMap = this;

    var layerGroup = new L.LayerGroup();
    var layerGeoJson = L.geoJson(currentLayer.GeoJsonFeatureCollection, {
        pointToLayer: function (feature, latlng) {
            var featureColor = feature.properties.FeatureColor == null ? currentLayer.LayerColor : feature.properties.FeatureColor;
            var marker = L.marker(latlng, { icon: L.MakiMarkers.icon({ icon: "marker", color: featureColor, size: "s", alt: "Location" }) });
            return marker;
        },
        style: function (feature) {
            var fillPolygonByDefault = _.includes(["Polygon", "MultiPolygon"], feature.geometry.type);
            return {
                color: feature.properties.FeatureColor == null ? currentLayer.LayerColor : feature.properties.FeatureColor,
                weight: feature.properties.FeatureWeight == null ? 2 : feature.properties.FeatureWeight,
                fill: feature.properties.FillPolygon == null ? fillPolygonByDefault : feature.properties.FillPolygon,
                fillOpacity: feature.properties.FillOpacity == null ? 0.2 : feature.properties.FillOpacity
            };
        },
        onEachFeature: function (feature, layer) {
            firmaMap.bindPopupToFeature(layer, feature);
            firmaMap.bindHoverToFeature(layer, feature);
        }
    }).addTo(layerGroup);

    if (currentLayer.LayerInitialVisibility === 1) {
        layerGroup.addTo(firmaMap.map);
    }

    overlayLayers[currentLayer.LayerName] = layerGroup;
    firmaMap.mapLayers.push(layerGeoJson);
};

// External Map layers
ProjectFirmaMaps.Map.prototype.addTiledLayerFromAGOL = function (layerConfig, overlayLayers) {
    var tileLayer = L.esri.tiledMapLayer({ url: layerConfig.LayerUrl });
    overlayLayers[layerConfig.DisplayName] = tileLayer;
    // Add to map if layer is on by default
    if (layerConfig.LayerIsOnByDefault) {
        tileLayer.addTo(this.map);
    }
}

ProjectFirmaMaps.Map.prototype.addVectorLayerFromAGOL = function (layerConfig, overlayLayers) {
    var featureLayer = L.esri.featureLayer(
        {
            url: layerConfig.LayerUrl,
            useCors: true,
            interactive: false,
        }
    );

    overlayLayers[layerConfig.DisplayName] = featureLayer;
    // Add to map if layer is on by default
    if (layerConfig.LayerIsOnByDefault) {
        featureLayer.addTo(this.map);
    }
}

// On hover behavior
ProjectFirmaMaps.Map.prototype.bindHoverToFeature = function (layer, feature) {
    if (!Sitka.Methods.isUndefinedNullOrEmpty(feature.properties["Hover Name"])) {
        layer.bindTooltip(feature.properties["Hover Name"], { direction: "auto", sticky: "true" });

        var originalFillOpacity = layer.options.fillOpacity;
        layer.on("mouseover",
            function () {
                layer.setStyle({ fillOpacity: Math.min(originalFillOpacity + 0.4, 1) });
            });
        layer.on("mouseout",
            function () {
                layer.setStyle({ fillOpacity: originalFillOpacity });
            });
    };
    return layer;
};

// On click behavior
ProjectFirmaMaps.Map.prototype.bindPopupToFeature = function (layer, feature) {
    var firmaMap = this;
    if (!Sitka.Methods.isUndefinedNullOrEmpty(feature.properties.PopupUrl)) {
        layer.bindPopup("Loading...");
        layer.on("click",
            function (e) {
                var latlng = e.target.getLatLng();
                firmaMap.map.setView(latlng);
                // PF-2830: one-way linking from a project's simple icon to its detailed polygon(s);
                // rides along with detailed-polygon display (PF-2829), no independent toggle
                if (firmaMap.AutoDisplayProjectDetailedLocationsByZoom &&
                    !Sitka.Methods.isUndefinedNullOrEmpty(feature.properties.ProjectID)) {
                    firmaMap.highlightProjectDetailedLocations(feature.properties.ProjectID);
                }
                jQuery.get(feature.properties.PopupUrl).done(function(data) {
                    layer.bindPopup(data).openPopup();
                });
            });

    }
};

ProjectFirmaMaps.Map.prototype.assignClickEventHandler = function(clickEventFunction) {
    var firmaMap = this;
    for (var i = 0; i < firmaMap.mapLayers.length; ++i) {
        var currentLayer = firmaMap.mapLayers[i];
        currentLayer.on("click", function(e) { clickEventFunction(firmaMap, e); });
    }
    firmaMap.map.on("click", function(e) { clickEventFunction(firmaMap, e); });
};

ProjectFirmaMaps.Map.prototype.removeClickEventHandler = function() {
    for (var i = 0; i < this.mapLayers.length; ++i) {
        var currentLayer = this.mapLayers[i];
        currentLayer.off("click");
    }
    this.map.off("click");
};

ProjectFirmaMaps.Map.prototype.formatLayerProperty = function (propertyName, propertyValue)
{
    if (Sitka.Methods.isUndefinedNullOrEmpty(propertyValue))
    {
        propertyValue = "&nbsp";
    }
    return "<span><strong>" + propertyName + ":</strong> " + propertyValue + "</span></br>";
};

ProjectFirmaMaps.Map.prototype.getFeatureInfo = function(e) {
    var latlng = e.latlng;
    var firmaMap = this;

    var wmsLayers = firmaMap.mapLayers.filter(function (layer) {
        return layer.hasOwnProperty('wmsParams') && firmaMap.map.hasLayer(layer);
    });

    var vectorLayers = firmaMap.mapLayers.filter(function(layer) {
        return !layer.hasOwnProperty('wmsParams') && firmaMap.map.hasLayer(layer);
    });

    var externalFeatureLayers = firmaMap.externalFeatureLayers;

    if (highlightOverlay)
        highlightOverlay.remove(); //For any click, remove the existing highlight

    if (wmsLayers.length > 0) {
        firmaMap.popupForWMSAndVectorLayers(wmsLayers, vectorLayers, externalFeatureLayers, latlng);
    } else {
        firmaMap.callPopupForVectorLayers(vectorLayers, externalFeatureLayers, latlng);
    }
};

ProjectFirmaMaps.Map.prototype.popupForWMSAndVectorLayers = function (wmsLayers, vectorLayers, externalFeatureLayers, latlng) {
    var firmaMap = this;

    var point = this.map.latLngToContainerPoint(latlng, firmaMap.map.getZoom()),
        size = firmaMap.map.getSize();

        geospatialAreaWMSParams = {
            service: 'WMS',
            version: "1.1.1",
            request: 'GetFeatureInfo',
            styles: "",
            srs: 'EPSG:4326',
            bbox: firmaMap.map.getBounds().toBBoxString(),
            height: size.y,
            width: size.x,
            info_format: 'application/json'
        };

    geospatialAreaWMSParams['x'] = point.x;
    geospatialAreaWMSParams['y'] = point.y;

    var ajaxCalls = [];

    // Gather promises for WMS layers
    for (var j = 0; j < wmsLayers.length; ++j) {
        var layer = wmsLayers[j];
        // Update layer name for each layer being queried
        geospatialAreaWMSParams['layers'] = layer.wmsParams.layers;
        geospatialAreaWMSParams['query_layers'] = layer.wmsParams.layers;


        var queryUrl = layer._url + L.Util.getParamString(geospatialAreaWMSParams, null, true);
        ajaxCalls.push(jQuery.when(jQuery.ajax({ url: queryUrl }))
            .then(function (response) {
                return firmaMap.formatGeospatialAreaResponse(response).then(function (status) {
                    return status;
                });
                
            }));
    }

    // ESRI Leaflet .query() isn't Promise-compatible so we're handling the call to the next function in the callback
    if (externalFeatureLayers.length > 0) {
        for (var i = 0; i < externalFeatureLayers.length; i++) {
            var featureLayer = externalFeatureLayers[i];
            var query = L.esri.query({
                url: featureLayer.LayerUrl
            });
            query.intersects(latlng);

            firmaMap.runEsriFeatureLayerQueryThenResolvePromises(featureLayer, query, ajaxCalls, vectorLayers, latlng);
        }
    }
    // No external map layers to query
    else {
        firmaMap.carryOutPromises(ajaxCalls).then(
            function (responses) {
                firmaMap.openPopupIncludingAsyncContent(responses, vectorLayers, latlng);
            },
            function (responses) {
                console.log("error getting wms feature info");
            }
        );
    }
};

ProjectFirmaMaps.Map.prototype.callPopupForVectorLayers = function (vectorLayers, externalFeatureLayers, latlng) {
    var firmaMap = this;

    var allLayers = [];

    // ESRI Leaflet .query() isn't Promise-compatible so we're handling the call to the next function in the callback
    if (externalFeatureLayers.length > 0) {
        for (var i = 0; i < externalFeatureLayers.length; i++) {
            var featureLayer = externalFeatureLayers[i];
            var query = L.esri.query({
                url: featureLayer.LayerUrl
            });

            firmaMap.runEsriFeatureLayerQueryThenCallPopupForVectorLayers(featureLayer, query, latlng, vectorLayers, allLayers);
        }
    } else {
        firmaMap.popupForVectorLayers(vectorLayers, allLayers, latlng);
    }
};

ProjectFirmaMaps.Map.prototype.runEsriFeatureLayerQueryThenResolvePromises = function (featureLayer, query, ajaxCalls, vectorLayers, latlng) {
    var firmaMap = this;
    var externalFeatureLayersInfo;
    query.run(function (error, featureCollection, response) {
        externalFeatureLayersInfo = firmaMap.formatExternalFeatureLayerResponse(error, featureLayer, featureCollection);
        firmaMap.carryOutPromises(ajaxCalls).then(
            function (responses) {
                firmaMap.openPopupIncludingAsyncContent(responses, vectorLayers, latlng, externalFeatureLayersInfo);
            },
            function (responses) {
                console.log("error getting wms feature info");
            }
        );
    });
}

ProjectFirmaMaps.Map.prototype.runEsriFeatureLayerQueryThenCallPopupForVectorLayers = function (featureLayer, query, latlng, vectorLayers, allLayers) {
    var firmaMap = this;
    var externalFeatureLayersInfo;
    query.intersects(latlng);
    query.run(function (error, featureCollection, response) {
        externalFeatureLayersInfo = firmaMap.formatExternalFeatureLayerResponse(error, featureLayer, featureCollection);
        allLayers.push(externalFeatureLayersInfo);
        firmaMap.popupForVectorLayers(vectorLayers, allLayers, latlng);
    });
}

ProjectFirmaMaps.Map.prototype.popupForVectorLayers = function(vectorLayers, allLayers, latlng) {
    for (var j = 0; j < vectorLayers.length; ++j) {
        var currentLayer = vectorLayers[j];
        var vectorLayerInfoHtmlForPopup = this.formatVectorLayerInfo(currentLayer, latlng);
        allLayers.push(vectorLayerInfoHtmlForPopup);
    }

    var lat = L.Util.formatNum(latlng.lat, 4);
    var lon = L.Util.formatNum(latlng.lng, 4);
    allLayers.push({
        label: "Location",
        link: lat + ", " + lon
    });

    this.map.setView(latlng);
    this.map.openPopup(L.popup({ maxWidth: 200 }).setLatLng(latlng).setContent(this.htmlPopupContents(allLayers)).openOn(this.map));
}

ProjectFirmaMaps.Map.prototype.carryOutPromises = function (deferreds) {
    var deferred = new jQuery.Deferred();
    $.when.apply(jQuery, deferreds).then(
        function () {
            deferred.resolve(Array.prototype.slice.call(arguments));
        },
        function () {
            deferred.fail(Array.prototype.slice.call(arguments));
        });

    return deferred;
}

ProjectFirmaMaps.Map.prototype.openPopupIncludingAsyncContent = function (responses, vectorLayers, latlng, externalFeatureLayersInfo) {
    var firmaMap = this;
    var allLayers = [];
    // vector layers
    for (var j = 0; j < vectorLayers.length; ++j) {
        var currentLayer = vectorLayers[j];

        var vectorLayerInfoHtmlForPopup = firmaMap.formatVectorLayerInfo(currentLayer, latlng);
        allLayers.push(vectorLayerInfoHtmlForPopup);
    }
    // wms layers
    _.forEach(responses,
        function (resp) {
            if (resp) {
                allLayers.push(resp);
            }
        });
    // external map layers
    if (externalFeatureLayersInfo) {
        allLayers.push(externalFeatureLayersInfo);
    }

    //lat lon
    var lat = L.Util.formatNum(latlng.lat, 4);
    var lon = L.Util.formatNum(latlng.lng, 4);
    allLayers.push({
        label: "Location",
        link: lat + ", " + lon
    });

    firmaMap.map.setView(latlng);
    firmaMap.map.openPopup(L.popup({ maxWidth: 200 }).setLatLng(latlng).setContent(firmaMap.htmlPopupContents(allLayers)).openOn(firmaMap.map));
}

// Highlight all of a Project's Detailed Locations by querying GeoServer for its polygons
// and drawing them into the module-global highlightOverlay (replacing any existing highlight)
ProjectFirmaMaps.Map.prototype.highlightProjectDetailedLocations = function (projectID) {
    if (!this.MapServiceUrl) {
        return;
    }
    if (highlightOverlay) {
        highlightOverlay.remove();
    }
    var projectLocationDetailedWfsParams = {
        service: "WFS",
        version: "2.0",
        request: "GetFeature",
        outputFormat: "application/json",
        SrsName: "EPSG:4326",
        maxFeatures: 10000,
        typeNames: this.ProjectDetailedLocationsPublicApprovedGeoServerLayerName,
        cql_filter: 'ProjectID=' + projectID
    };

    jQuery.ajax({
        url: this.MapServiceUrl +
            L.Util.getParamString(projectLocationDetailedWfsParams),
        dataType: 'json'
    }).done(function (data) {
        if (data.features.length > 0) {
            //Highlight feature(s) with new layer
            var highlightStyle = {
                "color": "#2dc3a1",
                "weight": 2,
                "opacity": 0.5
            };
            highlightOverlay = L.geoJSON(data, { style: highlightStyle });
            highlightOverlay.addTo(mapOutsideScope);
        }
    }).fail(function (data) {
        console.log(data);
    });
};

ProjectFirmaMaps.Map.prototype.formatGeospatialAreaResponse = function (json) {
    var deferred = new jQuery.Deferred();
    if (json.features.length > 0) {

        var firstFeature = json.features[0];
        switch (firstFeature.geometry_name) {
            case "GeospatialAreaFeature":
                linkHtml = "<a title='' href='/GeospatialArea/Detail/" + json.features[0].properties.GeospatialAreaID + "'>" + json.features[0].properties.GeospatialAreaShortName + "</a>";
                labelText = json.features[0].properties.GeospatialAreaTypeName

                deferred.resolve({
                    label: labelText,
                    link: linkHtml
                });
                break;
            case "ProjectLocationGeometry":
                this.highlightProjectDetailedLocations(firstFeature.properties.ProjectID);

                if (jQuery("#" + this.MapDivId).height() < 400) {
                    linkHtml = "<a title='' href='/Project/Detail/" + json.features[0].properties.ProjectID + "'>" + json.features[0].properties.ProjectName + "</a>";
                    labelText = this.ProjectFieldDefinitionLabel;

                    deferred.resolve({
                        label: labelText,
                        link: linkHtml
                    });
                } else {
                    queryUrl = "/Project/ProjectMapPopup/" + firstFeature.properties.ProjectID;
                    labelText = "Project";

                    var mapPopupAjaxCall = [];
                    mapPopupAjaxCall.push(jQuery.when(jQuery.ajax({ url: queryUrl }))
                        .then(function (response) { return response; }));

                    this.carryOutPromises(mapPopupAjaxCall).then(
                        function (responses) {
                            linkHtml = deferred.resolve({
                                label: labelText,
                                link: responses[0]
                            });
                        },
                        function (responses) {
                            console.log("error getting project popup info: " + queryUrl);
                            deferred.resolve(null);
                        }
                    );
                }
                break;
        }
    } else {
        deferred.resolve(null);
    }
    return deferred.promise();
};




ProjectFirmaMaps.Map.prototype.formatVectorLayerInfo = function (currentLayer, latlng) {
    var key = null,
        value = null;
    var match = leafletPip.pointInLayer(
        // the clicked point
        latlng,
        // this layer
        currentLayer,
        // whether to stop at first match
        true);

    var propertiesGroupedByKey = _(match)
        .map(function (x) {
            return _.keys(x.feature.properties);
        })
        .flatten()
        .uniq()
        .map(function (x) {
            return {
                key: x,
                values: _(match)
                    .map(function (y) {
                        return y.feature.properties[x];
                    })
                    .filter()
                    .value()
            };
        })
        .value();

    // if there's overlap, add some content to the popup: the layer name
    // and a table of attributes
    for (var i = 0; i < propertiesGroupedByKey.length; i++) {
        var group = propertiesGroupedByKey[i];
        if (group.key !== "Hover Name") {
            key = group.values.length > 1
                    ? group.key + "s" // pluralized
                    : group.key,
                value = group.values.join(", ");
        }
    }

    return {
        label: key,
        link: value
    };
};

ProjectFirmaMaps.Map.prototype.formatExternalFeatureLayerResponse = function (error, featureLayer, featureCollection) {
    if (error) {
        console.log(error);
        return null;
    }

    var featureNames = "";
    for (var j = 0; j < featureCollection.features.length; j++) {
        var feature = featureCollection.features[j];
        if (feature.properties.hasOwnProperty(featureLayer.FeatureNameField)) {
            featureNames += feature.properties[featureLayer.FeatureNameField];
        }
    }
    var popupContent = null;
    if (featureNames != "") {
        popupContent = {
            label: featureLayer.DisplayName,
            link: featureNames
        };
    }
    return popupContent;
};

ProjectFirmaMaps.Map.prototype.htmlPopupContents = function (allLayers) {
    var firmaMap = this;
    var uniqueArray = firmaMap.removeDuplicatesFromArray(allLayers, "label");

    var html = "<div>";

    _.forEach(uniqueArray,
        function (layer) {
            if (layer.label != null) {
                html += firmaMap.formatLayerProperty(layer.label, layer.link);
            }
        });

    html += "</div>";

    return html;
};

ProjectFirmaMaps.Map.prototype.removeDuplicatesFromArray = function (originalArray, prop) {
    var newArray = [];
    var lookupObject = {};

    for (var i in originalArray) {
        lookupObject[originalArray[i][prop]] = originalArray[i];
    }

    for (i in lookupObject) {
        newArray.push(lookupObject[i]);
    }
    return newArray;
}