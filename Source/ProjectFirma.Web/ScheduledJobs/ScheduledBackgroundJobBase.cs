using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using ProjectFirma.Web.Common;
using ProjectFirmaModels.Models;

namespace ProjectFirma.Web.ScheduledJobs
{
    public abstract class ScheduledBackgroundJobBase
    {
        /// <summary>
        /// A safety guard to ensure only one job is running at a time, some jobs seem like they would collide if allowed to run concurrently or possibly drag the server down.
        /// </summary>
        public static readonly object ScheduledBackgroundJobLock = new object();

        public readonly string JobName;
        protected ILog Logger { get; }
        protected DatabaseEntities DbContext;

       

        /// <summary> 
        /// Jobs must have a proscribed environment to run in (for example, to prevent a job that makes a lot of calls to an external API from accidentally DOSing that API by running on all local boxes, QA, and Prod at the same time.
        /// </summary>
        public abstract List<FirmaEnvironmentType> RunEnvironments { get; }

        protected ScheduledBackgroundJobBase()
        {
            Logger = LogManager.GetLogger(GetType());
            var databaseEntities = new DatabaseEntities(Tenant.SitkaTechnologyGroup.TenantID); // default to Sitka
            databaseEntities.Configuration.AutoDetectChangesEnabled = false;
            DbContext = databaseEntities;
        }

        protected ScheduledBackgroundJobBase(string jobName) : this()
        {
            JobName = jobName;
        }

        /// <summary>
        /// This wraps the call to <see cref="RunJobImplementation"/> with all of the housekeeping for being a scheduled job.
        /// </summary>
        public void RunJob()
        {
            lock (ScheduledBackgroundJobLock)
            {
                // No-Op if we're not running in an allowed environment
                if (!RunEnvironments.Contains(FirmaWebConfiguration.FirmaEnvironment.FirmaEnvironmentType))
                {
                    return;
                }

                try
                {
                    Logger.Info($"Begin Firma Job {JobName}");
                    if (IsAsyncJob())
                    {
                        var task = RunJobImplementationAsync();
                        task.Wait();
                    }
                    else
                    {
                        RunJobImplementation();
                    }

                    Logger.Info($"End Firma Job {JobName}");
                }
                catch (Exception ex)
                {
                    // Wrap and rethrow with the information about which job encountered the problem
                    // Log once with the full exception. Hangfire's own failure logging is kept out of App Alerts by a filter on the SmtpAppender
                    Logger.Error($"Firma Job {JobName} failed", ex);
                    throw new ScheduledBackgroundJobException(JobName, ex);
                }
            }
        }

        /// <summary>
        /// Jobs can fill this in with whatever they need to run. This is called by <see cref="RunJob"/> which handles other miscellaneous stuff
        /// </summary>
        protected abstract void RunJobImplementation();

        /// <summary>
        /// Jobs can fill this in with whatever they need to run. This is called by <see cref="RunJob"/> which handles other miscellaneous stuff
        /// </summary>
        protected abstract Task RunJobImplementationAsync();

        protected abstract bool IsAsyncJob();
    }
}