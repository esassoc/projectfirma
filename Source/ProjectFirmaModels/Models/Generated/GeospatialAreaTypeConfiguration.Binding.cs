//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GeospatialAreaType]
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ProjectFirmaModels.Models
{
    public class GeospatialAreaTypeConfiguration : EntityTypeConfiguration<GeospatialAreaType>
    {
        public GeospatialAreaTypeConfiguration() : this("dbo"){}

        public GeospatialAreaTypeConfiguration(string schema)
        {
            ToTable("GeospatialAreaType", schema);
            HasKey(x => x.GeospatialAreaTypeID);
            Property(x => x.GeospatialAreaTypeID).HasColumnName(@"GeospatialAreaTypeID").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.TenantID).HasColumnName(@"TenantID").HasColumnType("int").IsRequired();
            Property(x => x.GeospatialAreaTypeName).HasColumnName(@"GeospatialAreaTypeName").HasColumnType("varchar").IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.GeospatialAreaTypeNamePluralized).HasColumnName(@"GeospatialAreaTypeNamePluralized").HasColumnType("varchar").IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.GeospatialAreaIntroContent).HasColumnName(@"GeospatialAreaIntroContent").HasColumnType("varchar").IsOptional();
            Property(x => x.GeospatialAreaTypeDefinition).HasColumnName(@"GeospatialAreaTypeDefinition").HasColumnType("varchar").IsOptional();
            Property(x => x.GeospatialAreaLayerName).HasColumnName(@"GeospatialAreaLayerName").HasColumnType("varchar").IsRequired().IsUnicode(false).HasMaxLength(255);
            Property(x => x.DisplayOnAllProjectMaps).HasColumnName(@"DisplayOnAllProjectMaps").HasColumnType("bit").IsRequired();
            Property(x => x.OnByDefaultOnProjectMap).HasColumnName(@"OnByDefaultOnProjectMap").HasColumnType("bit").IsRequired();
            Property(x => x.OnByDefaultOnOtherMaps).HasColumnName(@"OnByDefaultOnOtherMaps").HasColumnType("bit").IsRequired();
            Property(x => x.ServiceUrl).HasColumnName(@"ServiceUrl").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(1000);
            Property(x => x.MapLegendImageFileResourceInfoID).HasColumnName(@"MapLegendImageFileResourceInfoID").HasColumnType("int").IsOptional();
            Property(x => x.MapLayerGroupName).HasColumnName(@"MapLayerGroupName").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.SortOrder).HasColumnName(@"SortOrder").HasColumnType("int").IsOptional();

            // Foreign keys
            HasOptional(a => a.MapLegendImageFileResourceInfo).WithMany(b => b.GeospatialAreaTypesWhereYouAreTheMapLegendImageFileResourceInfo).HasForeignKey(c => c.MapLegendImageFileResourceInfoID).WillCascadeOnDelete(false); // FK_GeospatialAreaType_FileResourceInfo_MapLegendImageFileResourceInfoID_FileResourceInfoID
        }
    }
}