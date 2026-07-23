//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TenantAttribute]
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using CodeFirstStoreFunctions;
using LtInfo.Common;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;

namespace ProjectFirmaModels.Models
{
    // Table [dbo].[TenantAttribute] is multi-tenant, so is attributed as IHaveATenantID
    [Table("[dbo].[TenantAttribute]")]
    public partial class TenantAttribute : IHavePrimaryKey, IHaveATenantID
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected TenantAttribute()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public TenantAttribute(int tenantAttributeID, DbGeometry defaultBoundingBox, int minimumYear, int? primaryContactPersonID, int? tenantSquareLogoFileResourceInfoID, int? tenantBannerLogoFileResourceInfoID, int? tenantStyleSheetFileResourceInfoID, string tenantShortDisplayName, string toolDisplayName, bool showProposalsToThePublic, int taxonomyLevelID, int associatePerfomanceMeasureTaxonomyLevelID, bool isActive, bool projectExternalDataSourceEnabled, int accomplishmentsDashboardFundingDisplayTypeID, string accomplishmentsDashboardAccomplishmentsButtonText, string accomplishmentsDashboardExpendituresButtonText, string accomplishmentsDashboardOrganizationsButtonText, bool accomplishmentsDashboardIncludeReportingOrganizationType, bool showLeadImplementerLogoOnFactSheet, bool enableAccomplishmentsDashboard, int? projectStewardshipAreaTypeID, bool enableSecondaryProjectTaxonomyLeaf, string keystoneOpenIDClientIdentifier, string keystoneOpenIDClientSecret, int budgetTypeID, bool canManageCustomAttributes, bool excludeTargetedFundingOrganizations, string googleAnalyticsTrackingCode, bool useProjectTimeline, string geoServerNamespace, bool enableEvaluations, bool enableProjectCategories, bool enableReports, int? tenantFactSheetLogoFileResourceInfoID, bool enableMatchmaker, bool matchmakerAlgorithmIncludesProjectGeospatialAreas, bool areGeospatialAreasExternallySourced, bool showPhotoCreditOnFactSheet, bool trackAccomplishments, bool showExpectedPerformanceMeasuresOnFactSheet, bool enableStatusUpdates, bool enableSolicitations, bool enableSimpleAccomplishmentsDashboard, bool setTargetsByGeospatialArea, bool reportFinancialsAtProjectLevel, string projectExternalDataSourceApiUrl, string projectExternalSourceOfRecordName, string projectExternalSourceOfRecordUrl, bool areReportsPublic, bool? projectStewardshipVisibilityAdminOnly, bool accomplishmentsDashboardVisibilityAdminOnly, bool? disableProjectStewardOrganizationIndexPage, bool? enableDetailedLocationPolygonsOnProjectMap) : this()
        {
            this.TenantAttributeID = tenantAttributeID;
            this.DefaultBoundingBox = defaultBoundingBox;
            this.MinimumYear = minimumYear;
            this.PrimaryContactPersonID = primaryContactPersonID;
            this.TenantSquareLogoFileResourceInfoID = tenantSquareLogoFileResourceInfoID;
            this.TenantBannerLogoFileResourceInfoID = tenantBannerLogoFileResourceInfoID;
            this.TenantStyleSheetFileResourceInfoID = tenantStyleSheetFileResourceInfoID;
            this.TenantShortDisplayName = tenantShortDisplayName;
            this.ToolDisplayName = toolDisplayName;
            this.ShowProposalsToThePublic = showProposalsToThePublic;
            this.TaxonomyLevelID = taxonomyLevelID;
            this.AssociatePerfomanceMeasureTaxonomyLevelID = associatePerfomanceMeasureTaxonomyLevelID;
            this.IsActive = isActive;
            this.ProjectExternalDataSourceEnabled = projectExternalDataSourceEnabled;
            this.AccomplishmentsDashboardFundingDisplayTypeID = accomplishmentsDashboardFundingDisplayTypeID;
            this.AccomplishmentsDashboardAccomplishmentsButtonText = accomplishmentsDashboardAccomplishmentsButtonText;
            this.AccomplishmentsDashboardExpendituresButtonText = accomplishmentsDashboardExpendituresButtonText;
            this.AccomplishmentsDashboardOrganizationsButtonText = accomplishmentsDashboardOrganizationsButtonText;
            this.AccomplishmentsDashboardIncludeReportingOrganizationType = accomplishmentsDashboardIncludeReportingOrganizationType;
            this.ShowLeadImplementerLogoOnFactSheet = showLeadImplementerLogoOnFactSheet;
            this.EnableAccomplishmentsDashboard = enableAccomplishmentsDashboard;
            this.ProjectStewardshipAreaTypeID = projectStewardshipAreaTypeID;
            this.EnableSecondaryProjectTaxonomyLeaf = enableSecondaryProjectTaxonomyLeaf;
            this.KeystoneOpenIDClientIdentifier = keystoneOpenIDClientIdentifier;
            this.KeystoneOpenIDClientSecret = keystoneOpenIDClientSecret;
            this.BudgetTypeID = budgetTypeID;
            this.CanManageCustomAttributes = canManageCustomAttributes;
            this.ExcludeTargetedFundingOrganizations = excludeTargetedFundingOrganizations;
            this.GoogleAnalyticsTrackingCode = googleAnalyticsTrackingCode;
            this.UseProjectTimeline = useProjectTimeline;
            this.GeoServerNamespace = geoServerNamespace;
            this.EnableEvaluations = enableEvaluations;
            this.EnableProjectCategories = enableProjectCategories;
            this.EnableReports = enableReports;
            this.TenantFactSheetLogoFileResourceInfoID = tenantFactSheetLogoFileResourceInfoID;
            this.EnableMatchmaker = enableMatchmaker;
            this.MatchmakerAlgorithmIncludesProjectGeospatialAreas = matchmakerAlgorithmIncludesProjectGeospatialAreas;
            this.AreGeospatialAreasExternallySourced = areGeospatialAreasExternallySourced;
            this.ShowPhotoCreditOnFactSheet = showPhotoCreditOnFactSheet;
            this.TrackAccomplishments = trackAccomplishments;
            this.ShowExpectedPerformanceMeasuresOnFactSheet = showExpectedPerformanceMeasuresOnFactSheet;
            this.EnableStatusUpdates = enableStatusUpdates;
            this.EnableSolicitations = enableSolicitations;
            this.EnableSimpleAccomplishmentsDashboard = enableSimpleAccomplishmentsDashboard;
            this.SetTargetsByGeospatialArea = setTargetsByGeospatialArea;
            this.ReportFinancialsAtProjectLevel = reportFinancialsAtProjectLevel;
            this.ProjectExternalDataSourceApiUrl = projectExternalDataSourceApiUrl;
            this.ProjectExternalSourceOfRecordName = projectExternalSourceOfRecordName;
            this.ProjectExternalSourceOfRecordUrl = projectExternalSourceOfRecordUrl;
            this.AreReportsPublic = areReportsPublic;
            this.ProjectStewardshipVisibilityAdminOnly = projectStewardshipVisibilityAdminOnly;
            this.AccomplishmentsDashboardVisibilityAdminOnly = accomplishmentsDashboardVisibilityAdminOnly;
            this.DisableProjectStewardOrganizationIndexPage = disableProjectStewardOrganizationIndexPage;
            this.EnableDetailedLocationPolygonsOnProjectMap = enableDetailedLocationPolygonsOnProjectMap;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public TenantAttribute(DbGeometry defaultBoundingBox, int minimumYear, string tenantShortDisplayName, string toolDisplayName, bool showProposalsToThePublic, int taxonomyLevelID, int associatePerfomanceMeasureTaxonomyLevelID, bool isActive, bool projectExternalDataSourceEnabled, int accomplishmentsDashboardFundingDisplayTypeID, bool accomplishmentsDashboardIncludeReportingOrganizationType, bool showLeadImplementerLogoOnFactSheet, bool enableAccomplishmentsDashboard, bool enableSecondaryProjectTaxonomyLeaf, string keystoneOpenIDClientIdentifier, string keystoneOpenIDClientSecret, int budgetTypeID, bool canManageCustomAttributes, bool excludeTargetedFundingOrganizations, bool useProjectTimeline, bool enableEvaluations, bool enableProjectCategories, bool enableReports, bool enableMatchmaker, bool matchmakerAlgorithmIncludesProjectGeospatialAreas, bool areGeospatialAreasExternallySourced, bool showPhotoCreditOnFactSheet, bool trackAccomplishments, bool showExpectedPerformanceMeasuresOnFactSheet, bool enableStatusUpdates, bool enableSolicitations, bool enableSimpleAccomplishmentsDashboard, bool setTargetsByGeospatialArea, bool reportFinancialsAtProjectLevel, bool areReportsPublic, bool accomplishmentsDashboardVisibilityAdminOnly) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.TenantAttributeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.DefaultBoundingBox = defaultBoundingBox;
            this.MinimumYear = minimumYear;
            this.TenantShortDisplayName = tenantShortDisplayName;
            this.ToolDisplayName = toolDisplayName;
            this.ShowProposalsToThePublic = showProposalsToThePublic;
            this.TaxonomyLevelID = taxonomyLevelID;
            this.AssociatePerfomanceMeasureTaxonomyLevelID = associatePerfomanceMeasureTaxonomyLevelID;
            this.IsActive = isActive;
            this.ProjectExternalDataSourceEnabled = projectExternalDataSourceEnabled;
            this.AccomplishmentsDashboardFundingDisplayTypeID = accomplishmentsDashboardFundingDisplayTypeID;
            this.AccomplishmentsDashboardIncludeReportingOrganizationType = accomplishmentsDashboardIncludeReportingOrganizationType;
            this.ShowLeadImplementerLogoOnFactSheet = showLeadImplementerLogoOnFactSheet;
            this.EnableAccomplishmentsDashboard = enableAccomplishmentsDashboard;
            this.EnableSecondaryProjectTaxonomyLeaf = enableSecondaryProjectTaxonomyLeaf;
            this.KeystoneOpenIDClientIdentifier = keystoneOpenIDClientIdentifier;
            this.KeystoneOpenIDClientSecret = keystoneOpenIDClientSecret;
            this.BudgetTypeID = budgetTypeID;
            this.CanManageCustomAttributes = canManageCustomAttributes;
            this.ExcludeTargetedFundingOrganizations = excludeTargetedFundingOrganizations;
            this.UseProjectTimeline = useProjectTimeline;
            this.EnableEvaluations = enableEvaluations;
            this.EnableProjectCategories = enableProjectCategories;
            this.EnableReports = enableReports;
            this.EnableMatchmaker = enableMatchmaker;
            this.MatchmakerAlgorithmIncludesProjectGeospatialAreas = matchmakerAlgorithmIncludesProjectGeospatialAreas;
            this.AreGeospatialAreasExternallySourced = areGeospatialAreasExternallySourced;
            this.ShowPhotoCreditOnFactSheet = showPhotoCreditOnFactSheet;
            this.TrackAccomplishments = trackAccomplishments;
            this.ShowExpectedPerformanceMeasuresOnFactSheet = showExpectedPerformanceMeasuresOnFactSheet;
            this.EnableStatusUpdates = enableStatusUpdates;
            this.EnableSolicitations = enableSolicitations;
            this.EnableSimpleAccomplishmentsDashboard = enableSimpleAccomplishmentsDashboard;
            this.SetTargetsByGeospatialArea = setTargetsByGeospatialArea;
            this.ReportFinancialsAtProjectLevel = reportFinancialsAtProjectLevel;
            this.AreReportsPublic = areReportsPublic;
            this.AccomplishmentsDashboardVisibilityAdminOnly = accomplishmentsDashboardVisibilityAdminOnly;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public TenantAttribute(DbGeometry defaultBoundingBox, int minimumYear, string tenantShortDisplayName, string toolDisplayName, bool showProposalsToThePublic, TaxonomyLevel taxonomyLevel, TaxonomyLevel associatePerfomanceMeasureTaxonomyLevel, bool isActive, bool projectExternalDataSourceEnabled, AccomplishmentsDashboardFundingDisplayType accomplishmentsDashboardFundingDisplayType, bool accomplishmentsDashboardIncludeReportingOrganizationType, bool showLeadImplementerLogoOnFactSheet, bool enableAccomplishmentsDashboard, bool enableSecondaryProjectTaxonomyLeaf, string keystoneOpenIDClientIdentifier, string keystoneOpenIDClientSecret, BudgetType budgetType, bool canManageCustomAttributes, bool excludeTargetedFundingOrganizations, bool useProjectTimeline, bool enableEvaluations, bool enableProjectCategories, bool enableReports, bool enableMatchmaker, bool matchmakerAlgorithmIncludesProjectGeospatialAreas, bool areGeospatialAreasExternallySourced, bool showPhotoCreditOnFactSheet, bool trackAccomplishments, bool showExpectedPerformanceMeasuresOnFactSheet, bool enableStatusUpdates, bool enableSolicitations, bool enableSimpleAccomplishmentsDashboard, bool setTargetsByGeospatialArea, bool reportFinancialsAtProjectLevel, bool areReportsPublic, bool accomplishmentsDashboardVisibilityAdminOnly) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.TenantAttributeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.DefaultBoundingBox = defaultBoundingBox;
            this.MinimumYear = minimumYear;
            this.TenantShortDisplayName = tenantShortDisplayName;
            this.ToolDisplayName = toolDisplayName;
            this.ShowProposalsToThePublic = showProposalsToThePublic;
            this.TaxonomyLevelID = taxonomyLevel.TaxonomyLevelID;
            this.AssociatePerfomanceMeasureTaxonomyLevelID = associatePerfomanceMeasureTaxonomyLevel.TaxonomyLevelID;
            this.IsActive = isActive;
            this.ProjectExternalDataSourceEnabled = projectExternalDataSourceEnabled;
            this.AccomplishmentsDashboardFundingDisplayTypeID = accomplishmentsDashboardFundingDisplayType.AccomplishmentsDashboardFundingDisplayTypeID;
            this.AccomplishmentsDashboardIncludeReportingOrganizationType = accomplishmentsDashboardIncludeReportingOrganizationType;
            this.ShowLeadImplementerLogoOnFactSheet = showLeadImplementerLogoOnFactSheet;
            this.EnableAccomplishmentsDashboard = enableAccomplishmentsDashboard;
            this.EnableSecondaryProjectTaxonomyLeaf = enableSecondaryProjectTaxonomyLeaf;
            this.KeystoneOpenIDClientIdentifier = keystoneOpenIDClientIdentifier;
            this.KeystoneOpenIDClientSecret = keystoneOpenIDClientSecret;
            this.BudgetTypeID = budgetType.BudgetTypeID;
            this.CanManageCustomAttributes = canManageCustomAttributes;
            this.ExcludeTargetedFundingOrganizations = excludeTargetedFundingOrganizations;
            this.UseProjectTimeline = useProjectTimeline;
            this.EnableEvaluations = enableEvaluations;
            this.EnableProjectCategories = enableProjectCategories;
            this.EnableReports = enableReports;
            this.EnableMatchmaker = enableMatchmaker;
            this.MatchmakerAlgorithmIncludesProjectGeospatialAreas = matchmakerAlgorithmIncludesProjectGeospatialAreas;
            this.AreGeospatialAreasExternallySourced = areGeospatialAreasExternallySourced;
            this.ShowPhotoCreditOnFactSheet = showPhotoCreditOnFactSheet;
            this.TrackAccomplishments = trackAccomplishments;
            this.ShowExpectedPerformanceMeasuresOnFactSheet = showExpectedPerformanceMeasuresOnFactSheet;
            this.EnableStatusUpdates = enableStatusUpdates;
            this.EnableSolicitations = enableSolicitations;
            this.EnableSimpleAccomplishmentsDashboard = enableSimpleAccomplishmentsDashboard;
            this.SetTargetsByGeospatialArea = setTargetsByGeospatialArea;
            this.ReportFinancialsAtProjectLevel = reportFinancialsAtProjectLevel;
            this.AreReportsPublic = areReportsPublic;
            this.AccomplishmentsDashboardVisibilityAdminOnly = accomplishmentsDashboardVisibilityAdminOnly;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static TenantAttribute CreateNewBlank(TaxonomyLevel taxonomyLevel, TaxonomyLevel associatePerfomanceMeasureTaxonomyLevel, AccomplishmentsDashboardFundingDisplayType accomplishmentsDashboardFundingDisplayType, BudgetType budgetType)
        {
            return new TenantAttribute(default(DbGeometry), default(int), default(string), default(string), default(bool), taxonomyLevel, associatePerfomanceMeasureTaxonomyLevel, default(bool), default(bool), accomplishmentsDashboardFundingDisplayType, default(bool), default(bool), default(bool), default(bool), default(string), default(string), budgetType, default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool), default(bool));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return false;
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(TenantAttribute).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.AllTenantAttributes.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int TenantAttributeID { get; set; }
        public int TenantID { get; set; }
        public DbGeometry DefaultBoundingBox { get; set; }
        public int MinimumYear { get; set; }
        public int? PrimaryContactPersonID { get; set; }
        public int? TenantSquareLogoFileResourceInfoID { get; set; }
        public int? TenantBannerLogoFileResourceInfoID { get; set; }
        public int? TenantStyleSheetFileResourceInfoID { get; set; }
        public string TenantShortDisplayName { get; set; }
        public string ToolDisplayName { get; set; }
        public bool ShowProposalsToThePublic { get; set; }
        public int TaxonomyLevelID { get; set; }
        public int AssociatePerfomanceMeasureTaxonomyLevelID { get; set; }
        public bool IsActive { get; set; }
        public bool ProjectExternalDataSourceEnabled { get; set; }
        public int AccomplishmentsDashboardFundingDisplayTypeID { get; set; }
        public string AccomplishmentsDashboardAccomplishmentsButtonText { get; set; }
        [NotMapped]
        public HtmlString AccomplishmentsDashboardAccomplishmentsButtonTextHtmlString
        { 
            get { return AccomplishmentsDashboardAccomplishmentsButtonText == null ? null : new HtmlString(AccomplishmentsDashboardAccomplishmentsButtonText); }
            set { AccomplishmentsDashboardAccomplishmentsButtonText = value?.ToString(); }
        }
        public string AccomplishmentsDashboardExpendituresButtonText { get; set; }
        [NotMapped]
        public HtmlString AccomplishmentsDashboardExpendituresButtonTextHtmlString
        { 
            get { return AccomplishmentsDashboardExpendituresButtonText == null ? null : new HtmlString(AccomplishmentsDashboardExpendituresButtonText); }
            set { AccomplishmentsDashboardExpendituresButtonText = value?.ToString(); }
        }
        public string AccomplishmentsDashboardOrganizationsButtonText { get; set; }
        [NotMapped]
        public HtmlString AccomplishmentsDashboardOrganizationsButtonTextHtmlString
        { 
            get { return AccomplishmentsDashboardOrganizationsButtonText == null ? null : new HtmlString(AccomplishmentsDashboardOrganizationsButtonText); }
            set { AccomplishmentsDashboardOrganizationsButtonText = value?.ToString(); }
        }
        public bool AccomplishmentsDashboardIncludeReportingOrganizationType { get; set; }
        public bool ShowLeadImplementerLogoOnFactSheet { get; set; }
        public bool EnableAccomplishmentsDashboard { get; set; }
        public int? ProjectStewardshipAreaTypeID { get; set; }
        public bool EnableSecondaryProjectTaxonomyLeaf { get; set; }
        public string KeystoneOpenIDClientIdentifier { get; set; }
        public string KeystoneOpenIDClientSecret { get; set; }
        public int BudgetTypeID { get; set; }
        public bool CanManageCustomAttributes { get; set; }
        public bool ExcludeTargetedFundingOrganizations { get; set; }
        public string GoogleAnalyticsTrackingCode { get; set; }
        public bool UseProjectTimeline { get; set; }
        public string GeoServerNamespace { get; set; }
        public bool EnableEvaluations { get; set; }
        public bool EnableProjectCategories { get; set; }
        public bool EnableReports { get; set; }
        public int? TenantFactSheetLogoFileResourceInfoID { get; set; }
        public bool EnableMatchmaker { get; set; }
        public bool MatchmakerAlgorithmIncludesProjectGeospatialAreas { get; set; }
        public bool AreGeospatialAreasExternallySourced { get; set; }
        public bool ShowPhotoCreditOnFactSheet { get; set; }
        public bool TrackAccomplishments { get; set; }
        public bool ShowExpectedPerformanceMeasuresOnFactSheet { get; set; }
        public bool EnableStatusUpdates { get; set; }
        public bool EnableSolicitations { get; set; }
        public bool EnableSimpleAccomplishmentsDashboard { get; set; }
        public bool SetTargetsByGeospatialArea { get; set; }
        public bool ReportFinancialsAtProjectLevel { get; set; }
        public string ProjectExternalDataSourceApiUrl { get; set; }
        public string ProjectExternalSourceOfRecordName { get; set; }
        public string ProjectExternalSourceOfRecordUrl { get; set; }
        public bool AreReportsPublic { get; set; }
        public bool? ProjectStewardshipVisibilityAdminOnly { get; set; }
        public bool AccomplishmentsDashboardVisibilityAdminOnly { get; set; }
        public bool? DisableProjectStewardOrganizationIndexPage { get; set; }
        public bool? EnableDetailedLocationPolygonsOnProjectMap { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return TenantAttributeID; } set { TenantAttributeID = value; } }

