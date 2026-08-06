using GeoJSON.Net.Feature;
using LtInfo.Common;
using LtInfo.Common.DbSpatial;
using LtInfo.Common.GeoJson;
using LtInfo.Common.Views;
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

namespace ProjectFirma.Web.ScheduledJobs
{
    public class SyncProjectsForTscProjectTrackerBackgroundJob : ScheduledBackgroundJobBase
    {
        public const string ScheduledBackgroundJobName = "Sync Projects For TCS Project Tracker";

        public SyncProjectsForTscProjectTrackerBackgroundJob() : base(ScheduledBackgroundJobName)
        {
        }

        public SyncProjectsForTscProjectTrackerBackgroundJob(string jobName) : base(ScheduledBackgroundJobName)
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
        
        private  Dictionary<string, string> EIPPerformaceMeasureToTCSPerformanceMeasure = new Dictionary<string, string>()  {
            {"Acres of Forest Fuels Reduction Treatment", "Acres of Forest Fuels Reduction Treatment" },
            {"Acres of Habitat Restored or Enhanced", "Acres of Habitat Restored/Created"},
            {"Acres of SEZ Restored or Enhanced", "Acres of SEZ Restored or Enhanced"},
            {"Acres Treated for Invasive Species", "Acres Treated for Invasive Species"},
            {"Educational and Interpretive Programs Produced", "Educational and Interpretive Programs Produced"}, // TODO does not exist in TCS
            {"Facilities Improved or Created", "Facilities Improved & Created"},
            {"Length of Public Shoreline Added", "Length of Public Shoreline Added"}, // TODO does not exist in TCS
            {"Miles of Pedestrian and Bicycle Routes Improved or Constructed", "Miles of Pedestrian and Bicycle Routes Improved or Constructed"}, // TODO does not exist in TCS
            {"Miles of Roads Decommissioned or Retrofitted", "Miles of Road Treated"},
            {"Miles of Trails Developed or Improved", "Miles of Trails Developed or Improved"}, // TODO does not exist in TCS
            {"Pounds of Air Pollutants Removed or Avoided by Project", "Pounds of Air Pollutants Removed or Avoided by Project"},
            {"Fine Sediment Load Reduction", "Sediment Load Reduction"},
            {"Special Status Species Sites Protected or Re-Established", "Special Status Species Sites Protected or Re-established"},
            {"Tons of Biomass Utilized", "Tons of Biomass Utilized"},
            {"Tons of Greenhouse Gases Reduced", "Tons of Greenhouse Gases Reduced"},
            {"Watercraft Inspections for Invasive Species", "Watercraft Inspections for Invasive Species"},
            {"Acres of Environmentally Sensitive Land Acquired", "Acres of Environmentally Sensitive Land Acquired"}
        };

        private Dictionary<string, string> EIPActionPriorityNumberToTCSStrategy = new Dictionary<string, string>()  {
            {"01.01.01", "Stormwater Management"},
            {"01.01.02", "Stormwater Management"},
            {"01.02.01", "Hydrologic Process & Geomorphic Feature Restoration"},
            {"01.02.02", "Acquire Lands to Protect/Conserve Natural Resources"},
            {"01.02.03", "Mechanical and Manual Removal"},
            {"01.03.01", "Mechanical and Manual Removal"},
            {"02.01.01", "Wildland Urban Interface Hazardous Fuel Reduction"},
            {"02.01.02", "Defensible Space and Home Hardening"},
            {"02.01.03", "Firefighting Infrastructure Enhancement"},
            {"02.02.01", "Aspen Regeneration"},
            {"02.02.02", "Fuels Reduction"},
            {"02.02.03", "Restore/Reconnect Habitat"},
            {"03.01.01", "Expand Public Access to Natural Areas"},
            {"03.01.02", "Upgrade and Maintain Recreational Facilities"},
            {"03.02.01", "Develop, Improve, and Sustain Transit Systems "},
            {"03.02.02", "Create, Enhance, and Maintain Trail Networks "},
            {"03.02.03", "Develop, Improve, and Sustain Transit Systems "},
            {"03.02.04", "Create, Enhance, and Maintain Trail Networks "},
            {"04.01.01", "Sustainable Resource Management"},
            {"04.01.02", "Remote-Sensing and GIS Based Research"}
        };



