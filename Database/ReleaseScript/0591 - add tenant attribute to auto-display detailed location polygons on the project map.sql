
alter table dbo.TenantAttribute add EnableDetailedLocationPolygonsOnProjectMap bit null
go

-- PF-2829: enable for TCS Project Tracker; other tenants remain off (null/0)
update dbo.TenantAttribute set EnableDetailedLocationPolygonsOnProjectMap = 1 where TenantID = 14
