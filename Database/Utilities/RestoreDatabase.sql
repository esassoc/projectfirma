-- this script is used by nant to restore the ${db-name} database

-- 1. put the ${db-name} database in single user mode
use master
if (@@error != 0) goto failed

-- 2. restore the database from the backup file
alter database ${db-name} set single_user with rollback immediate
restore database ${db-name}
    from disk = '@DUMPFILE@'
    with
		move '${db-name}' to '@DATADEVICE@'
	,   move '${db-name}_Log'  to '@LOGDEVICE@'
	,   replace
if (@@error != 0) goto failed

-- 3. Correct account issues
use ${db-name}
if (@@error != 0) goto failed

exec sp_changedbowner 'sa'
if (@@error != 0) goto failed

-- 4. Retrieve the last backup information, and stash it in the restored database for later runtime reference

DECLARE @LastBackupKeyValues TABLE
	(
		 BackupName nvarchar(128)
		,BackupDescription nvarchar(255)
		,BackupType smallint
		,ExpirationDate datetime
		,Compressed tinyint
		,Position smallint
		,DeviceType tinyint
		,UserName nvarchar(128)
		,ServerName nvarchar(128)
		,DatabaseName nvarchar(128)
		,DatabaseVersion int
		,DatabaseCreationDate datetime
		,BackupSize numeric(20,0)
		,FirstLSN numeric(25,0)
		,LastLSN numeric(25,0)
		,CheckpointLSN numeric(25,0)
		,DatabaseBackupLSN numeric(25,0)
		,BackupStartDate datetime
		,BackupFinishDate datetime
		,SortOrder smallint
		,CodePage smallint
		,UnicodeLocaleId int
		,UnicodeComparisonStyle int
		,CompatibilityLevel tinyint
		,SoftwareVendorId int
		,SoftwareVersionMajor int
		,SoftwareVersionMinor int
		,SoftwareVersionBuild int
		,MachineName nvarchar(128)
		,Flags int
		,BindingID uniqueidentifier
		,RecoveryForkID uniqueidentifier
		 --following columns introduced in SQL 2008
		,Collation nvarchar(128)
		,FamilyGUID uniqueidentifier
		,HasBulkLoggedData bit
		,IsSnapshot bit
		,IsReadOnly bit
		,IsSingleUser bit
		,HasBackupChecksums bit
		,IsDamaged bit
		,BeginsLogChain bit
		,HasIncompleteMetaData bit
		,IsForceOffline bit
		,IsCopyOnly bit		,FirstRecoveryForkID uniqueidentifier		,ForkPointLSN numeric(25,0)		,RecoveryModel nvarchar(60)		,DifferentialBaseLSN numeric(25,0)		,DifferentialBaseGUID uniqueidentifier		,BackupTypeDescription nvarchar(60)		,BackupSetGUID uniqueidentifier NULL 		,CompressedBackupSize bigint		--following columns introduced in SQL 2012		,Containment tinyint 		--following columns introduced in SQL 2014		,KeyAlgorithm nvarchar(32)		,EncryptorThumbprint varbinary(20)		,EncryptorType nvarchar(32)
		--following columns introduced in SQL 2016
		,LastValidRestoreTime datetime
		,TimeZone smallint
		,CompressionAlgorithm nvarchar(32)
	);INSERT INTO @LastBackupKeyValues EXEC ('RESTORE HEADERONLY FROM DISK=''@DUMPFILE@''');-- Record this database restore data, only one row in the table so clear out old entries firstuse ${db-name}delete from dbo.LastSQLServerDatabaseBackupinsert into dbo.LastSQLServerDatabaseBackup
select    lbkv.BackupName,    lbkv.UserName,    lbkv.ServerName,    lbkv.DatabaseName,    lbkv.DatabaseVersion,    lbkv.DatabaseCreationDate,    lbkv.BackupSize,    lbkv.BackupStartDate,    lbkv.BackupFinishDate,    lbkv.MachineName,    lbkv.Collation,    lbkv.CompressedBackupSize 
    from @LastBackupKeyValues as lbkv

-- 5. put the ${db-name} database back into multi user mode
alter database ${db-name} set multi_user
if (@@error != 0) goto failed

goto goodbye

failed:

raiserror('The database restore script failed. Additional error details should be available above.', 16, 1)

goodbye:

use master