        protected override async Task RunJobImplementationAsync()
        {
            var tenantID = Tenant.TCSProjectTracker.TenantID;
            var tenantAttribute = DbContext.AllTenantAttributes.Single(x => x.TenantID == tenantID);
            if (tenantAttribute.ProjectExternalDataSourceEnabled)
            {
                var databaseEntities = new DatabaseEntities(tenantID);
                var client = new HttpClient();
                // The Projects API now lives on LtInfo.Scalar and authenticates via a shared secret in
                // the X-LtInfo-Integration-Key header (no query-string key). Every call this job makes goes
                // to Scalar, so set the header once on the client.
                client.DefaultRequestHeaders.Add("X-LtInfo-Integration-Key", FirmaWebConfiguration.LTInfoApiKey);

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

                // apiUrl is the LtInfo.Scalar origin only (e.g. https://host), no path. The Projects
                // API lives under /api/projects; auth is via the X-LtInfo-Integration-Key header set above.
                var getAllProjectsUrl = $"{apiUrl}/api/projects/all-projects?{queryParameter}";
                var response = await client.GetAsync(getAllProjectsUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Warn($"GET {getAllProjectsUrl} failed, reason: {response.ReasonPhrase}");
                }
                else
                {
                    var projectDtos = await response.Content.ReadAsAsync<List<ProjectSimpleDto>>();

                    foreach (var projectDto in projectDtos)
                    {
                        var project = projects.Single(x => x.ExternalID == projectDto.ProjectID);
                        recentlyModifiedExternalIDs.Add(projectDto.ProjectID);
                        
                        try
                        {
                            //Logger.Info($"Starting sync for ProjectID: {project?.ProjectID}; ExternalID: {projectDto.ProjectID}");

                            UpdateProjectFromExternalDataSourceProjectSimpleDto(project, projectDto, tenantID, databaseEntities);
                            databaseEntities.SaveChangesWithNoAuditing(tenantID);

                            //Logger.Info($"Project sync complete for ProjectID: {project.ProjectID}; ExternalID: {projectDto.ProjectID}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Info($"Project sync failed for ProjectID: {project.ProjectID}; ExternalID: {projectDto.ProjectID}");
                            Logger.Warn(ex);
                        }
                        
                    }
                    Logger.Info("Projects sync complete.");
                }

                // try to re-sync (or sync for the first time) any projects that are missing a primary contact
                var projectsMissingPrimaryContact = projects.Where(x => string.IsNullOrWhiteSpace(x.PrimaryContactPersonFullName)).ToList();
                if (projectsMissingPrimaryContact.Any())
                {
                    Logger.Info("Starting newly added Projects sync.");
                }
                foreach (var project in projectsMissingPrimaryContact)
                {
                    var getProjectUrl = $"{apiUrl}/api/projects/{project.ExternalID}";
                    var projectResponse = await client.GetAsync(getProjectUrl);
                    if (!projectResponse.IsSuccessStatusCode)
                    {
                        Logger.Warn($"GET {getProjectUrl} failed, reason: {projectResponse.ReasonPhrase}");
                    }
                    else
                    {
                        var projectDto = await projectResponse.Content.ReadAsAsync<ProjectSimpleDto>();
                        recentlyModifiedExternalIDs.Add(projectDto.ProjectID);
                        try
                        {
                            UpdateProjectFromExternalDataSourceProjectSimpleDto(project, projectDto, tenantID, databaseEntities);
                            databaseEntities.SaveChangesWithNoAuditing(tenantID);
                        }
                        catch (Exception ex)
                        {
                            Logger.Info($"Project sync failed for ProjectID: {project.ProjectID}; ExternalID: {projectDto.ProjectID}");
                            Logger.Warn(ex);
                        }
                    }
                }
                if (projectsMissingPrimaryContact.Any())
                {
                    Logger.Info("Newly added Projects sync complete.");
                }
            }
        }

        /// <summary>
        /// Updates the project with the data from the external source
        /// </summary>
        /// <param name="project"></param>
        /// <param name="projectSimpleDto"></param>
        private void UpdateProjectFromExternalDataSourceProjectSimpleDto(Project project, ProjectSimpleDto projectSimpleDto, int tenantID, DatabaseEntities databaseEntities)
        {
            project.ProjectStageID = ProjectStage.All.SingleOrDefault(x => x.ProjectStageName == projectSimpleDto.ProjectStage.ProjectStageName)?.ProjectStageID ?? project.ProjectStageID;
            project.ProjectName = projectSimpleDto.ProjectName;
            project.ProjectDescription = projectSimpleDto.ProjectDescription;
            project.ImplementationStartYear = projectSimpleDto.ImplementationStartYear;
            project.CompletionYear = projectSimpleDto.CompletionYear;
            project.PerformanceMeasureActualYearsExemptionExplanation = projectSimpleDto.ProjectIndicatorReportedValueYearsExemptionExplanation;
            project.ProjectLocationNotes = projectSimpleDto.ProjectLocationNotes;
            project.PlanningDesignStartYear = projectSimpleDto.PlanningDesignStartYear;
            project.ProjectLocationSimpleTypeID = ProjectLocationSimpleType.All.SingleOrDefault(x => x.ProjectLocationSimpleTypeName == projectSimpleDto.ProjectLocationSimpleType.ProjectLocationSimpleTypeName)?.ProjectLocationSimpleTypeID ?? project.ProjectLocationSimpleTypeID;
            if (projectSimpleDto.ProjectLocationSimpleType.ProjectLocationSimpleTypeName == "PointOnMap")
            {
                UpdateProjectLocationPoint(project, projectSimpleDto);
            }
            else
            {
                project.ProjectLocationPoint = null;
                if(projectSimpleDto.ProjectLocationSimpleType.ProjectLocationSimpleTypeName == "NamedAreas")
                {
                    project.ProjectLocationSimpleTypeID = ProjectLocationSimpleType.None.ProjectLocationSimpleTypeID;
                    project.ProjectLocationNotes =
                        $"Named Area: {projectSimpleDto.ProjectLocationAreaName}\r\n, Area Type: {projectSimpleDto.ProjectLocationAreaType}";
                }
            }
            
            project.ProposingDate = projectSimpleDto.CreationDate;
            project.LastUpdatedDate = projectSimpleDto.LastModificationDate ?? projectSimpleDto.CreationDate;
            project.ProjectApprovalStatusID = ProjectApprovalStatus.All.SingleOrDefault(x => x.ProjectApprovalStatusName == projectSimpleDto.ProjectApprovalStatus.ProjectApprovalStatusName)?.ProjectApprovalStatusID ?? project.ProjectApprovalStatusID;
            project.PerformanceMeasureNotes = projectSimpleDto.IndicatorNotes;
            project.SubmissionDate = projectSimpleDto.SubmissionDate;
            project.ApprovalDate = projectSimpleDto.ApprovalDate;

            // Match on email: both systems moved off the shared Keystone PersonGuid to Auth0, so
            // the GUID is no longer a usable cross-system key.
            var primaryContactPerson = !string.IsNullOrWhiteSpace(projectSimpleDto.PrimaryContactPersonEmail)
                ? databaseEntities.AllPeople.FirstOrDefault(x => x.TenantID == tenantID && x.Email == projectSimpleDto.PrimaryContactPersonEmail)
                : null;
            if (primaryContactPerson != null)
            {
                project.PrimaryContactPersonID = primaryContactPerson.PersonID;
                project.PrimaryContactPersonFullName = null;
                project.PrimaryContactPersonEmail = null;
            }
            else
            {
                project.PrimaryContactPersonFullName = projectSimpleDto.PrimaryContactPersonFullName;
                project.PrimaryContactPersonEmail = projectSimpleDto.PrimaryContactPersonEmail;
            }

            var strategyNameInDictionary = EIPActionPriorityNumberToTCSStrategy.TryGetValue(projectSimpleDto.EIPProject?.EIPActionPriority?.DisplayNumber ?? string.Empty, out var strategyName);
            if (strategyNameInDictionary)
            {
                var taxonomyLeaf = databaseEntities.AllTaxonomyLeafs.SingleOrDefault(x =>
                    x.TenantID == tenantID && x.TaxonomyLeafName == strategyName);
                project.TaxonomyLeafID = taxonomyLeaf?.TaxonomyLeafID ?? project.TaxonomyLeafID;

            }

            UpdateProjectOrganizations(project, projectSimpleDto, tenantID, databaseEntities);
            UpdateProjectLocations(project, projectSimpleDto, tenantID, databaseEntities);
            UpdateProjectFundingSourceBudgets(project, projectSimpleDto, tenantID, databaseEntities);
            UpdateProjectFundingExpenditures(project, projectSimpleDto, tenantID, databaseEntities);
            UpdatePerformanceMeasureExpecteds(project, projectSimpleDto, tenantID, databaseEntities);
            UpdatePerformanceMeasureActuals(project, projectSimpleDto, tenantID, databaseEntities);
            UpdateProjectExternalLinks(project, projectSimpleDto, tenantID, databaseEntities);
            //UpdateProjectImages(project, projectSimpleDto, tenantID, databaseEntities);
            UpdateProjectNotes(project, projectSimpleDto, tenantID, databaseEntities);
        }

