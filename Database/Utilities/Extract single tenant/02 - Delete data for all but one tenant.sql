/*
    Single-tenant extract, step 2 of 3: delete every other tenant's data.

    Run while connected to the restored COPY from step 1 (e.g. ProjectFirmaNCRP).

    How it works - schema-driven, so it keeps working as tables are added:
      1. Disables all FK/check constraints so delete order doesn't matter.
      2. Deletes rows WHERE TenantID <> @KeepTenantID from every table that has a TenantID
         column, in batches (except dbo.Tenant: those rows are just names/hostnames and the
         app's Tenant enum expects them; other tenants are marked TenantEnabled = 0).
      3. Cleans up the few global tables that point at tenant rows (ReleaseNote -> Person)
         or may hold other tenants' job payloads (HangFire).
      4. Re-enables every constraint WITH CHECK, which re-validates all remaining rows.
         Any failure there means a kept row points at a deleted row - it is reported
         and the constraint is left untrusted so you can investigate.

    Tenants: 1 Sitka, 2 Clackamas, 3 RCD, 4 NCRP, 5 Demo, 6 PeaksToPeople, 7 JohnDay,
             8 Ashland, 9 IASCD, 11 PugetSound, 12 BOR, 13 SSMP, 14 TCS
*/
-- Required by the spatial/filtered indexes (sqlcmd defaults QUOTED_IDENTIFIER to OFF).
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @KeepTenantID     int     = 4;                  -- NCRPProjectTracker
DECLARE @ConfirmDatabase  sysname = N'ProjectFirmaNCRP'; -- must match the database you're connected to

------------------------------------------------------------------------------------------
-- Safety checks
------------------------------------------------------------------------------------------
IF DB_NAME() = N'ProjectFirma'
    THROW 50000, 'Refusing to run in ProjectFirma. Restore a copy (step 1) and run this there.', 1;
IF DB_NAME() <> @ConfirmDatabase
    THROW 50000, 'Connected database does not match @ConfirmDatabase. Check your connection.', 1;
IF NOT EXISTS (SELECT 1 FROM dbo.Tenant WHERE TenantID = @KeepTenantID)
    THROW 50000, '@KeepTenantID does not exist in dbo.Tenant.', 1;

DECLARE @KeepTenantName varchar(100) = (SELECT TenantName FROM dbo.Tenant WHERE TenantID = @KeepTenantID);
RAISERROR('Keeping only TenantID %d (%s) in %s', 0, 1, @KeepTenantID, @KeepTenantName, @ConfirmDatabase) WITH NOWAIT;

DECLARE @sql nvarchar(max), @tbl nvarchar(300), @rows bigint, @total bigint, @batch int;

------------------------------------------------------------------------------------------
-- 1. Disable every FK and check constraint
------------------------------------------------------------------------------------------
DECLARE @AllTables TABLE (FullName nvarchar(300) PRIMARY KEY);
INSERT @AllTables
SELECT QUOTENAME(s.name) + N'.' + QUOTENAME(t.name)
FROM sys.tables t JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE t.is_ms_shipped = 0;

DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT FullName FROM @AllTables;
OPEN c; FETCH NEXT FROM c INTO @tbl;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = N'ALTER TABLE ' + @tbl + N' NOCHECK CONSTRAINT ALL;';
    EXEC (@sql);
    FETCH NEXT FROM c INTO @tbl;
END
CLOSE c; DEALLOCATE c;
RAISERROR('Constraints disabled.', 0, 1) WITH NOWAIT;

------------------------------------------------------------------------------------------
-- 2. Delete other tenants' rows from every TenantID table
------------------------------------------------------------------------------------------
DECLARE @TenantTables TABLE (FullName nvarchar(300) PRIMARY KEY, HasLob bit);
INSERT @TenantTables
SELECT QUOTENAME(s.name) + N'.' + QUOTENAME(t.name),
       CASE WHEN EXISTS (SELECT 1 FROM sys.columns lc
                         WHERE lc.object_id = t.object_id AND lc.max_length = -1) THEN 1 ELSE 0 END
FROM sys.tables t
JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE EXISTS (SELECT 1 FROM sys.columns c WHERE c.object_id = t.object_id AND c.name = N'TenantID')
  AND t.object_id <> OBJECT_ID(N'dbo.Tenant');

DECLARE c CURSOR LOCAL FAST_FORWARD FOR
    SELECT FullName, CASE WHEN HasLob = 1 THEN 500 ELSE 50000 END FROM @TenantTables ORDER BY FullName;
