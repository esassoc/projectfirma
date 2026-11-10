/*
    Single-tenant extract, step 3 of 3: verify, reclaim space, and write a compressed backup.

    Run while connected to the purged COPY (e.g. ProjectFirmaNCRP).

    Shrinking matters: RESTORE recreates data files at their original size, so without it the
    recipient would need ~55 GB of disk to restore a ~3 GB database.
*/
-- Required by the spatial/filtered indexes (sqlcmd defaults QUOTED_IDENTIFIER to OFF).
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO
SET NOCOUNT ON;

DECLARE @KeepTenantID    int            = 4;
DECLARE @ConfirmDatabase sysname        = N'ProjectFirmaNCRP';
DECLARE @BackupFile      nvarchar(4000) = N'C:\Sitka\ProjectFirma\Backup\ProjectFirmaNCRP.bak';

IF DB_NAME() = N'ProjectFirma' OR DB_NAME() <> @ConfirmDatabase
    THROW 50000, 'Connect to the extract copy and set @ConfirmDatabase to its name.', 1;

------------------------------------------------------------------------------------------
-- 1. Verify: no rows for other tenants remain
------------------------------------------------------------------------------------------
DECLARE @sql nvarchar(max) = N'';
SELECT @sql += N'UNION ALL SELECT N''' + s.name + N'.' + t.name + N''', COUNT_BIG(*) FROM '
             + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) + N' WHERE TenantID <> @KeepTenantID '
FROM sys.tables t
JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE EXISTS (SELECT 1 FROM sys.columns c WHERE c.object_id = t.object_id AND c.name = N'TenantID')
  AND t.object_id <> OBJECT_ID(N'dbo.Tenant');
SET @sql = N'SELECT TableName, Leftover FROM (' + STUFF(@sql, 1, 10, N'') + N') x(TableName, Leftover) WHERE Leftover > 0;';

DECLARE @Leftovers TABLE (TableName nvarchar(300), Leftover bigint);
INSERT @Leftovers EXEC sp_executesql @sql, N'@KeepTenantID int', @KeepTenantID;
IF EXISTS (SELECT 1 FROM @Leftovers)
BEGIN
    SELECT * FROM @Leftovers;
    THROW 50000, 'Other tenants still have rows (listed above). Re-run step 2.', 1;
END
RAISERROR('Verified: no rows for other tenants remain.', 0, 1) WITH NOWAIT;

------------------------------------------------------------------------------------------
-- 2. Reclaim space
------------------------------------------------------------------------------------------
-- FileResourceInfo carries ~5 GB of LOB pages from a long-dropped blob column.
DBCC CLEANTABLE (0, N'dbo.FileResourceInfo', 1000) WITH NO_INFOMSGS;
ALTER INDEX ALL ON dbo.FileResourceInfo REORGANIZE WITH (LOB_COMPACTION = ON);
ALTER INDEX ALL ON dbo.FileResourceData REORGANIZE WITH (LOB_COMPACTION = ON);
CHECKPOINT;

DECLARE @DataFile sysname = (SELECT name FROM sys.database_files WHERE type_desc = 'ROWS');
DECLARE @LogFile  sysname = (SELECT name FROM sys.database_files WHERE type_desc = 'LOG');
RAISERROR('Shrinking data file (can take a while)...', 0, 1) WITH NOWAIT;
DBCC SHRINKFILE (@DataFile, 0) WITH NO_INFOMSGS;
DBCC SHRINKFILE (@LogFile, 64) WITH NO_INFOMSGS;

-- Shrink fragments indexes; rebuild them now that the data is small.
-- (Not sp_MSforeachtable: it forces QUOTED_IDENTIFIER OFF, which spatial indexes reject.)
DECLARE @tbl nvarchar(300);
DECLARE c CURSOR LOCAL FAST_FORWARD FOR
    SELECT QUOTENAME(s.name) + N'.' + QUOTENAME(t.name)
    FROM sys.tables t JOIN sys.schemas s ON s.schema_id = t.schema_id WHERE t.is_ms_shipped = 0;
OPEN c; FETCH NEXT FROM c INTO @tbl;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = N'ALTER INDEX ALL ON ' + @tbl + N' REBUILD;';
    EXEC (@sql);
    FETCH NEXT FROM c INTO @tbl;
END
CLOSE c; DEALLOCATE c;

EXEC sp_spaceused;

------------------------------------------------------------------------------------------
-- 3. Back up
------------------------------------------------------------------------------------------
SET @sql = N'BACKUP DATABASE ' + QUOTENAME(DB_NAME()) + N' TO DISK = @BackupFile
             WITH COPY_ONLY, COMPRESSION, CHECKSUM, INIT, FORMAT, STATS = 10;';
EXEC sp_executesql @sql, N'@BackupFile nvarchar(4000)', @BackupFile;

RESTORE VERIFYONLY FROM DISK = @BackupFile WITH CHECKSUM;
GO