        private void UpdateProjectOrganizations(Project project, ProjectSimpleDto projectSimpleDto, int tenantID, DatabaseEntities databaseEntities)
        {
            var projectOrganizationsUpdated = new List<ProjectOrganization>();
            var organizationRelationshipTypes = databaseEntities.AllOrganizationRelationshipTypes.Where(x => x.TenantID == tenantID).ToList();
            foreach (var projectOrgSimpleDto in projectSimpleDto.Organizations)
            {
                var relationshipType = projectOrgSimpleDto.IsLeadOrganization
                    ?
                    organizationRelationshipTypes.SingleOrDefault(x =>
                        x.OrganizationRelationshipTypeName == "Lead Implementer")
                    :
                    projectOrgSimpleDto.IsImplementingOrganization
                        ? organizationRelationshipTypes.SingleOrDefault(x =>
                            x.OrganizationRelationshipTypeName == "Partner")
                        :
                        projectOrgSimpleDto.IsFundingOrganization
                            ? organizationRelationshipTypes.SingleOrDefault(x =>
                                x.OrganizationRelationshipTypeName == "Funder")
                            : null;
                if (relationshipType == null)
                {
                    Logger.Warn($"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No relationship type found for Organization '{projectOrgSimpleDto.Organization.OrganizationName}'");
                    continue;
                }
                var organization = GetOrganizationByOrganizationName(projectOrgSimpleDto.Organization.OrganizationName, tenantID, databaseEntities);
                if (organization == null)
                {
                    Logger.Warn($"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Organization found for '{projectOrgSimpleDto.Organization.OrganizationName}', GUID: '{projectOrgSimpleDto.Organization.OrganizationGuid}'");
                    continue;
                }

                projectOrganizationsUpdated.Add(new ProjectOrganization(project.ProjectID, organization.OrganizationID,
                    relationshipType.OrganizationRelationshipTypeID) { TenantID = tenantID });
            }
            databaseEntities.ProjectOrganizations.Load();
            var allProjectOrganizations = databaseEntities.AllProjectOrganizations.Local;
            project.ProjectOrganizations.Merge(projectOrganizationsUpdated,
                allProjectOrganizations,
                (x, y) => x.TenantID == y.TenantID && x.ProjectID == y.ProjectID && x.OrganizationID == y.OrganizationID && x.OrganizationRelationshipTypeID == y.OrganizationRelationshipTypeID, databaseEntities);

        }

        private Organization GetOrganizationByOrganizationName(string organizationName, int tenantID, DatabaseEntities databaseEntities)
        {
            if (organizationName == "U.S. Bureau of Reclamation")
            {
                // special case, named differently in TCS Project Tracker
                return databaseEntities.AllOrganizations.SingleOrDefault(x => x.TenantID == tenantID && x.OrganizationName == "U.S. Bureau of Reclamation (California Great Basin Region)");
            }
            return databaseEntities.AllOrganizations.SingleOrDefault(x => x.TenantID == tenantID && x.OrganizationName == organizationName);
        }

