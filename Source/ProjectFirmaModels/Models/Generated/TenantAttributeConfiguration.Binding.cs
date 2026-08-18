//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TenantAttribute]
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ProjectFirmaModels.Models
{
    public class TenantAttributeConfiguration : EntityTypeConfiguration<TenantAttribute>
    {
        public TenantAttributeConfiguration() : this("dbo"){}

        public TenantAttributeConfiguration(string schema)
        {
            ToTable("TenantAttribute", schema);
            HasKey(x => x.TenantAttributeID);
            Property(x => x.TenantAttributeID).HasColumnName(@"TenantAttributeID").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.TenantID).HasColumnName(@"TenantID").HasColumnType("int").IsRequired();
            Property(x => x.DefaultBoundingBox).HasColumnName(@"DefaultBoundingBox").HasColumnType("geometry").IsRequired();
            Property(x => x.MinimumYear).HasColumnName(@"MinimumYear").HasColumnType("int").IsRequired();
            Property(x => x.PrimaryContactPersonID).HasColumnName(@"PrimaryContactPersonID").HasColumnType("int").IsOptional();
            Property(x => x.TenantSquareLogoFileResourceInfoID).HasColumnName(@"TenantSquareLogoFileResourceInfoID").HasColumnType("int").IsOptional();
            Property(x => x.TenantBannerLogoFileResourceInfoID).HasColumnName(@"TenantBannerLogoFileResourceInfoID").HasColumnType("int").IsOptional();
            Property(x => x.TenantStyleSheetFileResourceInfoID).HasColumnName(@"TenantStyleSheetFileResourceInfoID").HasColumnType("int").IsOptional();
            Property(x => x.TenantShortDisplayName).HasColumnName(@"TenantShortDisplayName").HasColumnType("varchar").IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ToolDisplayName).HasColumnName(@"ToolDisplayName").HasColumnType("varchar").IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ShowProposalsToThePublic).HasColumnName(@"ShowProposalsToThePublic").HasColumnType("bit").IsRequired();
            Property(x => x.TaxonomyLevelID).HasColumnName(@"TaxonomyLevelID").HasColumnType("int").IsRequired();
            Property(x => x.AssociatePerfomanceMeasureTaxonomyLevelID).HasColumnName(@"AssociatePerfomanceMeasureTaxonomyLevelID").HasColumnType("int").IsRequired();
            Property(x => x.IsActive).HasColumnName(@"IsActive").HasColumnType("bit").IsRequired();
            Property(x => x.ProjectExternalDataSourceEnabled).HasColumnName(@"ProjectExternalDataSourceEnabled").HasColumnType("bit").IsRequired();
            Property(x => x.AccomplishmentsDashboardFundingDisplayTypeID).HasColumnName(@"AccomplishmentsDashboardFundingDisplayTypeID").HasColumnType("int").IsRequired();
            Property(x => x.AccomplishmentsDashboardAccomplishmentsButtonText).HasColumnName(@"AccomplishmentsDashboardAccomplishmentsButtonText").HasColumnType("varchar").IsOptional();
            Property(x => x.AccomplishmentsDashboardExpendituresButtonText).HasColumnName(@"AccomplishmentsDashboardExpendituresButtonText").HasColumnType("varchar").IsOptional();
            Property(x => x.AccomplishmentsDashboardOrganizationsButtonText).HasColumnName(@"AccomplishmentsDashboardOrganizationsButtonText").HasColumnType("varchar").IsOptional();
            Property(x => x.AccomplishmentsDashboardIncludeReportingOrganizationType).HasColumnName(@"AccomplishmentsDashboardIncludeReportingOrganizationType").HasColumnType("bit").IsRequired();
            Property(x => x.ShowLeadImplementerLogoOnFactSheet).HasColumnName(@"ShowLeadImplementerLogoOnFactSheet").HasColumnType("bit").IsRequired();
            Property(x => x.EnableAccomplishmentsDashboard).HasColumnName(@"EnableAccomplishmentsDashboard").HasColumnType("bit").IsRequired();
            Property(x => x.ProjectStewardshipAreaTypeID).HasColumnName(@"ProjectStewardshipAreaTypeID").HasColumnType("int").IsOptional();
            Property(x => x.EnableSecondaryProjectTaxonomyLeaf).HasColumnName(@"EnableSecondaryProjectTaxonomyLeaf").HasColumnType("bit").IsRequired();
            Property(x => x.KeystoneOpenIDClientIdentifier).HasColumnName(@"KeystoneOpenIDClientIdentifier").HasColumnType("varchar").IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.KeystoneOpenIDClientSecret).HasColumnName(@"KeystoneOpenIDClientSecret").HasColumnType("varchar").IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.BudgetTypeID).HasColumnName(@"BudgetTypeID").HasColumnType("int").IsRequired();
            Property(x => x.CanManageCustomAttributes).HasColumnName(@"CanManageCustomAttributes").HasColumnType("bit").IsRequired();
            Property(x => x.ExcludeTargetedFundingOrganizations).HasColumnName(@"ExcludeTargetedFundingOrganizations").HasColumnType("bit").IsRequired();
            Property(x => x.GoogleAnalyticsTrackingCode).HasColumnName(@"GoogleAnalyticsTrackingCode").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.UseProjectTimeline).HasColumnName(@"UseProjectTimeline").HasColumnType("bit").IsRequired();
            Property(x => x.GeoServerNamespace).HasColumnName(@"GeoServerNamespace").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.EnableEvaluations).HasColumnName(@"EnableEvaluations").HasColumnType("bit").IsRequired();
            Property(x => x.EnableProjectCategories).HasColumnName(@"EnableProjectCategories").HasColumnType("bit").IsRequired();
            Property(x => x.EnableReports).HasColumnName(@"EnableReports").HasColumnType("bit").IsRequired();
            Property(x => x.TenantFactSheetLogoFileResourceInfoID).HasColumnName(@"TenantFactSheetLogoFileResourceInfoID").HasColumnType("int").IsOptional();
            Property(x => x.EnableMatchmaker).HasColumnName(@"EnableMatchmaker").HasColumnType("bit").IsRequired();
            Property(x => x.MatchmakerAlgorithmIncludesProjectGeospatialAreas).HasColumnName(@"MatchmakerAlgorithmIncludesProjectGeospatialAreas").HasColumnType("bit").IsRequired();
            Property(x => x.AreGeospatialAreasExternallySourced).HasColumnName(@"AreGeospatialAreasExternallySourced").HasColumnType("bit").IsRequired();
            Property(x => x.ShowPhotoCreditOnFactSheet).HasColumnName(@"ShowPhotoCreditOnFactSheet").HasColumnType("bit").IsRequired();
            Property(x => x.TrackAccomplishments).HasColumnName(@"TrackAccomplishments").HasColumnType("bit").IsRequired();
            Property(x => x.ShowExpectedPerformanceMeasuresOnFactSheet).HasColumnName(@"ShowExpectedPerformanceMeasuresOnFactSheet").HasColumnType("bit").IsRequired();
            Property(x => x.EnableStatusUpdates).HasColumnName(@"EnableStatusUpdates").HasColumnType("bit").IsRequired();
            Property(x => x.EnableSolicitations).HasColumnName(@"EnableSolicitations").HasColumnType("bit").IsRequired();
            Property(x => x.EnableSimpleAccomplishmentsDashboard).HasColumnName(@"EnableSimpleAccomplishmentsDashboard").HasColumnType("bit").IsRequired();
            Property(x => x.SetTargetsByGeospatialArea).HasColumnName(@"SetTargetsByGeospatialArea").HasColumnType("bit").IsRequired();
            Property(x => x.ReportFinancialsAtProjectLevel).HasColumnName(@"ReportFinancialsAtProjectLevel").HasColumnType("bit").IsRequired();
            Property(x => x.ProjectExternalDataSourceApiUrl).HasColumnName(@"ProjectExternalDataSourceApiUrl").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.ProjectExternalSourceOfRecordName).HasColumnName(@"ProjectExternalSourceOfRecordName").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.ProjectExternalSourceOfRecordUrl).HasColumnName(@"ProjectExternalSourceOfRecordUrl").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.AreReportsPublic).HasColumnName(@"AreReportsPublic").HasColumnType("bit").IsRequired();
            Property(x => x.ProjectStewardshipVisibilityAdminOnly).HasColumnName(@"ProjectStewardshipVisibilityAdminOnly").HasColumnType("bit").IsOptional();
            Property(x => x.AccomplishmentsDashboardVisibilityAdminOnly).HasColumnName(@"AccomplishmentsDashboardVisibilityAdminOnly").HasColumnType("bit").IsRequired();
            Property(x => x.DisableProjectStewardOrganizationIndexPage).HasColumnName(@"DisableProjectStewardOrganizationIndexPage").HasColumnType("bit").IsOptional();
            Property(x => x.EnableDetailedLocationPolygonsOnProjectMap).HasColumnName(@"EnableDetailedLocationPolygonsOnProjectMap").HasColumnType("bit").IsOptional();
            Property(x => x.ProjectDataMapLayerGroupName).HasColumnName(@"ProjectDataMapLayerGroupName").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ReferenceMapLayerGroupName).HasColumnName(@"ReferenceMapLayerGroupName").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(100);

            // Foreign keys
            HasOptional(a => a.PrimaryContactPerson).WithMany(b => b.TenantAttributesWhereYouAreThePrimaryContactPerson).HasForeignKey(c => c.PrimaryContactPersonID).WillCascadeOnDelete(false); // FK_TenantAttribute_Person_PrimaryContactPersonID_PersonID
            HasOptional(a => a.TenantSquareLogoFileResourceInfo).WithMany(b => b.TenantAttributesWhereYouAreTheTenantSquareLogoFileResourceInfo).HasForeignKey(c => c.TenantSquareLogoFileResourceInfoID).WillCascadeOnDelete(false); // FK_TenantAttribute_FileResourceInfo_TenantSquareLogoFileResourceInfoID_FileResourceInfoID
            HasOptional(a => a.TenantBannerLogoFileResourceInfo).WithMany(b => b.TenantAttributesWhereYouAreTheTenantBannerLogoFileResourceInfo).HasForeignKey(c => c.TenantBannerLogoFileResourceInfoID).WillCascadeOnDelete(false); // FK_TenantAttribute_FileResourceInfo_TenantBannerLogoFileResourceInfoID_FileResourceInfoID
            HasOptional(a => a.TenantStyleSheetFileResourceInfo).WithMany(b => b.TenantAttributesWhereYouAreTheTenantStyleSheetFileResourceInfo).HasForeignKey(c => c.TenantStyleSheetFileResourceInfoID).WillCascadeOnDelete(false); // FK_TenantAttribute_FileResourceInfo_TenantStyleSheetFileResourceInfoID_FileResourceInfoID
            HasOptional(a => a.TenantFactSheetLogoFileResourceInfo).WithMany(b => b.TenantAttributesWhereYouAreTheTenantFactSheetLogoFileResourceInfo).HasForeignKey(c => c.TenantFactSheetLogoFileResourceInfoID).WillCascadeOnDelete(false); // FK_TenantAttribute_FileResourceInfo_TenantFactSheetLogoFileResourceInfoID_FileResourceInfoID
        }
    }
}