        public Tenant Tenant { get { return Tenant.AllLookupDictionary[TenantID]; } }
        public virtual Person PrimaryContactPerson { get; set; }
        public virtual FileResourceInfo TenantBannerLogoFileResourceInfo { get; set; }
        public virtual FileResourceInfo TenantFactSheetLogoFileResourceInfo { get; set; }
        public virtual FileResourceInfo TenantSquareLogoFileResourceInfo { get; set; }
        public virtual FileResourceInfo TenantStyleSheetFileResourceInfo { get; set; }
        public TaxonomyLevel AssociatePerfomanceMeasureTaxonomyLevel { get { return TaxonomyLevel.AllLookupDictionary[AssociatePerfomanceMeasureTaxonomyLevelID]; } }
        public TaxonomyLevel TaxonomyLevel { get { return TaxonomyLevel.AllLookupDictionary[TaxonomyLevelID]; } }
        public AccomplishmentsDashboardFundingDisplayType AccomplishmentsDashboardFundingDisplayType { get { return AccomplishmentsDashboardFundingDisplayType.AllLookupDictionary[AccomplishmentsDashboardFundingDisplayTypeID]; } }
        public ProjectStewardshipAreaType ProjectStewardshipAreaType { get { return ProjectStewardshipAreaTypeID.HasValue ? ProjectStewardshipAreaType.AllLookupDictionary[ProjectStewardshipAreaTypeID.Value] : null; } }
        public BudgetType BudgetType { get { return BudgetType.AllLookupDictionary[BudgetTypeID]; } }

        public static class FieldLengths
        {
            public const int TenantShortDisplayName = 100;
            public const int ToolDisplayName = 100;
            public const int KeystoneOpenIDClientIdentifier = 256;
            public const int KeystoneOpenIDClientSecret = 256;
            public const int GoogleAnalyticsTrackingCode = 100;
            public const int GeoServerNamespace = 256;
            public const int ProjectExternalDataSourceApiUrl = 500;
            public const int ProjectExternalSourceOfRecordName = 256;
            public const int ProjectExternalSourceOfRecordUrl = 500;
        }
    }
}