        private void UpdateProjectLocationPoint(Project project, ProjectSimpleDto projectSimpleDto)
        {
            if (projectSimpleDto.ProjectLocationPointGeoJson == null)
            {
                return;
            }
            var htmlEncodedGeoJson = projectSimpleDto.ProjectLocationPointGeoJson.HtmlEncodeWithQuotes();
            var projectLocationPoint =  JsonTools.DeserializeObject<FeatureCollection>(htmlEncodedGeoJson);
            project.ProjectLocationPoint = projectLocationPoint.Features.Select(x => x.ToSqlGeometry()).FirstOrDefault().ToDbGeometry(LtInfoGeometryConfiguration.DefaultCoordinateSystemId);
        }

        private void UpdateProjectLocations(Project project, ProjectSimpleDto projectSimpleDto, int tenantID, DatabaseEntities databaseEntities)
        {
            foreach (var projectLocation in project.ProjectLocations.ToList())
            {
                projectLocation.DeleteFull(databaseEntities);
            }

            foreach (var projectLocationSimpleDto in projectSimpleDto.ProjectLocations)
            {
                var htmlEncodedGeoJson = projectLocationSimpleDto.ProjectLocationGeoJson.HtmlEncodeWithQuotes();
                var projectLocation = JsonTools.DeserializeObject<FeatureCollection>(htmlEncodedGeoJson);
                project.ProjectLocations.Add(new ProjectLocation(project.ProjectID, projectLocation.Features.Select(x => x.ToSqlGeometry()).FirstOrDefault().ToDbGeometry(LtInfoGeometryConfiguration.DefaultCoordinateSystemId))
                {
                    Annotation = projectLocationSimpleDto.Annotation,
                    TenantID = tenantID
                });
            }
        }

        private void UpdateProjectFundingSourceBudgets(Project project, ProjectSimpleDto projectSimpleDto, int tenantID, DatabaseEntities databaseEntities)
        {
            var projectFundingSourceBudgetsUpdated = new List<ProjectFundingSourceBudget>();
            var noFundingSourceIdentifiedUpdated = new List<ProjectNoFundingSourceIdentified>();
            if (projectSimpleDto.EstimatedTotalCost.HasValue)
            {
                project.FundingTypeID = FundingType.BudgetVariesByYear.FundingTypeID;
                var securedFunding = projectSimpleDto.ProjectFundingSourceRequests.Any() ? projectSimpleDto.ProjectFundingSourceRequests.Sum(x => x.SecuredAmount) : 0;
                var unfundedNeed = projectSimpleDto.EstimatedTotalCost.Value - securedFunding; //EstimatedTotalCost - GetSecuredFunding();
                var noFundingSourceIdentified = new ProjectNoFundingSourceIdentified(project.ProjectID)
                {
                    NoFundingSourceIdentifiedYet = unfundedNeed
                };
                noFundingSourceIdentifiedUpdated.Add(noFundingSourceIdentified);
            }
            foreach (var projectFundingSourceRequestSimpleDto in projectSimpleDto.ProjectFundingSourceRequests)
            {
                var organization = GetOrganizationByOrganizationName(projectFundingSourceRequestSimpleDto.FundingSource.Organization.OrganizationName, tenantID, databaseEntities);
                if (organization == null)
                {
                    Logger.Warn($"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Organization found for '{projectFundingSourceRequestSimpleDto.FundingSource.Organization.OrganizationName}', GUID: '{projectFundingSourceRequestSimpleDto.FundingSource.Organization.OrganizationGuid}'");
                    continue;
                }

                var fundingSource = databaseEntities.AllFundingSources.SingleOrDefault(x =>
                    x.TenantID == tenantID && x.OrganizationID == organization.OrganizationID && x.FundingSourceName ==
                    projectFundingSourceRequestSimpleDto.FundingSource.FundingSourceName);
                if (fundingSource == null)
                {
                    Logger.Warn($"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Funding Source found for '{projectFundingSourceRequestSimpleDto.FundingSource.FundingSourceName}', Organization Name '{projectFundingSourceRequestSimpleDto.FundingSource.Organization.OrganizationName}', GUID: '{projectFundingSourceRequestSimpleDto.FundingSource.Organization.OrganizationGuid}");
                    continue;
                }

                projectFundingSourceBudgetsUpdated.Add(new ProjectFundingSourceBudget(project.ProjectID, fundingSource.FundingSourceID)
                {
                    SecuredAmount = projectFundingSourceRequestSimpleDto.SecuredAmount,
                    TargetedAmount = projectFundingSourceRequestSimpleDto.UnsecuredAmount,
                    TenantID = tenantID
                });
            }

            databaseEntities.ProjectFundingSourceBudgets.Load();
            var allProjectFundingSourceBudgets = databaseEntities.AllProjectFundingSourceBudgets.Local;
            project.ProjectFundingSourceBudgets.Merge(projectFundingSourceBudgetsUpdated,
                allProjectFundingSourceBudgets,
                (x, y) => x.TenantID == y.TenantID && x.ProjectID == y.ProjectID && x.FundingSourceID == y.FundingSourceID,
                (x, y) =>
                {
                    x.SecuredAmount = y.SecuredAmount;
                    x.TargetedAmount = y.TargetedAmount;
                },
                databaseEntities);

            databaseEntities.ProjectNoFundingSourceIdentifieds.Load();
            var allProjectNoFundingSourceIdentifieds = databaseEntities.AllProjectNoFundingSourceIdentifieds.Local;
            project.ProjectNoFundingSourceIdentifieds.Merge(noFundingSourceIdentifiedUpdated,
                allProjectNoFundingSourceIdentifieds,
                (x, y) => x.TenantID == y.TenantID && x.ProjectID == y.ProjectID,
                (x, y) =>
                {
                    x.NoFundingSourceIdentifiedYet = y.NoFundingSourceIdentifiedYet;
                },
                databaseEntities);
        }

