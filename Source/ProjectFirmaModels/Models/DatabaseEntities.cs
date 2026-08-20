/*-----------------------------------------------------------------------
<copyright file="DatabaseEntities.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
Copyright (c) Tahoe Regional Planning Agency and Environmental Science Associates. All rights reserved.
<author>Environmental Science Associates</author>
</copyright>

<license>
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

Source code is available upon request via <support@sitkatech.com>.
</license>
-----------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Transactions;
using LtInfo.Common.Mvc;

namespace ProjectFirmaModels.Models
{
    public partial class DatabaseEntities : ISitkaDbContext
    {
        public Person Person { get; set; }

        public int SaveChanges(FirmaSession currentFirmaSession)
        {
            return SaveChanges(currentFirmaSession.Person);
        }

        public int SaveChanges(Person userPerson)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.Snapshot }))
            {
                return SaveChangesImpl(userPerson, userPerson.Tenant, scope);
            }
        }

        public int SaveChanges(Person userPerson, IsolationLevel isolationLevel)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = isolationLevel }))
            {
                return SaveChangesImpl(userPerson, userPerson.Tenant, scope);
            }
        }

        public override int SaveChanges()
        {
            return SaveChanges(Person);
        }

        public int SaveChanges(IsolationLevel isolationLevel)
        {
            return SaveChanges(Person, isolationLevel);
        }

        public int SaveChangesWithNoAuditing(int tenantId)
        {
            ChangeTracker.DetectChanges();
            var dbEntityEntries = ChangeTracker.Entries().ToList();
            SetTenantIDForAllModifiedEntries(dbEntityEntries, tenantId);
            // PF-2838: capture the changed ProjectLocations before saving. ProjectID on a newly added
            // ProjectLocation is not populated until the save itself, so this has to be read afterwards.
            var changedProjectLocations = GetChangedProjectLocations(dbEntityEntries);
            var changes = base.SaveChanges();
            RecomputeGisAcres(changedProjectLocations);
            return changes;
        }

        private int SaveChangesImpl(Person person, Tenant tenant, TransactionScope scope)
        {
            ChangeTracker.DetectChanges();

            var dbEntityEntries = ChangeTracker.Entries().ToList();
            var addedEntries = dbEntityEntries.Where(e => e.State == EntityState.Added).ToList();
            var modifiedEntries = dbEntityEntries
                .Where(e => e.State == EntityState.Deleted || e.State == EntityState.Modified).ToList();
            
            var tenantID = tenant.TenantID;

            SetTenantIDForAllModifiedEntries(dbEntityEntries, tenantID);

            // PF-2838: see GetChangedProjectLocations. Captured here, applied after the save below.
            var changedProjectLocations = GetChangedProjectLocations(dbEntityEntries);

            // Project is such an important piece to PF; if we generate an audit log record that has a ProjectID, we need to update the last update date on the Project
            var distinctProjectIDsModified = new HashSet<int>();

            foreach (var entry in modifiedEntries)
            {
                // For each changed record, get the audit record entries and add them
                var auditRecordsForChange =
                    AuditLog.GetAuditLogRecordsForModifiedOrDeleted(entry, person, this, tenantID);
                AllAuditLogs.AddRange(auditRecordsForChange);
                ExtractProjectIDsFromAuditLogs(auditRecordsForChange).ForEach(x => distinctProjectIDsModified.Add(x));
            }

            int changes;
            try
            {
                changes = base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }

            foreach (var entry in addedEntries)
            {
                // For each added record, get the audit record entries and add them
                var auditRecordsForChange = AuditLog.GetAuditLogRecordsForAdded(entry, person, this, tenantID);
                AllAuditLogs.AddRange(auditRecordsForChange);
                ExtractProjectIDsFromAuditLogs(auditRecordsForChange).ForEach(x => distinctProjectIDsModified.Add(x));
            }

            // now update LastUpdatedDate of any Projects that were touched
            if (distinctProjectIDsModified.Any())
            {
                var listForLinqToSqlTranslation = distinctProjectIDsModified.ToList();
                List<Project> projects = Projects.Where(x => listForLinqToSqlTranslation.Contains(x.ProjectID)).ToList();
                foreach (var project in projects)
                {
                    project.LastUpdatedDate = DateTime.Now;
                }
                ChangeTracker.DetectChanges();
            }
            // we need to save the audit log entries now
            base.SaveChanges();

            RecomputeGisAcres(changedProjectLocations);

            scope.Complete();
            return changes;
        }

        private static List<int> ExtractProjectIDsFromAuditLogs(IEnumerable<AuditLog> auditRecordsForChange)
        {
            var auditLogsWithProjectID = auditRecordsForChange.Where(x => x.ProjectID.HasValue).ToList();
            return auditLogsWithProjectID.Any() ? auditLogsWithProjectID.Select(x => x.ProjectID.Value).Distinct().ToList() : new List<int>();
        }

        /// <summary>
        /// PF-2838: every ProjectLocation write path in the app is delete-all-then-reinsert, and
        /// DbSpatialHelper.Reduce rewrites geometry on already-inserted rows after the fact, so the
        /// ChangeTracker is the only reliable place to notice that a project's geometry changed.
        /// Must be called before base.SaveChanges(); the ProjectIDs are read afterwards.
        /// </summary>
        private static List<ProjectLocation> GetChangedProjectLocations(IEnumerable<DbEntityEntry> dbEntityEntries)
        {
            return dbEntityEntries
                .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified ||
                             x.State == EntityState.Deleted) && x.Entity is ProjectLocation)
                .Select(x => (ProjectLocation) x.Entity)
                .ToList();
        }

        /// <summary>
        /// PF-2838: Project.GisAcres is a stored, geometry-derived acreage exposed on the GeoServer
        /// WFS/WMS project layers. Computing it on the fly in dbo.vGeoServerProjectAttributes measured
        /// ~3.5s per tenant-filtered WFS query, so it is maintained here on write instead.
        /// Call this AFTER base.SaveChanges(), so inserted and updated geometry is visible to the
        /// function, and while any surrounding TransactionScope is still open.
        /// Raw SQL is deliberate: this is a derived value, so it should neither generate AuditLog rows
        /// nor bump Project.LastUpdatedDate.
        /// </summary>
        private void RecomputeGisAcres(List<ProjectLocation> changedProjectLocations)
        {
            if (!changedProjectLocations.Any())
            {
                return;
            }

            // A ProjectLocation added via the Project navigation property has ProjectID == 0 until the
            // save populates it; fall back to the navigation property for anything still unset.
            var projectIDs = changedProjectLocations
                .Select(x => x.ProjectID != 0 ? x.ProjectID : x.Project?.ProjectID ?? 0)
                .Where(x => x != 0)
                .Distinct()
                .ToList();

            if (!projectIDs.Any())
            {
                return;
            }

            // Safe to inline: these are ints. A project deleted in this same save simply matches no row.
            var projectIDList = string.Join(",", projectIDs);
            Database.ExecuteSqlCommand(
                $"update dbo.Project set GisAcres = dbo.fProjectGisAcres(ProjectID) where ProjectID in ({projectIDList})");
        }

        private static void SetTenantIDForAllModifiedEntries(List<DbEntityEntry> dbEntityEntries, int tenantID)
        {
            /*
             * This is where we are setting it to the TenantID of the current thread or HttpRequestStorage.Tenant;
             */
            foreach (var entry in dbEntityEntries.Where(entry =>
                (entry.State == EntityState.Added || entry.State == EntityState.Deleted ||
                 entry.State == EntityState.Modified) && entry.Entity is IHaveATenantID))
            {
                if (entry.Entity is IHaveATenantID haveATenantID && haveATenantID.TenantID <= 0)
                {
                    haveATenantID.TenantID = tenantID;
                }
            }
        }

        public DbContext GetDbContext()
        {
            return this;
        }

        public ObjectContext GetObjectContext()
        {
            return ((IObjectContextAdapter) this).ObjectContext;
        }

    }
}
