ProjectFirmaMaps.SelectableMap = function(mapInitJson) {
    ProjectFirmaMaps.Map.call(this, mapInitJson);

    var self = this;
    
    _.each(this.mapLayers, function (mapLayer) {
        mapLayer.removeEventListener("click");
        mapLayer.on("click", function(event) {
            self.setLayerSelected(event.layer);
            self.onSelectLayer(event);
        });
    });
};

ProjectFirmaMaps.SelectableMap.prototype = Sitka.Methods.clonePrototype(ProjectFirmaMaps.Map.prototype);

ProjectFirmaMaps.Map.prototype.addLayersToMapLayersControl = function (baseLayers, overlayLayers) {
    delete overlayLayers["Street Labels"];
    var groupedOverlays = {
        "Feature Groups": overlayLayers
    };
    var groupedLayerControlOptions = {
        exclusiveGroups: ["Feature Groups"]
    };
    // Assign layerControl: callers such as EditProjectGeospatialAreasController and
    // BulkSetProjectSpatialInformationController add and remove their selected-area layers through it.
    // Those removeLayer calls need the plugin's broken removeLayer repaired first.
    ProjectFirmaMaps.patchGroupedLayerControlRemoveLayer();
    this.layerControl = L.control.groupedLayers(baseLayers, groupedOverlays, groupedLayerControlOptions);
    this.layerControl.addTo(this.map);
};

ProjectFirmaMaps.SelectableMap.prototype.onSelectLayer = function () {
    // This can be overriden on the page to do custom stuff when a layer is selected
};

ProjectFirmaMaps.SelectableMap.prototype.setLayerSelected = function(layer) {
    if (!Sitka.Methods.isUndefinedNullOrEmpty(this.lastSelected)) {
        this.map.removeLayer(this.lastSelected);
    }

    this.lastSelected = L.geoJson(layer.toGeoJSON(),
        {
            pointToLayer: function(feature, latlng) {
                return L.marker(latlng,
                    {
                        icon: L.MakiMarkers.icon({
                            icon: "marker",
                            color: "#ff0",
                            size: "m",
                            alt: "Selected Marker",
                        }),
                        riseOnHover: true
                    });
            },
            style: function() {
                return {
                    fillcolor: "#ff0",
                    fill: true,
                    fillOpacity: 0.2,
                    color: "#ff0",
                    weight: 5,
                    stroke: true
                };
            }
        });
    this.lastSelected.addTo(this.map);
};