        private void UpdateProjectFundingExpenditures(Project project, ProjectSimpleDto projectSimpleDto, int tenantID, DatabaseEntities databaseEntities)
        {
            var projectFundingSourceExpendituresUpdated = new List<ProjectFundingSourceExpenditure>();
            if (!projectSimpleDto.ProjectFundingSourceExpenditures.Any())
            {
                project.ExpendituresNote = "No expenditures reported in the EIP Project Tracker.";
            }

            foreach (var projectFundingSourceExpenditureSimpleDto in projectSimpleDto.ProjectFundingSourceExpenditures)
            {
                var organization = GetOrganizationByOrganizationName(projectFundingSourceExpenditureSimpleDto.FundingSource.Organization.OrganizationName, tenantID, databaseEntities);
                if (organization == null)
                {
                    Logger.Warn($"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Organization found for '{projectFundingSourceExpenditureSimpleDto.FundingSource.Organization.OrganizationName}', GUID: '{projectFundingSourceExpenditureSimpleDto.FundingSource.Organization.OrganizationGuid}'");
                    continue;
                }

                var fundingSource = databaseEntities.AllFundingSources.SingleOrDefault(x =>
                    x.TenantID == tenantID && x.OrganizationID == organization.OrganizationID && x.FundingSourceName ==
                    projectFundingSourceExpenditureSimpleDto.FundingSource.FundingSourceName);
                if (fundingSource == null)
                {
                    Logger.Warn($"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Funding Source found for '{projectFundingSourceExpenditureSimpleDto.FundingSource.FundingSourceName}', Organization Name '{projectFundingSourceExpenditureSimpleDto.FundingSource.Organization.OrganizationName}', GUID: '{projectFundingSourceExpenditureSimpleDto.FundingSource.Organization.OrganizationGuid}");
                    continue;
                }

                var calendarYear = projectFundingSourceExpenditureSimpleDto.CalendarYear;
                var expenditureAmount = projectFundingSourceExpenditureSimpleDto.ExpenditureAmount;
                projectFundingSourceExpendituresUpdated.Add(new ProjectFundingSourceExpenditure(project.ProjectID, fundingSource.FundingSourceID, calendarYear, expenditureAmount)
                {
                    TenantID = tenantID
                });
            }

            databaseEntities.ProjectFundingSourceExpenditures.Load();
            var allProjectFundingSourceExpenditures = databaseEntities.AllProjectFundingSourceExpenditures.Local;
            project.ProjectFundingSourceExpenditures.Merge(projectFundingSourceExpendituresUpdated,
                allProjectFundingSourceExpenditures,
                (x, y) => x.TenantID == y.TenantID && x.ProjectID == y.ProjectID && x.FundingSourceID == y.FundingSourceID && x.CalendarYear == y.CalendarYear,
                (x, y) => x.ExpenditureAmount = y.ExpenditureAmount, databaseEntities);

        }

