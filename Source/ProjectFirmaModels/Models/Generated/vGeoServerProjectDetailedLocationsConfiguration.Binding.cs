//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source View: [dbo].[vGeoServerProjectDetailedLocations]
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ProjectFirmaModels.Models
{
    public class vGeoServerProjectDetailedLocationsConfiguration : EntityTypeConfiguration<vGeoServerProjectDetailedLocations>
    {
        public vGeoServerProjectDetailedLocationsConfiguration() : this("dbo"){}

        public vGeoServerProjectDetailedLocationsConfiguration(string schema)
        {
            ToTable("vGeoServerProjectDetailedLocations", schema);
            HasKey(x => x.PrimaryKey);
            
            
            
            
            
            
            
            
            
            
            
        }
    }
}