/*
    Single-tenant extract, optional step 4: prove the kept tenant's data came through unchanged.

    Restore BOTH of these on the same server first:
      - the original backup the extract was made from (e.g. as ProjectFirmaSource)
      - the extract backup from step 3 (e.g. as ProjectFirmaNCRPCheck)

    For every table it compares, between the two databases:
      - tables with TenantID: the kept tenant's row count and content checksum
      - tables without TenantID: the whole table's row count and content checksum

    The content checksum covers every column: BINARY_CHECKSUM(*) for ordinary columns, plus a
    SHA2-256 hash of each column BINARY_CHECKSUM skips or under-reports: all text columns (it
    ignores trailing spaces), (max) types, xml, geometry and geography - so file bytes and map
    shapes are compared too. A deliberate tamper test (one byte in a file, a trailing space in a
    name, a nudged shape, one deleted row) was flagged on every count.

    Expected differences (from step 2, by design): dbo.ReleaseNote (orphaned notes removed)
    and the HangFire tables (job history cleared). Anything else is a problem.

    Columns step 2 deliberately blanks (the Keystone client ID/secret) are left out of the
    comparison, so the rest of their table still has to match exactly; the script then checks
    separately that they are blank in the extract.
*/
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO
SET NOCOUNT ON;

DECLARE @KeepTenantID    int     = 4;
DECLARE @SourceDatabase  sysname = N'ProjectFirmaSource';
DECLARE @ExtractDatabase sysname = N'ProjectFirmaNCRPCheck';

IF DB_ID(@SourceDatabase) IS NULL OR DB_ID(@ExtractDatabase) IS NULL
    THROW 50000, 'Restore both databases and set @SourceDatabase / @ExtractDatabase.', 1;

-- Columns step 2 blanks on purpose: excluded from the comparison, checked separately below.
CREATE TABLE #Ignored (SchemaName sysname, TableName sysname, ColumnName sysname);
INSERT #Ignored VALUES (N'dbo', N'TenantAttribute', N'KeystoneOpenIDClientIdentifier'),
                       (N'dbo', N'TenantAttribute', N'KeystoneOpenIDClientSecret');

-- Every column goes into exactly one of two expressions (ignored columns into neither):
--   HashExpr: SHA2-256 of text, (max), xml, spatial and legacy LOB columns
--   OrdExpr:  BINARY_CHECKSUM over all other columns, listed explicitly
CREATE TABLE #Columns (SchemaName sysname, TableName sysname, ColumnName sysname, IsHashed bit);
DECLARE @sql nvarchar(max) = N'
INSERT #Columns
SELECT s.name, t.name, c.name,
       CASE WHEN c.max_length = -1
              OR ty.name IN (N''xml'', N''geometry'', N''geography'', N''hierarchyid'', N''char'', N''varchar'',
                             N''nchar'', N''nvarchar'', N''text'', N''ntext'', N''image'') THEN 1 ELSE 0 END
FROM ' + QUOTENAME(@SourceDatabase) + N'.sys.tables t
JOIN ' + QUOTENAME(@SourceDatabase) + N'.sys.schemas s ON s.schema_id = t.schema_id
JOIN ' + QUOTENAME(@SourceDatabase) + N'.sys.columns c ON c.object_id = t.object_id
JOIN ' + QUOTENAME(@SourceDatabase) + N'.sys.types ty ON ty.user_type_id = c.user_type_id
WHERE t.is_ms_shipped = 0
  AND NOT EXISTS (SELECT 1 FROM #Ignored i
                  WHERE i.SchemaName = s.name AND i.TableName = t.name AND i.ColumnName = c.name);';
EXEC (@sql);

CREATE TABLE #Tables (SchemaName sysname, TableName sysname, HasTenant bit, OrdExpr nvarchar(max), LobExpr nvarchar(max));
INSERT #Tables
SELECT SchemaName, TableName,
       MAX(CASE WHEN ColumnName = N'TenantID' THEN 1 ELSE 0 END),
       STRING_AGG(CASE WHEN IsHashed = 0 THEN CONVERT(nvarchar(max), QUOTENAME(ColumnName)) END, N', '),
       STRING_AGG(CASE WHEN IsHashed = 1 THEN CONVERT(nvarchar(max),
                  N'HASHBYTES(''SHA2_256'', CAST(' + QUOTENAME(ColumnName) + N' AS varbinary(max)))') END, N', ')
FROM #Columns
GROUP BY SchemaName, TableName;

CREATE TABLE #Results (SchemaName sysname, TableName sysname, Scope varchar(20),
                       SourceRows bigint, ExtractRows bigint,
                       SourceSum int, ExtractSum int, SourceLobSum int, ExtractLobSum int);

DECLARE @schema sysname, @table sysname, @hasTenant bit, @ord nvarchar(max), @lob nvarchar(max), @where nvarchar(100),
        @db sysname, @rows bigint, @sum int, @lobSum int, @isSource bit;

DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT SchemaName, TableName, HasTenant, OrdExpr, LobExpr FROM #Tables;
OPEN c; FETCH NEXT FROM c INTO @schema, @table, @hasTenant, @ord, @lob;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @where = CASE WHEN @hasTenant = 1 THEN N' WHERE TenantID = @KeepTenantID' ELSE N'' END;
    INSERT #Results (SchemaName, TableName, Scope)
    VALUES (@schema, @table, CASE WHEN @hasTenant = 1 THEN 'kept tenant rows' ELSE 'whole table' END);

    SET @isSource = 1;
    WHILE @isSource IS NOT NULL
    BEGIN
        SET @db = CASE WHEN @isSource = 1 THEN @SourceDatabase ELSE @ExtractDatabase END;
        SET @sql = N'SELECT @rows = COUNT_BIG(*), @sum = '
                 + CASE WHEN @ord IS NULL THEN N'0' ELSE N'CHECKSUM_AGG(BINARY_CHECKSUM(' + @ord + N'))' END
                 + N', @lobSum = '
                 + CASE WHEN @lob IS NULL THEN N'0' ELSE N'CHECKSUM_AGG(BINARY_CHECKSUM(' + @lob + N'))' END
                 + N' FROM ' + QUOTENAME(@db) + N'.' + QUOTENAME(@schema) + N'.' + QUOTENAME(@table) + @where + N';';
        BEGIN TRY
            EXEC sp_executesql @sql, N'@KeepTenantID int, @rows bigint OUTPUT, @sum int OUTPUT, @lobSum int OUTPUT',
                 @KeepTenantID, @rows OUTPUT, @sum OUTPUT, @lobSum OUTPUT;
        END TRY
        BEGIN CATCH
            SELECT @rows = -1, @sum = NULL, @lobSum = NULL;  -- e.g. table missing in the extract
            DECLARE @err nvarchar(2048) = ERROR_MESSAGE();
            RAISERROR('  %s.%s in %s: %s', 0, 1, @schema, @table, @db, @err) WITH NOWAIT;
        END CATCH

        IF @isSource = 1
            UPDATE #Results SET SourceRows = @rows, SourceSum = @sum, SourceLobSum = @lobSum
            WHERE SchemaName = @schema AND TableName = @table;
        ELSE
            UPDATE #Results SET ExtractRows = @rows, ExtractSum = @sum, ExtractLobSum = @lobSum
            WHERE SchemaName = @schema AND TableName = @table;

        SET @isSource = CASE WHEN @isSource = 1 THEN 0 END;
    END
    FETCH NEXT FROM c INTO @schema, @table, @hasTenant, @ord, @lob;
END
CLOSE c; DEALLOCATE c;

-- The deliberately blanked columns must be blank in the extract.
CREATE TABLE #BlankCheck (ColumnName nvarchar(400), RowsStillSet bigint);
DECLARE @col sysname;
DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT SchemaName, TableName, ColumnName FROM #Ignored;
OPEN c; FETCH NEXT FROM c INTO @schema, @table, @col;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = N'INSERT #BlankCheck SELECT N''' + @schema + N'.' + @table + N'.' + @col + N''', COUNT_BIG(*) FROM '
             + QUOTENAME(@ExtractDatabase) + N'.' + QUOTENAME(@schema) + N'.' + QUOTENAME(@table)
             + N' WHERE ISNULL(CAST(' + QUOTENAME(@col) + N' AS nvarchar(max)), N'''') <> N'''';';
    EXEC (@sql);
    FETCH NEXT FROM c INTO @schema, @table, @col;
END
CLOSE c; DEALLOCATE c;
SELECT ColumnName AS BlankedColumn,
       CASE WHEN RowsStillSet = 0 THEN 'blank' ELSE '*** STILL SET ***' END AS Status, RowsStillSet
FROM #BlankCheck;

-- Summary
SELECT COUNT(*) AS TablesCompared,
       SUM(CASE WHEN Scope = 'kept tenant rows' THEN SourceRows ELSE 0 END) AS KeptTenantRowsInSource,
       SUM(CASE WHEN Scope = 'kept tenant rows' THEN ExtractRows ELSE 0 END) AS KeptTenantRowsInExtract,
       SUM(CASE WHEN SourceRows = ExtractRows
                 AND ISNULL(SourceSum, 0) = ISNULL(ExtractSum, 0)
                 AND ISNULL(SourceLobSum, 0) = ISNULL(ExtractLobSum, 0) THEN 1 ELSE 0 END) AS TablesIdentical
FROM #Results;

-- Every table that differs, flagged expected/unexpected
SELECT CASE WHEN SchemaName = N'HangFire' OR (SchemaName = N'dbo' AND TableName = N'ReleaseNote')
            THEN 'expected' ELSE '*** UNEXPECTED ***' END AS Verdict,
       SchemaName + N'.' + TableName AS TableName, Scope,
       SourceRows, ExtractRows, SourceRows - ExtractRows AS RowsMissing,
       CASE WHEN ISNULL(SourceSum, 0) = ISNULL(ExtractSum, 0)
             AND ISNULL(SourceLobSum, 0) = ISNULL(ExtractLobSum, 0) THEN 'same' ELSE 'DIFFERENT' END AS Content
FROM #Results
WHERE NOT (SourceRows = ExtractRows
           AND ISNULL(SourceSum, 0) = ISNULL(ExtractSum, 0)
           AND ISNULL(SourceLobSum, 0) = ISNULL(ExtractLobSum, 0))
ORDER BY 1 DESC, 2;
GO