        private void UpdatePerformanceMeasureExpecteds(Project project, ProjectSimpleDto projectSimpleDto, int tenantID, DatabaseEntities databaseEntities)
        {
            databaseEntities.PerformanceMeasureExpecteds.Load();
            var allPerformanceMeasureExpecteds = databaseEntities.AllPerformanceMeasureExpecteds.Local;
            databaseEntities.PerformanceMeasureExpectedSubcategoryOptions.Load();
            var allPerformanceMeasureExpectedSubcategoryOptions = databaseEntities.AllPerformanceMeasureExpectedSubcategoryOptions.Local;

            // Remove all existing associations
            project.PerformanceMeasureExpecteds.ToList().ForEach(pmav =>
            {
                pmav.PerformanceMeasureExpectedSubcategoryOptions.ToList().ForEach(pmavso => allPerformanceMeasureExpectedSubcategoryOptions.Remove(pmavso));
                allPerformanceMeasureExpecteds.Remove(pmav);
            });
            project.PerformanceMeasureExpecteds.Clear();

            // completely rebuild the list
            foreach (var projectIndicatorExpectedValueSimpleDto in projectSimpleDto.ProjectIndicatorExpectedValues)
            {
                var pmInDictionary = EIPPerformaceMeasureToTCSPerformanceMeasure.TryGetValue(
                    projectIndicatorExpectedValueSimpleDto.Indicator.IndicatorDisplayName,
                    out var performanceMeasureName);
                if(!pmInDictionary)
                {
                    continue;
                }
                var performanceMeasure = databaseEntities.AllPerformanceMeasures.SingleOrDefault(x => x.TenantID == tenantID && x.PerformanceMeasureDisplayName == performanceMeasureName);
                if (performanceMeasure == null)
                {
                    Logger.Warn($"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Performance Measure found in Database for '{performanceMeasureName}'");
                    continue;
                }

                var performanceMeasureExpected =
                    new PerformanceMeasureExpected(project.ProjectID, performanceMeasure.PerformanceMeasureID)
                    {
                        ExpectedValue = projectIndicatorExpectedValueSimpleDto.ExpectedValue,
                        TenantID = tenantID
                    };
                allPerformanceMeasureExpecteds.Add(performanceMeasureExpected);


                var performanceMeasureSubcategories = performanceMeasure.PerformanceMeasureSubcategories.ToList();
                foreach (var performanceMeasureSubcategory in performanceMeasureSubcategories)
                {
                    // try to find the subcategory in the DTO, set the option from the DTO
                    var indicatorExpectedValueSubcategoryOptionSimpleDto =
                        projectIndicatorExpectedValueSimpleDto.ProjectIndicatorExpectedValueSubcategoryOptions
                            .SingleOrDefault(x =>
                                x.IndicatorSubcategory.IndicatorSubcategoryDisplayName == performanceMeasureSubcategory
                                    .PerformanceMeasureSubcategoryDisplayName);
                    PerformanceMeasureSubcategoryOption subcategoryOption;

                    // special case for this PM because the subcategory name matches in TCS, but we do not want to import the data from EIP
                    if (performanceMeasureName == "Acres of Habitat Restored/Created" || (performanceMeasureName == "Acres of Forest Fuels Reduction Treatment" && performanceMeasureSubcategory.PerformanceMeasureSubcategoryDisplayName == "Treatment Zone"))
                    {
                        subcategoryOption =
                            performanceMeasureSubcategory.PerformanceMeasureSubcategoryOptions.SingleOrDefault(x =>
                                x.PerformanceMeasureSubcategoryOptionName == "Unspecified");
                        if (subcategoryOption == null)
                        {
                            Logger.Warn(
                                $"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Performance Measure Subcategory Option found for 'Unspecified' (Performance Measure: '{performanceMeasureName}'; Subcategory: '{performanceMeasureSubcategory.PerformanceMeasureSubcategoryDisplayName}'");
                            continue;
                        }
                    }
                    else if (performanceMeasureName == "Acres of Environmentally Sensitive Land Acquired" || performanceMeasureName == "Length of Public Shoreline Added")
                    {
                        // EIP has the subcategory and option named, but it is the "Default" subcategory and option in TCS
                        subcategoryOption = performanceMeasureSubcategory.PerformanceMeasureSubcategoryOptions.SingleOrDefault(x =>
                                x.PerformanceMeasureSubcategoryOptionName == "Default");
                        if (subcategoryOption == null)
                        {
                            Logger.Warn(
                                $"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Performance Measure Subcategory Option found for 'Default' (Performance Measure: '{performanceMeasureName}'; Subcategory: '{performanceMeasureSubcategory.PerformanceMeasureSubcategoryDisplayName}'");
                            continue;
                        }
                    }
                    else if (indicatorExpectedValueSubcategoryOptionSimpleDto != null)
                    {
                        subcategoryOption =
                            performanceMeasureSubcategory.PerformanceMeasureSubcategoryOptions.SingleOrDefault(x =>
                                x.PerformanceMeasureSubcategoryOptionName.ToLower().Trim() ==
                                indicatorExpectedValueSubcategoryOptionSimpleDto.IndicatorSubcategoryOption
                                    .IndicatorSubcategoryOptionName.ToLower().Trim());
                        if (subcategoryOption == null)
                        {
                            Logger.Warn(
                                $"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Performance Measure Subcategory Option found for '{indicatorExpectedValueSubcategoryOptionSimpleDto.IndicatorSubcategoryOption.IndicatorSubcategoryOptionName}' (Performance Measure: '{performanceMeasureName}'; Subcategory: '{performanceMeasureSubcategory.PerformanceMeasureSubcategoryDisplayName}'");
                            continue;
                        }
                    }
                    else
                    {
                        subcategoryOption =
                            performanceMeasureSubcategory.PerformanceMeasureSubcategoryOptions.SingleOrDefault(x =>
                                x.PerformanceMeasureSubcategoryOptionName == "Unspecified");
                        // if it is not in there, then we default the option to "Unspecified"
                        if (subcategoryOption == null)
                        {
                            Logger.Warn(
                                $"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Performance Measure Subcategory Option found for 'Unspecified' (Performance Measure: '{performanceMeasureName}'; Subcategory: '{performanceMeasureSubcategory.PerformanceMeasureSubcategoryDisplayName}'");
                            continue;
                        }
                    }

                    if (subcategoryOption != null)
                    {
                        allPerformanceMeasureExpectedSubcategoryOptions.Add(
                            new PerformanceMeasureExpectedSubcategoryOption(
                                performanceMeasureExpected.PerformanceMeasureExpectedID,
                                subcategoryOption.PerformanceMeasureSubcategoryOptionID,
                                performanceMeasure.PerformanceMeasureID,
                                performanceMeasureSubcategory.PerformanceMeasureSubcategoryID)
                            {
                                TenantID = tenantID
                            });
                    }
                }
            }
        }

