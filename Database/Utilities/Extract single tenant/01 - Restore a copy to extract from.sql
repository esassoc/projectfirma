/*
    Single-tenant extract, step 1 of 3: restore a throwaway copy of ProjectFirma.

    Never run steps 2-3 against a real ProjectFirma database. Step 2 refuses to run in a
    database named ProjectFirma, and requires you to type the copy's name.

    Run in SSMS (or sqlcmd) connected to master. Edit the variables below.
*/
USE master;
GO

DECLARE @BackupFile     nvarchar(4000) = N'C:\Sitka\ProjectFirma\Backup\ProjectFirma-prod-data.bak';
DECLARE @TargetDatabase sysname        = N'ProjectFirmaNCRP';
DECLARE @DataFolder     nvarchar(4000) = N'C:\Sitka\ProjectFirma\SqlData\';

IF @TargetDatabase = N'ProjectFirma'
    THROW 50000, 'Pick a target name other than ProjectFirma - this copy gets most of its data deleted.', 1;

IF DB_ID(@TargetDatabase) IS NOT NULL
    THROW 50000, 'Target database already exists. Drop it first if you mean to start over.', 1;

DECLARE @sql nvarchar(max) = N'
RESTORE DATABASE ' + QUOTENAME(@TargetDatabase) + N'
FROM DISK = @BackupFile
WITH MOVE N''ProjectFirma''     TO @DataFile,
     MOVE N''ProjectFirma_Log'' TO @LogFile,
     STATS = 5;';

DECLARE @DataFile nvarchar(4000) = @DataFolder + @TargetDatabase + N'.mdf';
DECLARE @LogFile  nvarchar(4000) = @DataFolder + @TargetDatabase + N'_log.ldf';

EXEC sp_executesql @sql,
    N'@BackupFile nvarchar(4000), @DataFile nvarchar(4000), @LogFile nvarchar(4000)',
    @BackupFile, @DataFile, @LogFile;

-- The copy is disposable; simple recovery keeps the purge in step 2 from ballooning the log.
SET @sql = N'ALTER DATABASE ' + QUOTENAME(@TargetDatabase) + N' SET RECOVERY SIMPLE;';
EXEC (@sql);
GO
