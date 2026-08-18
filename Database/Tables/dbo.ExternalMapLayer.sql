SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExternalMapLayer](
	[ExternalMapLayerID] [int] IDENTITY(1,1) NOT NULL,
	[TenantID] [int] NOT NULL,
	[DisplayName] [varchar](75) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LayerUrl] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LayerDescription] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FeatureNameField] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DisplayOnAllProjectMaps] [bit] NOT NULL,
	[LayerIsOnByDefault] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsTiledMapService] [bit] NOT NULL,
	[MapLegendImageFileResourceInfoID] [int] NULL,
	[MapLayerGroupName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SortOrder] [int] NULL,
 CONSTRAINT [PK_ExternalMapLayer_ExternalMapLayerID] PRIMARY KEY CLUSTERED 
(
	[ExternalMapLayerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_ExternalMapLayer_TenantID_DisplayName] UNIQUE NONCLUSTERED 
(
	[TenantID] ASC,
	[DisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_ExternalMapLayer_TenantID_LayerUrl] UNIQUE NONCLUSTERED 
(
	[TenantID] ASC,
	[LayerUrl] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ExternalMapLayer]  WITH CHECK ADD  CONSTRAINT [FK_ExternalMapLayer_FileResourceInfo_MapLegendImageFileResourceInfoID_FileResourceInfoID] FOREIGN KEY([MapLegendImageFileResourceInfoID])
REFERENCES [dbo].[FileResourceInfo] ([FileResourceInfoID])
GO
ALTER TABLE [dbo].[ExternalMapLayer] CHECK CONSTRAINT [FK_ExternalMapLayer_FileResourceInfo_MapLegendImageFileResourceInfoID_FileResourceInfoID]
GO
ALTER TABLE [dbo].[ExternalMapLayer]  WITH CHECK ADD  CONSTRAINT [FK_ExternalMapLayer_FileResourceInfo_MapLegendImageFileResourceInfoID_TenantID_FileResourceInfoID_TenantID] FOREIGN KEY([MapLegendImageFileResourceInfoID], [TenantID])
REFERENCES [dbo].[FileResourceInfo] ([FileResourceInfoID], [TenantID])
GO
ALTER TABLE [dbo].[ExternalMapLayer] CHECK CONSTRAINT [FK_ExternalMapLayer_FileResourceInfo_MapLegendImageFileResourceInfoID_TenantID_FileResourceInfoID_TenantID]
GO
ALTER TABLE [dbo].[ExternalMapLayer]  WITH CHECK ADD  CONSTRAINT [FK_ExternalMapLayer_Tenant_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([TenantID])
GO
ALTER TABLE [dbo].[ExternalMapLayer] CHECK CONSTRAINT [FK_ExternalMapLayer_Tenant_TenantID]