        private void UpdatePerformanceMeasureActuals(Project project, ProjectSimpleDto projectSimpleDto, int tenantID, DatabaseEntities databaseEntities)
        {
            databaseEntities.PerformanceMeasureActuals.Load();
            var allPerformanceMeasureActuals = databaseEntities.AllPerformanceMeasureActuals.Local;
            databaseEntities.PerformanceMeasureActualSubcategoryOptions.Load();
            var allPerformanceMeasureActualSubcategoryOptions = databaseEntities.AllPerformanceMeasureActualSubcategoryOptions.Local;
            databaseEntities.AllPerformanceMeasureReportingPeriods.Load();
            var allPerformanceMeasureReportingPeriods = databaseEntities.AllPerformanceMeasureReportingPeriods.Local;

            // Remove all existing associations
            project.PerformanceMeasureActuals.ToList().ForEach(pmav =>
            {
                pmav.PerformanceMeasureActualSubcategoryOptions.ToList().ForEach(pmavso => allPerformanceMeasureActualSubcategoryOptions.Remove(pmavso));
                allPerformanceMeasureActuals.Remove(pmav);
            });
            project.PerformanceMeasureActuals.Clear();

            // completely rebuild the list
            foreach (var projectIndicatorReportedValueSimpleDto in projectSimpleDto.ProjectIndicatorReportedValues)
            {
                var pmInDictionary = EIPPerformaceMeasureToTCSPerformanceMeasure.TryGetValue(
                    projectIndicatorReportedValueSimpleDto.Indicator.IndicatorDisplayName,
                    out var performanceMeasureName);
                if (!pmInDictionary)
                {
                    continue;
                }
                var performanceMeasure = databaseEntities.AllPerformanceMeasures.SingleOrDefault(x => x.TenantID == tenantID && x.PerformanceMeasureDisplayName == performanceMeasureName);
                if (performanceMeasure == null)
                {
                    Logger.Warn($"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Performance Measure found in Database for '{performanceMeasureName}'");
                    continue;
                }

                var reportingPeriod = allPerformanceMeasureReportingPeriods.SingleOrDefault(x => x.TenantID == tenantID && x.PerformanceMeasureReportingPeriodCalendarYear == projectIndicatorReportedValueSimpleDto.CalendarYear);
                if (reportingPeriod == null)
                {
                    // create the reporting period instead of logging an error and moving on
                    reportingPeriod = new PerformanceMeasureReportingPeriod(projectIndicatorReportedValueSimpleDto.CalendarYear, projectIndicatorReportedValueSimpleDto.CalendarYear.ToString())
                    {
                        TenantID = tenantID
                    };
                    allPerformanceMeasureReportingPeriods.Add(reportingPeriod);
                }
                var performanceMeasureActual =
                    new PerformanceMeasureActual(project.ProjectID, performanceMeasure.PerformanceMeasureID, projectIndicatorReportedValueSimpleDto.ActualValue, reportingPeriod.PerformanceMeasureReportingPeriodID)
                    {
                        TenantID = tenantID
                    };
                allPerformanceMeasureActuals.Add(performanceMeasureActual);


                var performanceMeasureSubcategories = performanceMeasure.PerformanceMeasureSubcategories.ToList();
                foreach (var performanceMeasureSubcategory in performanceMeasureSubcategories)
                {
                    // try to find the subcategory in the DTO, set the option from the DTO
                    var indicatorReportedValueSubcategoryOptionSimpleDto =
                        projectIndicatorReportedValueSimpleDto.ProjectIndicatorReportedValueSubcategoryOptions
                            .SingleOrDefault(x =>
                                x.IndicatorSubcategory.IndicatorSubcategoryDisplayName == performanceMeasureSubcategory
                                    .PerformanceMeasureSubcategoryDisplayName);
                    PerformanceMeasureSubcategoryOption subcategoryOption;

                    // special case for this PM because the subcategory name matches in TCS, but we do not want to import the data from EIP
                    if (performanceMeasureName == "Acres of Habitat Restored/Created" || (performanceMeasureName == "Acres of Forest Fuels Reduction Treatment" && performanceMeasureSubcategory.PerformanceMeasureSubcategoryDisplayName == "Treatment Zone"))
                    {
                        subcategoryOption =
                            performanceMeasureSubcategory.PerformanceMeasureSubcategoryOptions.SingleOrDefault(x =>
                                x.PerformanceMeasureSubcategoryOptionName == "Unspecified");
                        // if it is not in there, then we default the option to "Unspecified"
                        if (subcategoryOption == null)
                        {
                            Logger.Warn(
                                $"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Performance Measure Subcategory Option found for 'Unspecified' (Performance Measure: '{performanceMeasureName}'; Subcategory: '{performanceMeasureSubcategory.PerformanceMeasureSubcategoryDisplayName}'");
                            continue;
                        }
                    }
                    else if (performanceMeasureName == "Acres of Environmentally Sensitive Land Acquired" || performanceMeasureName == "Length of Public Shoreline Added")
                    {
                        // EIP has the subcategory and option named, but it is the "Default" subcategory and option in TCS
                        subcategoryOption = performanceMeasureSubcategory.PerformanceMeasureSubcategoryOptions.SingleOrDefault(x =>
                            x.PerformanceMeasureSubcategoryOptionName == "Default");
                        if (subcategoryOption == null)
                        {
                            Logger.Warn(
                                $"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Performance Measure Subcategory Option found for 'Default' (Performance Measure: '{performanceMeasureName}'; Subcategory: '{performanceMeasureSubcategory.PerformanceMeasureSubcategoryDisplayName}'");
                            continue;
                        }
                    }
                    else if (indicatorReportedValueSubcategoryOptionSimpleDto != null)
                    {
                        subcategoryOption =
                            performanceMeasureSubcategory.PerformanceMeasureSubcategoryOptions.SingleOrDefault(x =>
                                x.PerformanceMeasureSubcategoryOptionName.ToLower().Trim() ==
                                indicatorReportedValueSubcategoryOptionSimpleDto.IndicatorSubcategoryOption
                                    .IndicatorSubcategoryOptionName.ToLower().Trim());
                        if (subcategoryOption == null)
                        {
                            Logger.Warn(
                                $"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Performance Measure Subcategory Option found for '{indicatorReportedValueSubcategoryOptionSimpleDto.IndicatorSubcategoryOption.IndicatorSubcategoryOptionName}' (Performance Measure: '{performanceMeasureName}'; Subcategory: '{performanceMeasureSubcategory.PerformanceMeasureSubcategoryDisplayName}'");
                            continue;
                        }
                    }
                    else
                    {
                        // if it is not in there, then we default the option to "Unspecified"
                        subcategoryOption =
                            performanceMeasureSubcategory.PerformanceMeasureSubcategoryOptions.SingleOrDefault(x =>
                                x.PerformanceMeasureSubcategoryOptionName == "Unspecified");
                        if (subcategoryOption == null)
                        {
                            Logger.Warn(
                                $"\tProjectID: {project.ProjectID}; ExternalID: {projectSimpleDto.ProjectID}. No Performance Measure Subcategory Option found for 'Unspecified' (Performance Measure: '{performanceMeasureName}'; Subcategory: '{performanceMeasureSubcategory.PerformanceMeasureSubcategoryDisplayName}'");
                            continue;
                        }
                    }

                    if (subcategoryOption != null)
                    {
                        allPerformanceMeasureActualSubcategoryOptions.Add(
                            new PerformanceMeasureActualSubcategoryOption(
                                performanceMeasureActual.PerformanceMeasureActualID,
                                subcategoryOption.PerformanceMeasureSubcategoryOptionID,
                                performanceMeasure.PerformanceMeasureID,
                                performanceMeasureSubcategory.PerformanceMeasureSubcategoryID)
                            {
                                TenantID = tenantID
                            });
                    }
                }
            }
        }

