SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TenantAttribute](
	[TenantAttributeID] [int] IDENTITY(1,1) NOT NULL,
	[TenantID] [int] NOT NULL,
	[DefaultBoundingBox] [geometry] NOT NULL,
	[MinimumYear] [int] NOT NULL,
	[PrimaryContactPersonID] [int] NULL,
	[TenantSquareLogoFileResourceInfoID] [int] NULL,
	[TenantBannerLogoFileResourceInfoID] [int] NULL,
	[TenantStyleSheetFileResourceInfoID] [int] NULL,
	[TenantShortDisplayName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ToolDisplayName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ShowProposalsToThePublic] [bit] NOT NULL,
	[TaxonomyLevelID] [int] NOT NULL,
	[AssociatePerfomanceMeasureTaxonomyLevelID] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[ProjectExternalDataSourceEnabled] [bit] NOT NULL,
	[AccomplishmentsDashboardFundingDisplayTypeID] [int] NOT NULL,
	[AccomplishmentsDashboardAccomplishmentsButtonText] [dbo].[html] NULL,
	[AccomplishmentsDashboardExpendituresButtonText] [dbo].[html] NULL,
	[AccomplishmentsDashboardOrganizationsButtonText] [dbo].[html] NULL,
	[AccomplishmentsDashboardIncludeReportingOrganizationType] [bit] NOT NULL,
	[ShowLeadImplementerLogoOnFactSheet] [bit] NOT NULL,
	[EnableAccomplishmentsDashboard] [bit] NOT NULL,
	[ProjectStewardshipAreaTypeID] [int] NULL,
	[EnableSecondaryProjectTaxonomyLeaf] [bit] NOT NULL,
	[KeystoneOpenIDClientIdentifier] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[KeystoneOpenIDClientSecret] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[BudgetTypeID] [int] NOT NULL,
	[CanManageCustomAttributes] [bit] NOT NULL,
	[ExcludeTargetedFundingOrganizations] [bit] NOT NULL,
	[GoogleAnalyticsTrackingCode] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[UseProjectTimeline] [bit] NOT NULL,
	[GeoServerNamespace] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[EnableEvaluations] [bit] NOT NULL,
	[EnableProjectCategories] [bit] NOT NULL,
	[EnableReports] [bit] NOT NULL,
	[TenantFactSheetLogoFileResourceInfoID] [int] NULL,
	[EnableMatchmaker] [bit] NOT NULL,
	[MatchmakerAlgorithmIncludesProjectGeospatialAreas] [bit] NOT NULL,
	[AreGeospatialAreasExternallySourced] [bit] NOT NULL,
	[ShowPhotoCreditOnFactSheet] [bit] NOT NULL,
	[TrackAccomplishments] [bit] NOT NULL,
	[ShowExpectedPerformanceMeasuresOnFactSheet] [bit] NOT NULL,
	[EnableStatusUpdates] [bit] NOT NULL,
	[EnableSolicitations] [bit] NOT NULL,
	[EnableSimpleAccomplishmentsDashboard] [bit] NOT NULL,
	[SetTargetsByGeospatialArea] [bit] NOT NULL,
	[ReportFinancialsAtProjectLevel] [bit] NOT NULL,
	[ProjectExternalDataSourceApiUrl] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProjectExternalSourceOfRecordName] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProjectExternalSourceOfRecordUrl] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AreReportsPublic] [bit] NOT NULL,
	[ProjectStewardshipVisibilityAdminOnly] [bit] NULL,
	[AccomplishmentsDashboardVisibilityAdminOnly] [bit] NOT NULL,
	[DisableProjectStewardOrganizationIndexPage] [bit] NULL,
	[EnableDetailedLocationPolygonsOnProjectMap] [bit] NULL,
	[ProjectDataMapLayerGroupName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ReferenceMapLayerGroupName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_TenantAttribute_TenantAttributeID] PRIMARY KEY CLUSTERED
