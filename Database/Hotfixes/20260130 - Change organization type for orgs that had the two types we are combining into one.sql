SELECT TOP (1000) [OrganizationTypeID]
      ,[TenantID]
      ,[OrganizationTypeName]
      ,[OrganizationTypeAbbreviation]
      ,[LegendColor]
      ,[ShowOnProjectMaps]
      ,[IsDefaultOrganizationType]
      ,[IsFundingType]
      ,[LayerOnByDefault]
  FROM [ProjectFirma].[dbo].[OrganizationType]
  where tenantid = 4

  
update dbo.Organization 
set OrganizationTypeID = 1128
where TenantID = 4 and OrganizationTypeID in (1106, 1116)