        private void UpdateProjectExternalLinks(Project project, ProjectSimpleDto projectSimpleDto, int tenantID, DatabaseEntities databaseEntities)
        {
            var projectExternalLinksUpdated = new List<ProjectExternalLink>();
            if (projectSimpleDto.ProjectExternalLinks != null)
            {
                projectExternalLinksUpdated = projectSimpleDto.ProjectExternalLinks.Select(x => new ProjectExternalLink(project.ProjectID, x.ExternalLinkLabel, x.ExternalLinkUrl){TenantID = tenantID}).ToList();
            }

            databaseEntities.ProjectExternalLinks.Load();
            var allProjectExternalLinks = databaseEntities.AllProjectExternalLinks.Local;

            project.ProjectExternalLinks.Merge(projectExternalLinksUpdated,
                allProjectExternalLinks,
                (x, y) => x.TenantID == y.TenantID && x.ProjectID == y.ProjectID && x.ExternalLinkLabel == y.ExternalLinkLabel && x.ExternalLinkUrl == y.ExternalLinkUrl, databaseEntities);
        }

        

        private void UpdateProjectNotes(Project project, ProjectSimpleDto projectSimpleDto, int tenantID, DatabaseEntities databaseEntities)
        {
            var projectNotesUpdated = new List<ProjectNote>();
            foreach(var projectNoteSimpleDto in projectSimpleDto.ProjectNotes)
            {
                var projectNote = new ProjectNote(project.ProjectID, projectNoteSimpleDto.Note,
                    projectNoteSimpleDto.CreateDate)
                {
                    TenantID = tenantID,
                    UpdateDate = projectNoteSimpleDto.UpdateDate
                };

                // Match on email: both systems moved off the shared Keystone PersonGuid to Auth0.
                var createPerson = !string.IsNullOrWhiteSpace(projectNoteSimpleDto.CreatePersonEmail)
                    ? databaseEntities.AllPeople.FirstOrDefault(x => x.TenantID == tenantID && x.Email == projectNoteSimpleDto.CreatePersonEmail)
                    : null;
                if (createPerson != null)
                {
                    projectNote.CreatePersonID = createPerson.PersonID;
                }
                else
                {
                    projectNote.CreatePersonFullName = projectNoteSimpleDto.CreatePersonFullName;
                }
                var updatePerson = !string.IsNullOrWhiteSpace(projectNoteSimpleDto.UpdatePersonEmail)
                    ? databaseEntities.AllPeople.FirstOrDefault(x => x.TenantID == tenantID && x.Email == projectNoteSimpleDto.UpdatePersonEmail)
                    : null;
                if (updatePerson != null)
                {
                    projectNote.UpdatePersonID = updatePerson.PersonID;
                }
                else
                {
                    projectNote.UpdatePersonFullName = projectNoteSimpleDto.UpdatePersonFullName;
                }

                projectNotesUpdated.Add(projectNote);
            }

            databaseEntities.ProjectNotes.Load();
            var allProjectNotes = databaseEntities.AllProjectNotes.Local;

            project.ProjectNotes.Merge(projectNotesUpdated,
                allProjectNotes,
                (x, y) => x.TenantID == y.TenantID && x.ProjectID == y.ProjectID && x.Note == y.Note && x.CreateDate == y.CreateDate, databaseEntities);
        }
    }
}
