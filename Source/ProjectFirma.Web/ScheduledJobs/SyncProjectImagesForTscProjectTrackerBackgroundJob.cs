using ProjectFirma.Web.Common;
using ProjectFirmaModels;
using ProjectFirmaModels.Models;
using ProjectFirmaModels.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LtInfo.Common.Models;

namespace ProjectFirma.Web.ScheduledJobs
{
    public class SyncProjectImagesForTscProjectTrackerBackgroundJob : ScheduledBackgroundJobBase
    {
        public const string ScheduledBackgroundJobName = "Sync Project Images For TCS Project Tracker";

        public SyncProjectImagesForTscProjectTrackerBackgroundJob() : base(ScheduledBackgroundJobName)
        {
        }

        public SyncProjectImagesForTscProjectTrackerBackgroundJob(string jobName) : base(ScheduledBackgroundJobName)
        {
        }

        public override List<FirmaEnvironmentType> RunEnvironments => new List<FirmaEnvironmentType>
        {
            FirmaEnvironmentType.Local,
            FirmaEnvironmentType.Prod,
            FirmaEnvironmentType.Qa
        };

        protected override void RunJobImplementation()
        {
            throw new NotImplementedException();
        }

        protected override bool IsAsyncJob()
        { 
            return true;
        }

        protected override async Task RunJobImplementationAsync() {
            var tenantID = Tenant.TCSProjectTracker.TenantID;
            var tenantAttribute = DbContext.AllTenantAttributes.Single(x => x.TenantID == tenantID);
            if (tenantAttribute.ProjectExternalDataSourceEnabled)
            {
                var databaseEntities = new DatabaseEntities(tenantID);
                var client = new HttpClient();

                var projects = databaseEntities.AllProjects.Where(x => x.TenantID == tenantID && x.ExternalID.HasValue).ToList();
                var externalIDs = projects.Select(x => x.ExternalID.Value).ToList();
                var apiUrl = tenantAttribute.ProjectExternalDataSourceApiUrl;

                StringBuilder builder = new StringBuilder();
                bool first = true;
                foreach (int externalID in externalIDs)
                {
                    if (!first)
                        builder.Append("&");
                    builder.Append("projectID=");
                    builder.Append(externalID);
                    
                    first = false;
                }
                var queryParameter = builder.ToString();

                var recentlyModifiedExternalIDs = new List<int>();

                // apiUrl is the LtInfo API origin only (e.g. https://host), no path. Projects live
                // under /api/projects; file resources live at root under /file-resources.
                var getRecentlyModifiedProjectsUrl = $"{apiUrl}/api/projects/recently-modified?apiKey={FirmaWebConfiguration.LTInfoApiKey}&{queryParameter}";
                var response = await client.GetAsync(getRecentlyModifiedProjectsUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Warn($"GET {getRecentlyModifiedProjectsUrl} failed, reason: {response.ReasonPhrase}");
                }
                else
                {
                    var projectDtos = await response.Content.ReadAsAsync<List<ProjectSimpleDto>>();

                    foreach (var projectDto in projectDtos)
                    {
                        recentlyModifiedExternalIDs.Add(projectDto.ProjectID);
                    }
                }

                Logger.Info("Starting Project Images sync.");
                foreach (var externalID in recentlyModifiedExternalIDs)
                {
                    var project = projects.Single(x => x.ExternalID == externalID);
                    Logger.Info($"Starting Project Images sync for ProjectID: {project?.ProjectID}; ExternalID: {externalID}");

                    var getProjectImagesUrl = $"{apiUrl}/api/projects/{externalID}/project-images?apiKey={FirmaWebConfiguration.LTInfoApiKey}";
                    var getProjectImages = await client.GetAsync(getProjectImagesUrl);
                    if (!getProjectImages.IsSuccessStatusCode)
                    {
                        Logger.Warn($"GET {getProjectImagesUrl} failed, reason: {getProjectImages.ReasonPhrase}");
                    }
                    else
                    {
                        var projectImageDtos = await getProjectImages.Content.ReadAsAsync<List<ProjectImageSimpleDto>>();
                        try
                        {
                            await UpdateProjectImages(project, projectImageDtos, tenantID, databaseEntities, client, apiUrl);
                            databaseEntities.SaveChangesWithNoAuditing(tenantID);
                            Logger.Info($"Project Images sync complete for ProjectID: {project.ProjectID}; ExternalID: {externalID}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Info($"Project Images sync failed for ProjectID: {project.ProjectID}; ExternalID: {externalID}");
                            Logger.Warn(ex);
                        }
                    }
                }
                Logger.Info("Project Images sync complete.");
                
            }
        }

        
        private async Task UpdateProjectImages(Project project, List<ProjectImageSimpleDto> projectImageSimpleDtos, int tenantID, DatabaseEntities databaseEntities, HttpClient client, string apiUrl)
        {
            var projectImagesUpdated = new List<ProjectImage>();
            databaseEntities.ProjectImages.Load();
            var allProjectImages = databaseEntities.AllProjectImages.Local;
            databaseEntities.FileResourceInfos.Load();
            var allFileResourceInfos = databaseEntities.AllFileResourceInfos.Local;
            databaseEntities.FileResourceDatas.Load();
            var allFileResourceDatas = databaseEntities.AllFileResourceDatas.Local;

            foreach (var projectImageSimpleDto in projectImageSimpleDtos)
            {
                var mimeType = FileResourceMimeType.All.SingleOrDefault(x => x.FileResourceMimeTypeName == projectImageSimpleDto.FileResourceInfo.FileResourceMimeType.FileResourceMimeTypeName);
                if (mimeType == null)
                {
                    Logger.Warn($"\tProjectID: {project.ProjectID}; ExternalID: {projectImageSimpleDto.ProjectID}. No Mime Type found for '{projectImageSimpleDto.FileResourceInfo.FileResourceMimeType.FileResourceMimeTypeName}'");
                    continue;
                }
                var projectImageTiming = ProjectImageTiming.All.SingleOrDefault(x => x.ProjectImageTimingName == projectImageSimpleDto.ProjectImageTiming.ProjectImageTimingName);
                if (projectImageTiming == null)
                {
                    Logger.Warn($"\tProjectID: {project.ProjectID}; ExternalID: {projectImageSimpleDto.ProjectID}. No Project Image Timing found for '{projectImageSimpleDto.ProjectImageTiming.ProjectImageTimingName}'");
                    continue;
                }
                // Match on email: both systems moved off the shared Keystone PersonGuid to Auth0.
                // Falls back to the first Admin / ESA Admin when the author can't be matched.
                var createPerson = (!string.IsNullOrWhiteSpace(projectImageSimpleDto.FileResourceInfo.CreatePersonEmail)
                                       ? databaseEntities.AllPeople.FirstOrDefault(x => x.TenantID == tenantID && x.Email == projectImageSimpleDto.FileResourceInfo.CreatePersonEmail)
                                       : null) ??
                                    databaseEntities.AllPeople.Where(x => x.TenantID == tenantID && x.RoleID == Role.Admin.RoleID || x.RoleID == Role.ESAAdmin.RoleID).OrderBy(x => x.RoleID).ThenBy(x => x.PersonID).FirstOrDefault();
                if (createPerson == null)
                {
                    Logger.Warn($"\tProjectID: {project.ProjectID}; ExternalID: {projectImageSimpleDto.ProjectID}. No Create Person found for '{projectImageSimpleDto.FileResourceInfo.CreatePersonGUID}' and the system could not default the Create Person to a current Admin or ESA Admin");
                    continue;
                }

                // get file resource from lt info api
                // File resources are served at the API root under /file-resources (NOT under /api).
                var getFileResource = $"{apiUrl}/file-resources/GetWithApiKey/{projectImageSimpleDto.FileResourceInfo.FileResourceInfoGUID}?apiKey={FirmaWebConfiguration.LTInfoApiKey}";

                var response = await client.GetAsync(getFileResource);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Warn($"\tProjectID: {project.ProjectID}; ExternalID: {projectImageSimpleDto.ProjectID}. GET {getFileResource} failed, reason: {response.ReasonPhrase}");
                    continue;
                }

                var dataBytes = await response.Content.ReadAsByteArrayAsync();

                var fileResourceInfo = allFileResourceInfos.SingleOrDefault(x =>
                                           x.TenantID == tenantID && x.FileResourceGUID ==
                                           projectImageSimpleDto.FileResourceInfo.FileResourceInfoGUID)
                                       ?? new FileResourceInfo(mimeType.FileResourceMimeTypeID,
                                           projectImageSimpleDto.FileResourceInfo.OriginalBaseFilename,
                                           projectImageSimpleDto.FileResourceInfo.OriginalFileExtension,
                                           projectImageSimpleDto.FileResourceInfo.FileResourceInfoGUID,
                                           createPerson.PersonID,
                                           projectImageSimpleDto.FileResourceInfo.CreateDate);
                if (!ModelObjectHelpers.IsRealPrimaryKeyValue(fileResourceInfo.FileResourceInfoID))
                {
                    allFileResourceInfos.Add(fileResourceInfo);
                    var fileResourceDatum = new FileResourceData(fileResourceInfo.FileResourceInfoID,
                        dataBytes);
                    allFileResourceDatas.Add(fileResourceDatum);
                }
                var projectImage = new ProjectImage(fileResourceInfo.FileResourceInfoID, project.ProjectID, projectImageTiming.ProjectImageTimingID, projectImageSimpleDto.Caption, projectImageSimpleDto.Credit, projectImageSimpleDto.IsKeyPhoto, !projectImageSimpleDto.ExcludeFromFactSheet)
                {
                    TenantID = tenantID
                };
                projectImagesUpdated.Add(projectImage);
            }

            project.ProjectImages.Merge(projectImagesUpdated,
                allProjectImages,
                (x, y) => x.TenantID == y.TenantID && x.ProjectID == y.ProjectID && x.FileResourceInfoID == y.FileResourceInfoID,
                (x, y) =>
                {
                    x.ProjectImageTimingID = y.ProjectImageTimingID;
                    x.Caption = y.Caption;
                    x.Credit = y.Credit;
                    x.IsKeyPhoto = y.IsKeyPhoto;
                    x.IncludeInFactSheet = y.IncludeInFactSheet;
                },
                databaseEntities);
        }

    }
}
