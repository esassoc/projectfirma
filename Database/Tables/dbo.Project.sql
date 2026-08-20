SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Project](
	[ProjectID] [int] IDENTITY(1,1) NOT NULL,
	[TenantID] [int] NOT NULL,
	[TaxonomyLeafID] [int] NOT NULL,
	[ProjectStageID] [int] NOT NULL,
	[ProjectName] [varchar](140) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ProjectDescription] [varchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ImplementationStartYear] [int] NULL,
	[CompletionYear] [int] NULL,
	[EstimatedTotalCostDeprecated] [money] NULL,
	[ProjectLocationPoint] [geometry] NULL,
	[PerformanceMeasureActualYearsExemptionExplanation] [varchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsFeatured] [bit] NOT NULL,
	[ProjectLocationNotes] [varchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PlanningDesignStartYear] [int] NULL,
	[ProjectLocationSimpleTypeID] [int] NOT NULL,
	[EstimatedAnnualOperatingCostDeprecated] [money] NULL,
	[FundingTypeID] [int] NULL,
	[PrimaryContactPersonID] [int] NULL,
	[ProjectApprovalStatusID] [int] NOT NULL,
	[ProposingPersonID] [int] NULL,
	[ProposingDate] [datetime] NULL,
	[PerformanceMeasureNotes] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SubmissionDate] [datetime] NULL,
	[ApprovalDate] [datetime] NULL,
	[ReviewedByPersonID] [int] NULL,
	[DefaultBoundingBox] [geometry] NULL,
	[ExpendituresNote] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExpectedFundingUpdateNote] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
	[ProjectCategoryID] [int] NOT NULL,
	[BasicsComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CustomAttributesComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LocationSimpleComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LocationDetailedComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OrganizationsComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ContactsComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExpectedAccomplishmentsComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ReportedAccomplishmentsComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[BudgetComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExpendituresComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProposalClassificationsComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AttachmentsNotesComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PhotosComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SubmittedByPersonID] [int] NULL,
	[LocationIsPrivate] [bit] NOT NULL,
	[SolicitationID] [int] NULL,
	[OtherPartners] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExternalLinksComment] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExternalID] [int] NULL,
	[PrimaryContactPersonFullName] [varchar](201) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PrimaryContactPersonEmail] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[GisAcres] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Project_ProjectID] PRIMARY KEY CLUSTERED 
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_Project_ProjectID_TenantID] UNIQUE NONCLUSTERED 
(
	[ProjectID] ASC,
	[TenantID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_Project_ProjectName_TenantID] UNIQUE NONCLUSTERED 
(
	[ProjectName] ASC,
	[TenantID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [IDX_Project_TaxonomyLeaf] ON [dbo].[Project]
(
	[TaxonomyLeafID] ASC
)
INCLUDE([TenantID],[ProjectName],[PrimaryContactPersonID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Project] ADD  DEFAULT ((1)) FOR [ProjectCategoryID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_FundingType_FundingTypeID] FOREIGN KEY([FundingTypeID])
REFERENCES [dbo].[FundingType] ([FundingTypeID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_FundingType_FundingTypeID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Person_PrimaryContactPersonID_PersonID] FOREIGN KEY([PrimaryContactPersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Person_PrimaryContactPersonID_PersonID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Person_PrimaryContactPersonID_TenantID_PersonID_TenantID] FOREIGN KEY([PrimaryContactPersonID], [TenantID])
REFERENCES [dbo].[Person] ([PersonID], [TenantID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Person_PrimaryContactPersonID_TenantID_PersonID_TenantID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Person_ProposingPersonID_PersonID] FOREIGN KEY([ProposingPersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Person_ProposingPersonID_PersonID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Person_ProposingPersonID_TenantID_PersonID_TenantID] FOREIGN KEY([ProposingPersonID], [TenantID])
REFERENCES [dbo].[Person] ([PersonID], [TenantID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Person_ProposingPersonID_TenantID_PersonID_TenantID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Person_ReviewedByPersonID_PersonID] FOREIGN KEY([ReviewedByPersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Person_ReviewedByPersonID_PersonID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Person_ReviewedByPersonID_TenantID_PersonID_TenantID] FOREIGN KEY([ReviewedByPersonID], [TenantID])
REFERENCES [dbo].[Person] ([PersonID], [TenantID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Person_ReviewedByPersonID_TenantID_PersonID_TenantID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Person_SubmittedByPersonID_PersonID] FOREIGN KEY([SubmittedByPersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Person_SubmittedByPersonID_PersonID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Person_SubmittedByPersonID_TenantID_PersonID_TenantID] FOREIGN KEY([SubmittedByPersonID], [TenantID])
REFERENCES [dbo].[Person] ([PersonID], [TenantID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Person_SubmittedByPersonID_TenantID_PersonID_TenantID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_ProjectApprovalStatus_ProjectApprovalStatusID] FOREIGN KEY([ProjectApprovalStatusID])
REFERENCES [dbo].[ProjectApprovalStatus] ([ProjectApprovalStatusID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_ProjectApprovalStatus_ProjectApprovalStatusID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_ProjectCategory_ProjectCategoryID] FOREIGN KEY([ProjectCategoryID])
REFERENCES [dbo].[ProjectCategory] ([ProjectCategoryID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_ProjectCategory_ProjectCategoryID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_ProjectLocationSimpleType_ProjectLocationSimpleTypeID] FOREIGN KEY([ProjectLocationSimpleTypeID])
REFERENCES [dbo].[ProjectLocationSimpleType] ([ProjectLocationSimpleTypeID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_ProjectLocationSimpleType_ProjectLocationSimpleTypeID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_ProjectStage_ProjectStageID] FOREIGN KEY([ProjectStageID])
REFERENCES [dbo].[ProjectStage] ([ProjectStageID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_ProjectStage_ProjectStageID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Solicitation_SolicitationID] FOREIGN KEY([SolicitationID])
REFERENCES [dbo].[Solicitation] ([SolicitationID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Solicitation_SolicitationID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Solicitation_SolicitationID_TenantID] FOREIGN KEY([SolicitationID], [TenantID])
REFERENCES [dbo].[Solicitation] ([SolicitationID], [TenantID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Solicitation_SolicitationID_TenantID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_TaxonomyLeaf_TaxonomyLeafID] FOREIGN KEY([TaxonomyLeafID])
REFERENCES [dbo].[TaxonomyLeaf] ([TaxonomyLeafID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_TaxonomyLeaf_TaxonomyLeafID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_TaxonomyLeaf_TaxonomyLeafID_TenantID] FOREIGN KEY([TaxonomyLeafID], [TenantID])
REFERENCES [dbo].[TaxonomyLeaf] ([TaxonomyLeafID], [TenantID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_TaxonomyLeaf_TaxonomyLeafID_TenantID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Tenant_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([TenantID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Tenant_TenantID]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [CK_Project_CompletionYearHasToBeSetWhenStageIsInCompletedOrPostImplementation] CHECK  ((([ProjectStageID]=(8) OR [ProjectStageID]=(4)) AND [CompletionYear] IS NOT NULL OR NOT ([ProjectStageID]=(8) OR [ProjectStageID]=(4))))
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [CK_Project_CompletionYearHasToBeSetWhenStageIsInCompletedOrPostImplementation]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [CK_Project_DefaultBoundingBox_SpatialReferenceID_Must_Be_4326] CHECK  (([DefaultBoundingBox] IS NULL OR [DefaultBoundingBox].[STSrid]=(4326)))
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [CK_Project_DefaultBoundingBox_SpatialReferenceID_Must_Be_4326]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [CK_Project_ImplementationStartYearLessThanEqualToCompletionYear] CHECK  (([ImplementationStartYear] IS NULL OR [CompletionYear] IS NULL OR [CompletionYear]>=[ImplementationStartYear]))
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [CK_Project_ImplementationStartYearLessThanEqualToCompletionYear]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [CK_Project_PlanningDesignStartYearLessThanEqualToImplementationYear] CHECK  (([PlanningDesignStartYear] IS NULL OR [ImplementationStartYear] IS NULL OR [ImplementationStartYear]>=[PlanningDesignStartYear]))
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [CK_Project_PlanningDesignStartYearLessThanEqualToImplementationYear]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [CK_Project_ProjectCannotHaveProjectStageProposalAndApprovalStatusApproved] CHECK  (([ProjectStageID]<>(1) OR [ProjectApprovalStatusID]<>(3)))
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [CK_Project_ProjectCannotHaveProjectStageProposalAndApprovalStatusApproved]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [CK_Project_ProjectLocationPoint_IsPointData] CHECK  (([ProjectLocationPoint] IS NULL OR [ProjectLocationPoint] IS NOT NULL AND [ProjectLocationPoint].[STGeometryType]()='Point'))
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [CK_Project_ProjectLocationPoint_IsPointData]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [CK_Project_ProjectLocationPoint_SpatialReferenceID_Must_Be_4326] CHECK  (([ProjectLocationPoint] IS NULL OR [ProjectLocationPoint].[STSrid]=(4326)))
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [CK_Project_ProjectLocationPoint_SpatialReferenceID_Must_Be_4326]
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO
CREATE SPATIAL INDEX [SPATIAL_Project_DefaultBoundingBox] ON [dbo].[Project]
(
	[DefaultBoundingBox]
)USING  GEOMETRY_AUTO_GRID 
WITH (BOUNDING_BOX =(-126, 34, -111, 50), 
CELLS_PER_OBJECT = 8, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO
CREATE SPATIAL INDEX [SPATIAL_Project_ProjectLocationPoint] ON [dbo].[Project]
(
	[ProjectLocationPoint]
)USING  GEOMETRY_AUTO_GRID 
WITH (BOUNDING_BOX =(-126, 33, -104, 340), 
CELLS_PER_OBJECT = 8, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]