OPEN c; FETCH NEXT FROM c INTO @tbl, @batch;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @total = 0;
    SET @sql = N'DELETE TOP (@batch) FROM ' + @tbl + N' WHERE TenantID <> @KeepTenantID; SET @rows = @@ROWCOUNT;';
    WHILE 1 = 1
    BEGIN
        EXEC sp_executesql @sql, N'@batch int, @KeepTenantID int, @rows bigint OUTPUT', @batch, @KeepTenantID, @rows OUTPUT;
        SET @total += @rows;
        IF @rows < @batch BREAK;
        CHECKPOINT; -- simple recovery: lets log space be reused between batches
    END
    IF @total > 0
        RAISERROR('  %-60s %I64d rows deleted', 0, 1, @tbl, @total) WITH NOWAIT;
    FETCH NEXT FROM c INTO @tbl, @batch;
END
CLOSE c; DEALLOCATE c;

------------------------------------------------------------------------------------------
-- 3. Global (no TenantID) tables that reference or may contain other tenants' data
------------------------------------------------------------------------------------------
-- ReleaseNote is global but FKs to Person; drop notes whose author/editor was deleted.
DELETE rn FROM dbo.ReleaseNote rn
WHERE NOT EXISTS (SELECT 1 FROM dbo.Person p WHERE p.PersonID = rn.CreatePersonID)
   OR (rn.UpdatePersonID IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.Person p WHERE p.PersonID = rn.UpdatePersonID));
SET @rows = @@ROWCOUNT;
RAISERROR('  [dbo].[ReleaseNote] orphans deleted: %I64d', 0, 1, @rows) WITH NOWAIT;

-- HangFire job payloads can carry other tenants' IDs/emails; the extract doesn't need job history.
IF SCHEMA_ID(N'HangFire') IS NOT NULL
BEGIN
    DELETE FROM HangFire.JobParameter;
    DELETE FROM HangFire.State;
    DELETE FROM HangFire.JobQueue;
    DELETE FROM HangFire.Job;
    DELETE FROM HangFire.[Hash];
    DELETE FROM HangFire.[Set];
    DELETE FROM HangFire.[List];
    DELETE FROM HangFire.Counter;
    DELETE FROM HangFire.AggregatedCounter;
    DELETE FROM HangFire.[Server];
    RAISERROR('  HangFire tables cleared.', 0, 1) WITH NOWAIT;
END

-- Keep other Tenant rows (the app's Tenant enum expects them) but switch them off.
UPDATE dbo.Tenant SET TenantEnabled = 0 WHERE TenantID <> @KeepTenantID;

-- Remove the old Keystone sign-in client ID/secret; whoever receives the extract shouldn't hold them.
-- Only read when AuthenticationType = KeystoneAuth (FirmaOwinStartup.cs); every environment uses Auth0Auth.
-- Columns are NOT NULL, so blank rather than NULL. Step 04 skips these two columns when comparing.
UPDATE dbo.TenantAttribute
SET KeystoneOpenIDClientIdentifier = '', KeystoneOpenIDClientSecret = ''
WHERE KeystoneOpenIDClientIdentifier <> '' OR KeystoneOpenIDClientSecret <> '';
SET @rows = @@ROWCOUNT;
RAISERROR('  [dbo].[TenantAttribute] Keystone client ID/secret blanked: %I64d row(s)', 0, 1, @rows) WITH NOWAIT;

------------------------------------------------------------------------------------------
-- 4. Re-enable and re-validate every constraint
------------------------------------------------------------------------------------------
RAISERROR('Re-enabling constraints WITH CHECK...', 0, 1) WITH NOWAIT;
DECLARE @Failures TABLE (TableName nvarchar(300), ErrorMessage nvarchar(4000));

DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT FullName FROM @AllTables;
OPEN c; FETCH NEXT FROM c INTO @tbl;
WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY
        SET @sql = N'ALTER TABLE ' + @tbl + N' WITH CHECK CHECK CONSTRAINT ALL;';
        EXEC (@sql);
    END TRY
    BEGIN CATCH
        INSERT @Failures VALUES (@tbl, ERROR_MESSAGE());
        -- Re-enable without validating so the constraint at least enforces new writes.
        SET @sql = N'ALTER TABLE ' + @tbl + N' CHECK CONSTRAINT ALL;';
        EXEC (@sql);
    END CATCH
    FETCH NEXT FROM c INTO @tbl;
END
CLOSE c; DEALLOCATE c;

------------------------------------------------------------------------------------------
-- 5. Report
------------------------------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM @Failures)
BEGIN
    RAISERROR('CONSTRAINT VALIDATION FAILURES - kept rows reference deleted rows:', 0, 1) WITH NOWAIT;
    SELECT * FROM @Failures;
END
ELSE
    RAISERROR('All constraints re-enabled and validated.', 0, 1) WITH NOWAIT;

-- Anything still untrusted/disabled (should be empty)
SELECT OBJECT_NAME(parent_object_id) AS TableName, name AS ConstraintName, is_disabled, is_not_trusted
FROM sys.foreign_keys WHERE is_disabled = 1 OR is_not_trusted = 1
UNION ALL
SELECT OBJECT_NAME(parent_object_id), name, is_disabled, is_not_trusted
FROM sys.check_constraints WHERE is_disabled = 1 OR is_not_trusted = 1;
GO
