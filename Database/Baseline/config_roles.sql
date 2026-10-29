use ${db-name}

print 'Getting logins to create'
if object_id('tempdb.dbo.#accountsToCreate') is not null drop table #accountsToCreate
select '${db-user}' as AccountName, 'U' as AccountType
into #accountsToCreate
union select '${db-batch-user}', 'U'
union select '${db-geoserver-user}', 'U'
union select 'Sitka\Rocket QA Support', 'G'
union select 'Sitka\Rocket QA Tester', 'G'
union select 'Sitka\Hawk Moth QA Support', 'G'
union select 'Sitka\Hawk Moth QA Tester', 'G'

-- Clear out any existing logins
print 'Clearing out any existing logins'

if object_id('tempdb.dbo.#allAccounts') is not null drop table #allAccounts
select a.AccountName, a.AccountType
into #allAccounts
from
(
    select '${local-db-user}' as AccountName, 'U' as AccountType
    union select '${qa-db-user}', 'U'
    union select '${prod-db-user}', 'U'
    union select '${local-db-batch-user}', 'U'
    union select '${qa-db-batch-user}', 'U'
    union select '${prod-db-batch-user}', 'U'
    union select '${local-db-geoserver-user}', 'U'
    union select '${qa-db-geoserver-user}', 'U'
    union select '${prod-db-geoserver-user}', 'U'
    union select '${local-db-geoserver-docker-user}', 'S'
    union select '${qa-db-geoserver-docker-user}', 'S'
    union select '${prod-db-geoserver-docker-user}', 'S'
    union select 'Sitka\Rocket QA Support', 'G'
    union select 'Sitka\Rocket QA Tester', 'G'
    union select 'Sitka\Hawk Moth QA Support', 'G'
    union select 'Sitka\Hawk Moth QA Tester', 'G'
) a left join #accountsToCreate ac on a.AccountName = ac.AccountName
where ac.AccountName is null


declare @accountName varchar(200)
declare @accountType varchar(2)
declare @sql nvarchar(1000)
declare @roleName sysname

while exists(select 1 from #allAccounts)
begin
    select top 1 @accountName = AccountName, @accountType = AccountType from #allAccounts

    print 'Login: ' + @accountName
    
    if exists (select 1 from sys.server_principals where name = @accountName and type = @accountType)
    begin
        set @sql = replace('drop login [ACCOUNT]', 'ACCOUNT',  @accountName)
        print @sql
        exec sp_executesql @sql
    end
    if exists (select 1 from sys.sysusers where name = @accountName)
    begin
        set @sql = replace('drop user [ACCOUNT]', 'ACCOUNT',  @accountName)
        print @sql
        exec sp_executesql @sql
    end
    delete from #allAccounts where AccountName = @accountName
end



-- now add the user for this environment specified by ${db-user} token
print 'Creating logins for this environment'


while exists(select 1 from #accountsToCreate)
begin
    select top 1 @accountName = AccountName, @accountType = AccountType from #accountsToCreate

    print 'Login: ' + @accountName

    if not exists (select 1 from sys.server_principals where name = @accountName and type = @accountType)
    begin
        set @sql = replace('create login [ACCOUNT] from windows', 'ACCOUNT',  @accountName)
        print @sql
        exec sp_executesql @sql
    end

    if not exists (select 1 from sys.sysusers where name = @accountName)
    begin
        set @sql = replace('create user [ACCOUNT] for login [ACCOUNT]', 'ACCOUNT',  @accountName)
        print @sql
        exec sp_executesql @sql    
    end
    
    -- GeoServer's Windows account only reads the vGeoServer* views
    set @roleName = case when @accountName = '${db-geoserver-user}' then 'db_datareader' else 'db_owner' end
    exec sp_addrolemember @roleName, @accountName
    print 'added user [' + @accountName + '] to ' + @roleName + ' role'
    if @roleName = 'db_datareader' and is_rolemember('db_owner', @accountName) = 1
    begin
        exec sp_droprolemember 'db_owner', @accountName
        print 'removed user [' + @accountName + '] from db_owner role'
    end

    delete from #accountsToCreate where AccountName = @accountName
end