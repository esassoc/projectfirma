//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ExternalMapLayer]
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ProjectFirmaModels.Models
{
    public class ExternalMapLayerConfiguration : EntityTypeConfiguration<ExternalMapLayer>
    {
        public ExternalMapLayerConfiguration() : this("dbo"){}

        public ExternalMapLayerConfiguration(string schema)
        {
            ToTable("ExternalMapLayer", schema);
            HasKey(x => x.ExternalMapLayerID);
            Property(x => x.ExternalMapLayerID).HasColumnName(@"ExternalMapLayerID").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.TenantID).HasColumnName(@"TenantID").HasColumnType("int").IsRequired();
            Property(x => x.DisplayName).HasColumnName(@"DisplayName").HasColumnType("varchar").IsRequired().IsUnicode(false).HasMaxLength(75);
            Property(x => x.LayerUrl).HasColumnName(@"LayerUrl").HasColumnType("varchar").IsRequired().IsUnicode(false).HasMaxLength(500);
            Property(x => x.LayerDescription).HasColumnName(@"LayerDescription").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.FeatureNameField).HasColumnName(@"FeatureNameField").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.DisplayOnAllProjectMaps).HasColumnName(@"DisplayOnAllProjectMaps").HasColumnType("bit").IsRequired();
            Property(x => x.LayerIsOnByDefault).HasColumnName(@"LayerIsOnByDefault").HasColumnType("bit").IsRequired();
            Property(x => x.IsActive).HasColumnName(@"IsActive").HasColumnType("bit").IsRequired();
            Property(x => x.IsTiledMapService).HasColumnName(@"IsTiledMapService").HasColumnType("bit").IsRequired();
            Property(x => x.MapLegendImageFileResourceInfoID).HasColumnName(@"MapLegendImageFileResourceInfoID").HasColumnType("int").IsOptional();
            Property(x => x.MapLayerGroupName).HasColumnName(@"MapLayerGroupName").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.SortOrder).HasColumnName(@"SortOrder").HasColumnType("int").IsOptional();

            // Foreign keys
            HasOptional(a => a.MapLegendImageFileResourceInfo).WithMany(b => b.ExternalMapLayersWhereYouAreTheMapLegendImageFileResourceInfo).HasForeignKey(c => c.MapLegendImageFileResourceInfoID).WillCascadeOnDelete(false); // FK_ExternalMapLayer_FileResourceInfo_MapLegendImageFileResourceInfoID_FileResourceInfoID
        }
    }
}