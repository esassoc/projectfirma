using System.Web;
using LtInfo.Common.AgGridWrappers;

namespace ProjectFirma.Web.Models
{
    public class GeospatialAreaIndexGridSimple
    {
        public int GeospatialAreaID { get; set; }
        public string GeospatialAreaShortName { get; set; }
        public int ProjectViewableByUserCount { get; set; }
        public string LayerColor { get; set; }
        public GeospatialAreaIndexGridSimple()
        {
        }

        public GeospatialAreaIndexGridSimple(int geospatialAreaId, string geospatialAreaShortName, int projectViewableByUserCount, string layerColor)
        {
            GeospatialAreaID = geospatialAreaId;
            GeospatialAreaShortName = geospatialAreaShortName;
            ProjectViewableByUserCount = projectViewableByUserCount;
            LayerColor = layerColor;
        }

        /// <summary>
        /// Returns trusted HTML for the map legend color swatch shown ahead of the short name in the grid, or null when
        /// this area has no layer color. This is passed to ag-grid as <see cref="HtmlLinkObject.WithHtmlPrefix"/>, which
        /// emits it as markup, so the layer color is attribute encoded here.
        /// </summary>
        public string GetLayerColorSwatchHtml()
        {
            if (string.IsNullOrWhiteSpace(LayerColor))
            {
                return null;
            }
            var encodedLayerColor = HttpUtility.HtmlAttributeEncode(LayerColor);
            return $"<span style=\"vertical-align:middle; width:10px; height:10px; margin-right:5px; display:inline-block; background:{encodedLayerColor};\"></span>";
        }
    }
}