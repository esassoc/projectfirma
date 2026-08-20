//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source View: [dbo].[vGeoServerProjectAttributes]
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ProjectFirmaModels.Models
{
    public class vGeoServerProjectAttributesConfiguration : EntityTypeConfiguration<vGeoServerProjectAttributes>
    {
        public vGeoServerProjectAttributesConfiguration() : this("dbo"){}

        public vGeoServerProjectAttributesConfiguration(string schema)
        {
            ToTable("vGeoServerProjectAttributes", schema);
            HasKey(x => x.PrimaryKey);
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
        }
    }
}