(
	[TenantAttributeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_TenantAttribute_TenantID] UNIQUE NONCLUSTERED 
(
	[TenantID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_TenantAttribute_TenantShortDisplayName] UNIQUE NONCLUSTERED 
(
	[TenantShortDisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[TenantAttribute] ADD  DEFAULT ((0)) FOR [EnableProjectCategories]
GO
ALTER TABLE [dbo].[TenantAttribute] ADD  DEFAULT ((0)) FOR [MatchmakerAlgorithmIncludesProjectGeospatialAreas]
GO
ALTER TABLE [dbo].[TenantAttribute] ADD  DEFAULT ((0)) FOR [AreReportsPublic]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_AccomplishmentsDashboardFundingDisplayType_AccomplishmentsDashboardFundingDisplayTypeID] FOREIGN KEY([AccomplishmentsDashboardFundingDisplayTypeID])
REFERENCES [dbo].[AccomplishmentsDashboardFundingDisplayType] ([AccomplishmentsDashboardFundingDisplayTypeID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_AccomplishmentsDashboardFundingDisplayType_AccomplishmentsDashboardFundingDisplayTypeID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_BudgetType_BudgetTypeID] FOREIGN KEY([BudgetTypeID])
REFERENCES [dbo].[BudgetType] ([BudgetTypeID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_BudgetType_BudgetTypeID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantBannerLogoFileResourceInfoID_FileResourceInfoID] FOREIGN KEY([TenantBannerLogoFileResourceInfoID])
REFERENCES [dbo].[FileResourceInfo] ([FileResourceInfoID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantBannerLogoFileResourceInfoID_FileResourceInfoID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantBannerLogoFileResourceInfoID_TenantID_FileResourceInfoID_TenantID] FOREIGN KEY([TenantBannerLogoFileResourceInfoID], [TenantID])
REFERENCES [dbo].[FileResourceInfo] ([FileResourceInfoID], [TenantID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantBannerLogoFileResourceInfoID_TenantID_FileResourceInfoID_TenantID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantFactSheetLogoFileResourceInfoID_FileResourceInfoID] FOREIGN KEY([TenantFactSheetLogoFileResourceInfoID])
REFERENCES [dbo].[FileResourceInfo] ([FileResourceInfoID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantFactSheetLogoFileResourceInfoID_FileResourceInfoID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantFactSheetLogoFileResourceInfoID_TenantID_FileResourceInfoID_TenantID] FOREIGN KEY([TenantFactSheetLogoFileResourceInfoID], [TenantID])
REFERENCES [dbo].[FileResourceInfo] ([FileResourceInfoID], [TenantID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantFactSheetLogoFileResourceInfoID_TenantID_FileResourceInfoID_TenantID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantSquareLogoFileResourceInfoID_FileResourceInfoID] FOREIGN KEY([TenantSquareLogoFileResourceInfoID])
REFERENCES [dbo].[FileResourceInfo] ([FileResourceInfoID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantSquareLogoFileResourceInfoID_FileResourceInfoID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantSquareLogoFileResourceInfoID_TenantID_FileResourceInfoID_TenantID] FOREIGN KEY([TenantSquareLogoFileResourceInfoID], [TenantID])
REFERENCES [dbo].[FileResourceInfo] ([FileResourceInfoID], [TenantID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantSquareLogoFileResourceInfoID_TenantID_FileResourceInfoID_TenantID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantStyleSheetFileResourceInfoID_FileResourceInfoID] FOREIGN KEY([TenantStyleSheetFileResourceInfoID])
REFERENCES [dbo].[FileResourceInfo] ([FileResourceInfoID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantStyleSheetFileResourceInfoID_FileResourceInfoID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantStyleSheetFileResourceInfoID_TenantID_FileResourceInfoID_TenantID] FOREIGN KEY([TenantStyleSheetFileResourceInfoID], [TenantID])
REFERENCES [dbo].[FileResourceInfo] ([FileResourceInfoID], [TenantID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_FileResourceInfo_TenantStyleSheetFileResourceInfoID_TenantID_FileResourceInfoID_TenantID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_Person_PrimaryContactPersonID_PersonID] FOREIGN KEY([PrimaryContactPersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_Person_PrimaryContactPersonID_PersonID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_Person_PrimaryContactPersonID_TenantID_PersonID_TenantID] FOREIGN KEY([PrimaryContactPersonID], [TenantID])
REFERENCES [dbo].[Person] ([PersonID], [TenantID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_Person_PrimaryContactPersonID_TenantID_PersonID_TenantID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_ProjectStewardshipAreaType_ProjectStewardshipAreaTypeID] FOREIGN KEY([ProjectStewardshipAreaTypeID])
REFERENCES [dbo].[ProjectStewardshipAreaType] ([ProjectStewardshipAreaTypeID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_ProjectStewardshipAreaType_ProjectStewardshipAreaTypeID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_TaxonomyLevel_AssociatePerfomanceMeasureTaxonomyLevelID_TaxonomyLevelID] FOREIGN KEY([AssociatePerfomanceMeasureTaxonomyLevelID])
REFERENCES [dbo].[TaxonomyLevel] ([TaxonomyLevelID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_TaxonomyLevel_AssociatePerfomanceMeasureTaxonomyLevelID_TaxonomyLevelID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_TaxonomyLevel_TaxonomyLevelID] FOREIGN KEY([TaxonomyLevelID])
REFERENCES [dbo].[TaxonomyLevel] ([TaxonomyLevelID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_TaxonomyLevel_TaxonomyLevelID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [FK_TenantAttribute_Tenant_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([TenantID])
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [FK_TenantAttribute_Tenant_TenantID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [CK_TenantAttribute_AssociatedPerfomanceMeasureTaxonomyLevelLessThanEqualToTaxonomyLevelID] CHECK  (([AssociatePerfomanceMeasureTaxonomyLevelID]<=[TaxonomyLevelID]))
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [CK_TenantAttribute_AssociatedPerfomanceMeasureTaxonomyLevelLessThanEqualToTaxonomyLevelID]
GO
ALTER TABLE [dbo].[TenantAttribute]  WITH CHECK ADD  CONSTRAINT [CK_TenantAttribute_DefaultBoundingBox_SpatialReferenceID_Must_Be_4326] CHECK  (([DefaultBoundingBox] IS NULL OR [DefaultBoundingBox].[STSrid]=(4326)))
GO
ALTER TABLE [dbo].[TenantAttribute] CHECK CONSTRAINT [CK_TenantAttribute_DefaultBoundingBox_SpatialReferenceID_Must_Be_4326]
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO
CREATE SPATIAL INDEX [SPATIAL_TenantAttribute_DefaultBoundingBox] ON [dbo].[TenantAttribute]
(
	[DefaultBoundingBox]
)USING  GEOMETRY_AUTO_GRID 
WITH (BOUNDING_BOX =(-125, 32, -104, 50), 
CELLS_PER_OBJECT = 8, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]