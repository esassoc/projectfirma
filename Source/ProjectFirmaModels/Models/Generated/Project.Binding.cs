//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Project]
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using CodeFirstStoreFunctions;
using LtInfo.Common;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;

namespace ProjectFirmaModels.Models
{
    // Table [dbo].[Project] is multi-tenant, so is attributed as IHaveATenantID
    [Table("[dbo].[Project]")]
    public partial class Project : IHavePrimaryKey, IHaveATenantID
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected Project()
        {
            this.NotificationProjects = new HashSet<NotificationProject>();
            this.PerformanceMeasureActuals = new HashSet<PerformanceMeasureActual>();
            this.PerformanceMeasureExpecteds = new HashSet<PerformanceMeasureExpected>();
            this.ProjectAssessmentQuestions = new HashSet<ProjectAssessmentQuestion>();
            this.ProjectAttachments = new HashSet<ProjectAttachment>();
            this.ProjectClassifications = new HashSet<ProjectClassification>();
            this.ProjectContacts = new HashSet<ProjectContact>();
            this.ProjectCustomAttributes = new HashSet<ProjectCustomAttribute>();
            this.ProjectEvaluations = new HashSet<ProjectEvaluation>();
            this.ProjectExemptReportingYears = new HashSet<ProjectExemptReportingYear>();
            this.ProjectExternalLinks = new HashSet<ProjectExternalLink>();
            this.ProjectFundingSourceBudgets = new HashSet<ProjectFundingSourceBudget>();
            this.ProjectFundingSourceExpenditures = new HashSet<ProjectFundingSourceExpenditure>();
            this.ProjectGeospatialAreas = new HashSet<ProjectGeospatialArea>();
            this.ProjectGeospatialAreaTypeNotes = new HashSet<ProjectGeospatialAreaTypeNote>();
            this.ProjectImages = new HashSet<ProjectImage>();
            this.ProjectInternalNotes = new HashSet<ProjectInternalNote>();
            this.ProjectLocations = new HashSet<ProjectLocation>();
            this.ProjectLocationStagings = new HashSet<ProjectLocationStaging>();
            this.ProjectNoFundingSourceIdentifieds = new HashSet<ProjectNoFundingSourceIdentified>();
            this.ProjectNotes = new HashSet<ProjectNote>();
            this.ProjectOrganizations = new HashSet<ProjectOrganization>();
            this.ProjectProjectStatuses = new HashSet<ProjectProjectStatus>();
            this.ProjectRelevantCostTypes = new HashSet<ProjectRelevantCostType>();
            this.ProjectTags = new HashSet<ProjectTag>();
            this.ProjectUpdateBatches = new HashSet<ProjectUpdateBatch>();
            this.SecondaryProjectTaxonomyLeafs = new HashSet<SecondaryProjectTaxonomyLeaf>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public Project(int projectID, int taxonomyLeafID, int projectStageID, string projectName, string projectDescription, int? implementationStartYear, int? completionYear, decimal? estimatedTotalCostDeprecated, DbGeometry projectLocationPoint, string performanceMeasureActualYearsExemptionExplanation, bool isFeatured, string projectLocationNotes, int? planningDesignStartYear, int projectLocationSimpleTypeID, decimal? estimatedAnnualOperatingCostDeprecated, int? fundingTypeID, int? primaryContactPersonID, int projectApprovalStatusID, int? proposingPersonID, DateTime? proposingDate, string performanceMeasureNotes, DateTime? submissionDate, DateTime? approvalDate, int? reviewedByPersonID, DbGeometry defaultBoundingBox, string expendituresNote, string expectedFundingUpdateNote, DateTime lastUpdatedDate, int projectCategoryID, string basicsComment, string customAttributesComment, string locationSimpleComment, string locationDetailedComment, string organizationsComment, string contactsComment, string expectedAccomplishmentsComment, string reportedAccomplishmentsComment, string budgetComment, string expendituresComment, string proposalClassificationsComment, string attachmentsNotesComment, string photosComment, int? submittedByPersonID, bool locationIsPrivate, int? solicitationID, string otherPartners, string externalLinksComment, int? externalID, string primaryContactPersonFullName, string primaryContactPersonEmail, decimal? gisAcres) : this()
        {
            this.ProjectID = projectID;
            this.TaxonomyLeafID = taxonomyLeafID;
            this.ProjectStageID = projectStageID;
            this.ProjectName = projectName;
            this.ProjectDescription = projectDescription;
            this.ImplementationStartYear = implementationStartYear;
            this.CompletionYear = completionYear;
            this.EstimatedTotalCostDeprecated = estimatedTotalCostDeprecated;
            this.ProjectLocationPoint = projectLocationPoint;
            this.PerformanceMeasureActualYearsExemptionExplanation = performanceMeasureActualYearsExemptionExplanation;
            this.IsFeatured = isFeatured;
            this.ProjectLocationNotes = projectLocationNotes;
            this.PlanningDesignStartYear = planningDesignStartYear;
            this.ProjectLocationSimpleTypeID = projectLocationSimpleTypeID;
            this.EstimatedAnnualOperatingCostDeprecated = estimatedAnnualOperatingCostDeprecated;
            this.FundingTypeID = fundingTypeID;
            this.PrimaryContactPersonID = primaryContactPersonID;
            this.ProjectApprovalStatusID = projectApprovalStatusID;
            this.ProposingPersonID = proposingPersonID;
            this.ProposingDate = proposingDate;
            this.PerformanceMeasureNotes = performanceMeasureNotes;
            this.SubmissionDate = submissionDate;
            this.ApprovalDate = approvalDate;
            this.ReviewedByPersonID = reviewedByPersonID;
            this.DefaultBoundingBox = defaultBoundingBox;
            this.ExpendituresNote = expendituresNote;
            this.ExpectedFundingUpdateNote = expectedFundingUpdateNote;
            this.LastUpdatedDate = lastUpdatedDate;
            this.ProjectCategoryID = projectCategoryID;
            this.BasicsComment = basicsComment;
            this.CustomAttributesComment = customAttributesComment;
            this.LocationSimpleComment = locationSimpleComment;
            this.LocationDetailedComment = locationDetailedComment;
            this.OrganizationsComment = organizationsComment;
            this.ContactsComment = contactsComment;
            this.ExpectedAccomplishmentsComment = expectedAccomplishmentsComment;
            this.ReportedAccomplishmentsComment = reportedAccomplishmentsComment;
            this.BudgetComment = budgetComment;
            this.ExpendituresComment = expendituresComment;
            this.ProposalClassificationsComment = proposalClassificationsComment;
            this.AttachmentsNotesComment = attachmentsNotesComment;
            this.PhotosComment = photosComment;
            this.SubmittedByPersonID = submittedByPersonID;
            this.LocationIsPrivate = locationIsPrivate;
            this.SolicitationID = solicitationID;
            this.OtherPartners = otherPartners;
            this.ExternalLinksComment = externalLinksComment;
            this.ExternalID = externalID;
            this.PrimaryContactPersonFullName = primaryContactPersonFullName;
            this.PrimaryContactPersonEmail = primaryContactPersonEmail;
            this.GisAcres = gisAcres;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public Project(int taxonomyLeafID, int projectStageID, string projectName, string projectDescription, bool isFeatured, int projectLocationSimpleTypeID, int projectApprovalStatusID, DateTime lastUpdatedDate, int projectCategoryID, bool locationIsPrivate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.TaxonomyLeafID = taxonomyLeafID;
            this.ProjectStageID = projectStageID;
            this.ProjectName = projectName;
            this.ProjectDescription = projectDescription;
            this.IsFeatured = isFeatured;
            this.ProjectLocationSimpleTypeID = projectLocationSimpleTypeID;
            this.ProjectApprovalStatusID = projectApprovalStatusID;
            this.LastUpdatedDate = lastUpdatedDate;
            this.ProjectCategoryID = projectCategoryID;
            this.LocationIsPrivate = locationIsPrivate;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public Project(TaxonomyLeaf taxonomyLeaf, ProjectStage projectStage, string projectName, string projectDescription, bool isFeatured, ProjectLocationSimpleType projectLocationSimpleType, ProjectApprovalStatus projectApprovalStatus, DateTime lastUpdatedDate, ProjectCategory projectCategory, bool locationIsPrivate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProjectID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.TaxonomyLeafID = taxonomyLeaf.TaxonomyLeafID;
            this.TaxonomyLeaf = taxonomyLeaf;
            taxonomyLeaf.Projects.Add(this);
            this.ProjectStageID = projectStage.ProjectStageID;
            this.ProjectName = projectName;
            this.ProjectDescription = projectDescription;
            this.IsFeatured = isFeatured;
            this.ProjectLocationSimpleTypeID = projectLocationSimpleType.ProjectLocationSimpleTypeID;
            this.ProjectApprovalStatusID = projectApprovalStatus.ProjectApprovalStatusID;
            this.LastUpdatedDate = lastUpdatedDate;
            this.ProjectCategoryID = projectCategory.ProjectCategoryID;
            this.LocationIsPrivate = locationIsPrivate;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static Project CreateNewBlank(TaxonomyLeaf taxonomyLeaf, ProjectStage projectStage, ProjectLocationSimpleType projectLocationSimpleType, ProjectApprovalStatus projectApprovalStatus, ProjectCategory projectCategory)
        {
            return new Project(taxonomyLeaf, projectStage, default(string), default(string), default(bool), projectLocationSimpleType, projectApprovalStatus, default(DateTime), projectCategory, default(bool));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return NotificationProjects.Any() || PerformanceMeasureActuals.Any() || PerformanceMeasureExpecteds.Any() || ProjectAssessmentQuestions.Any() || ProjectAttachments.Any() || ProjectClassifications.Any() || ProjectContacts.Any() || ProjectCustomAttributes.Any() || ProjectEvaluations.Any() || ProjectExemptReportingYears.Any() || ProjectExternalLinks.Any() || ProjectFundingSourceBudgets.Any() || ProjectFundingSourceExpenditures.Any() || ProjectGeospatialAreas.Any() || ProjectGeospatialAreaTypeNotes.Any() || ProjectImages.Any() || ProjectInternalNotes.Any() || ProjectLocations.Any() || ProjectLocationStagings.Any() || ProjectNoFundingSourceIdentifieds.Any() || ProjectNotes.Any() || ProjectOrganizations.Any() || ProjectProjectStatuses.Any() || ProjectRelevantCostTypes.Any() || ProjectTags.Any() || ProjectUpdateBatches.Any() || SecondaryProjectTaxonomyLeafs.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(NotificationProjects.Any())
            {
                dependentObjects.Add(typeof(NotificationProject).Name);
            }

            if(PerformanceMeasureActuals.Any())
            {
                dependentObjects.Add(typeof(PerformanceMeasureActual).Name);
            }

            if(PerformanceMeasureExpecteds.Any())
            {
                dependentObjects.Add(typeof(PerformanceMeasureExpected).Name);
            }

            if(ProjectAssessmentQuestions.Any())
            {
                dependentObjects.Add(typeof(ProjectAssessmentQuestion).Name);
            }

            if(ProjectAttachments.Any())
            {
                dependentObjects.Add(typeof(ProjectAttachment).Name);
            }

            if(ProjectClassifications.Any())
            {
                dependentObjects.Add(typeof(ProjectClassification).Name);
            }

            if(ProjectContacts.Any())
            {
                dependentObjects.Add(typeof(ProjectContact).Name);
            }

            if(ProjectCustomAttributes.Any())
            {
                dependentObjects.Add(typeof(ProjectCustomAttribute).Name);
            }

            if(ProjectEvaluations.Any())
            {
                dependentObjects.Add(typeof(ProjectEvaluation).Name);
            }

            if(ProjectExemptReportingYears.Any())
            {
                dependentObjects.Add(typeof(ProjectExemptReportingYear).Name);
            }

            if(ProjectExternalLinks.Any())
            {
                dependentObjects.Add(typeof(ProjectExternalLink).Name);
            }

            if(ProjectFundingSourceBudgets.Any())
            {
                dependentObjects.Add(typeof(ProjectFundingSourceBudget).Name);
            }

            if(ProjectFundingSourceExpenditures.Any())
            {
                dependentObjects.Add(typeof(ProjectFundingSourceExpenditure).Name);
            }

            if(ProjectGeospatialAreas.Any())
            {
                dependentObjects.Add(typeof(ProjectGeospatialArea).Name);
            }

            if(ProjectGeospatialAreaTypeNotes.Any())
            {
                dependentObjects.Add(typeof(ProjectGeospatialAreaTypeNote).Name);
            }

            if(ProjectImages.Any())
            {
                dependentObjects.Add(typeof(ProjectImage).Name);
            }

            if(ProjectInternalNotes.Any())
            {
                dependentObjects.Add(typeof(ProjectInternalNote).Name);
            }

            if(ProjectLocations.Any())
            {
                dependentObjects.Add(typeof(ProjectLocation).Name);
            }

            if(ProjectLocationStagings.Any())
            {
                dependentObjects.Add(typeof(ProjectLocationStaging).Name);
            }

            if(ProjectNoFundingSourceIdentifieds.Any())
            {
                dependentObjects.Add(typeof(ProjectNoFundingSourceIdentified).Name);
            }

            if(ProjectNotes.Any())
            {
                dependentObjects.Add(typeof(ProjectNote).Name);
            }

            if(ProjectOrganizations.Any())
            {
                dependentObjects.Add(typeof(ProjectOrganization).Name);
            }

            if(ProjectProjectStatuses.Any())
            {
                dependentObjects.Add(typeof(ProjectProjectStatus).Name);
            }

            if(ProjectRelevantCostTypes.Any())
            {
                dependentObjects.Add(typeof(ProjectRelevantCostType).Name);
            }

            if(ProjectTags.Any())
            {
                dependentObjects.Add(typeof(ProjectTag).Name);
            }

            if(ProjectUpdateBatches.Any())
            {
                dependentObjects.Add(typeof(ProjectUpdateBatch).Name);
            }

            if(SecondaryProjectTaxonomyLeafs.Any())
            {
                dependentObjects.Add(typeof(SecondaryProjectTaxonomyLeaf).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(Project).Name, typeof(NotificationProject).Name, typeof(PerformanceMeasureActual).Name, typeof(PerformanceMeasureExpected).Name, typeof(ProjectAssessmentQuestion).Name, typeof(ProjectAttachment).Name, typeof(ProjectClassification).Name, typeof(ProjectContact).Name, typeof(ProjectCustomAttribute).Name, typeof(ProjectEvaluation).Name, typeof(ProjectExemptReportingYear).Name, typeof(ProjectExternalLink).Name, typeof(ProjectFundingSourceBudget).Name, typeof(ProjectFundingSourceExpenditure).Name, typeof(ProjectGeospatialArea).Name, typeof(ProjectGeospatialAreaTypeNote).Name, typeof(ProjectImage).Name, typeof(ProjectInternalNote).Name, typeof(ProjectLocation).Name, typeof(ProjectLocationStaging).Name, typeof(ProjectNoFundingSourceIdentified).Name, typeof(ProjectNote).Name, typeof(ProjectOrganization).Name, typeof(ProjectProjectStatus).Name, typeof(ProjectRelevantCostType).Name, typeof(ProjectTag).Name, typeof(ProjectUpdateBatch).Name, typeof(SecondaryProjectTaxonomyLeaf).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.AllProjects.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            DeleteChildren(dbContext);
            Delete(dbContext);
        }
        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public void DeleteChildren(DatabaseEntities dbContext)
        {

            foreach(var x in NotificationProjects.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in PerformanceMeasureActuals.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in PerformanceMeasureExpecteds.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectAssessmentQuestions.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectAttachments.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectClassifications.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectContacts.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectCustomAttributes.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectEvaluations.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectExemptReportingYears.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectExternalLinks.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectFundingSourceBudgets.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectFundingSourceExpenditures.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectGeospatialAreas.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectGeospatialAreaTypeNotes.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectImages.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectInternalNotes.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectLocations.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectLocationStagings.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectNoFundingSourceIdentifieds.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectNotes.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectOrganizations.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectProjectStatuses.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectRelevantCostTypes.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectTags.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectUpdateBatches.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in SecondaryProjectTaxonomyLeafs.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int ProjectID { get; set; }
        public int TenantID { get; set; }
        public int TaxonomyLeafID { get; set; }
        public int ProjectStageID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public int? ImplementationStartYear { get; set; }
        public int? CompletionYear { get; set; }
        public decimal? EstimatedTotalCostDeprecated { get; set; }
        public DbGeometry ProjectLocationPoint { get; set; }
        public string PerformanceMeasureActualYearsExemptionExplanation { get; set; }
        public bool IsFeatured { get; set; }
        public string ProjectLocationNotes { get; set; }
        public int? PlanningDesignStartYear { get; set; }
        public int ProjectLocationSimpleTypeID { get; set; }
        public decimal? EstimatedAnnualOperatingCostDeprecated { get; set; }
        public int? FundingTypeID { get; set; }
        public int? PrimaryContactPersonID { get; set; }
        public int ProjectApprovalStatusID { get; set; }
        public int? ProposingPersonID { get; set; }
        public DateTime? ProposingDate { get; set; }
        public string PerformanceMeasureNotes { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int? ReviewedByPersonID { get; set; }
        public DbGeometry DefaultBoundingBox { get; set; }
        public string ExpendituresNote { get; set; }
        public string ExpectedFundingUpdateNote { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int ProjectCategoryID { get; set; }
        public string BasicsComment { get; set; }
        public string CustomAttributesComment { get; set; }
        public string LocationSimpleComment { get; set; }
        public string LocationDetailedComment { get; set; }
        public string OrganizationsComment { get; set; }
        public string ContactsComment { get; set; }
        public string ExpectedAccomplishmentsComment { get; set; }
        public string ReportedAccomplishmentsComment { get; set; }
        public string BudgetComment { get; set; }
        public string ExpendituresComment { get; set; }
        public string ProposalClassificationsComment { get; set; }
        public string AttachmentsNotesComment { get; set; }
        public string PhotosComment { get; set; }
        public int? SubmittedByPersonID { get; set; }
        public bool LocationIsPrivate { get; set; }
        public int? SolicitationID { get; set; }
        public string OtherPartners { get; set; }
        public string ExternalLinksComment { get; set; }
        public int? ExternalID { get; set; }
        public string PrimaryContactPersonFullName { get; set; }
        public string PrimaryContactPersonEmail { get; set; }
        public decimal? GisAcres { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectID; } set { ProjectID = value; } }

        public virtual ICollection<NotificationProject> NotificationProjects { get; set; }
        public virtual ICollection<PerformanceMeasureActual> PerformanceMeasureActuals { get; set; }
        public virtual ICollection<PerformanceMeasureExpected> PerformanceMeasureExpecteds { get; set; }
        public virtual ICollection<ProjectAssessmentQuestion> ProjectAssessmentQuestions { get; set; }
        public virtual ICollection<ProjectAttachment> ProjectAttachments { get; set; }
        public virtual ICollection<ProjectClassification> ProjectClassifications { get; set; }
        public virtual ICollection<ProjectContact> ProjectContacts { get; set; }
        public virtual ICollection<ProjectCustomAttribute> ProjectCustomAttributes { get; set; }
        public virtual ICollection<ProjectEvaluation> ProjectEvaluations { get; set; }
        public virtual ICollection<ProjectExemptReportingYear> ProjectExemptReportingYears { get; set; }
        public virtual ICollection<ProjectExternalLink> ProjectExternalLinks { get; set; }
        public virtual ICollection<ProjectFundingSourceBudget> ProjectFundingSourceBudgets { get; set; }
        public virtual ICollection<ProjectFundingSourceExpenditure> ProjectFundingSourceExpenditures { get; set; }
        public virtual ICollection<ProjectGeospatialArea> ProjectGeospatialAreas { get; set; }
        public virtual ICollection<ProjectGeospatialAreaTypeNote> ProjectGeospatialAreaTypeNotes { get; set; }
        public virtual ICollection<ProjectImage> ProjectImages { get; set; }
        public virtual ICollection<ProjectInternalNote> ProjectInternalNotes { get; set; }
        public virtual ICollection<ProjectLocation> ProjectLocations { get; set; }
        public virtual ICollection<ProjectLocationStaging> ProjectLocationStagings { get; set; }
        public virtual ICollection<ProjectNoFundingSourceIdentified> ProjectNoFundingSourceIdentifieds { get; set; }
        public virtual ICollection<ProjectNote> ProjectNotes { get; set; }
        public virtual ICollection<ProjectOrganization> ProjectOrganizations { get; set; }
        public virtual ICollection<ProjectProjectStatus> ProjectProjectStatuses { get; set; }
        public virtual ICollection<ProjectRelevantCostType> ProjectRelevantCostTypes { get; set; }
        public virtual ICollection<ProjectTag> ProjectTags { get; set; }
        public virtual ICollection<ProjectUpdateBatch> ProjectUpdateBatches { get; set; }
        public virtual ICollection<SecondaryProjectTaxonomyLeaf> SecondaryProjectTaxonomyLeafs { get; set; }
        public Tenant Tenant { get { return Tenant.AllLookupDictionary[TenantID]; } }
        public virtual TaxonomyLeaf TaxonomyLeaf { get; set; }
        public ProjectStage ProjectStage { get { return ProjectStage.AllLookupDictionary[ProjectStageID]; } }
        public ProjectLocationSimpleType ProjectLocationSimpleType { get { return ProjectLocationSimpleType.AllLookupDictionary[ProjectLocationSimpleTypeID]; } }
        public FundingType FundingType { get { return FundingTypeID.HasValue ? FundingType.AllLookupDictionary[FundingTypeID.Value] : null; } }
        public virtual Person PrimaryContactPerson { get; set; }
        public virtual Person ProposingPerson { get; set; }
        public virtual Person ReviewedByPerson { get; set; }
        public virtual Person SubmittedByPerson { get; set; }
        public ProjectApprovalStatus ProjectApprovalStatus { get { return ProjectApprovalStatus.AllLookupDictionary[ProjectApprovalStatusID]; } }
        public ProjectCategory ProjectCategory { get { return ProjectCategory.AllLookupDictionary[ProjectCategoryID]; } }
        public virtual Solicitation Solicitation { get; set; }

        public static class FieldLengths
        {
            public const int ProjectName = 140;
            public const int ProjectDescription = 4000;
            public const int PerformanceMeasureActualYearsExemptionExplanation = 4000;
            public const int ProjectLocationNotes = 4000;
            public const int PerformanceMeasureNotes = 500;
            public const int ExpectedFundingUpdateNote = 500;
            public const int BasicsComment = 1000;
            public const int CustomAttributesComment = 1000;
            public const int LocationSimpleComment = 1000;
            public const int LocationDetailedComment = 1000;
            public const int OrganizationsComment = 1000;
            public const int ContactsComment = 1000;
            public const int ExpectedAccomplishmentsComment = 1000;
            public const int ReportedAccomplishmentsComment = 1000;
            public const int BudgetComment = 1000;
            public const int ExpendituresComment = 1000;
            public const int ProposalClassificationsComment = 1000;
            public const int AttachmentsNotesComment = 1000;
            public const int PhotosComment = 1000;
            public const int OtherPartners = 500;
            public const int ExternalLinksComment = 1000;
            public const int PrimaryContactPersonFullName = 201;
            public const int PrimaryContactPersonEmail = 255;
        }
    }
}