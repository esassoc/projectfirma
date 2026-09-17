using System;
using System.Collections.Generic;


namespace ProjectFirmaModels.Models.DataTransferObjects
{
    public class ProjectSimpleDto
    {
        public int ProjectID { get; set; }
        public int ProjectStageID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public int? ImplementationStartYear { get; set; }
        public int? CompletionYear { get; set; }
        public decimal? EstimatedTotalCost { get; set; }
        public int? ProjectLocationAreaID { get; set; }
        public string ProjectIndicatorReportedValueYearsExemptionExplanation { get; set; }
        public string ProjectLocationNotes { get; set; }
        public int? PlanningDesignStartYear { get; set; }
        public int ProjectLocationSimpleTypeID { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public string NoDetailedLocationExplanation { get; set; }
        public int ProjectApprovalStatusID { get; set; }
        public int? ProposingPersonID { get; set; }
        public string IndicatorNotes { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int? ReviewedByPersonID { get; set; }
        public bool? EIPReviewComplete { get; set; }
        public bool? TransportationReviewComplete { get; set; }
        public bool? LakeClarityReviewComplete { get; set; }
        public int? PrimaryContactPersonID { get; set; }
        public ProjectStageSimpleDto ProjectStage { get; set; }
        public ProjectLocationSimpleTypeSimpleDto ProjectLocationSimpleType { get; set; }
        public string ProjectLocationAreaName { get; set; }
        public string ProjectLocationAreaType { get; set; }
        public string PrimaryContactPersonFullName { get; set; }
        public string PrimaryContactPersonEmail { get; set; }
        public ProjectApprovalStatusSimpleDto ProjectApprovalStatus { get; set; }
        public List<ProjectImplementingOrganizationOrProjectFundingOrganizationSimpleDto> Organizations { get; set; }
        public string ProjectLocationPointGeoJson { get; set; }
        public List<ProjectLocationSimpleDto> ProjectLocations { get; set; }
        public List<ProjectFundingSourceRequestSimpleDto> ProjectFundingSourceRequests { get; set; }
        public List<ProjectFundingSourceExpenditureSimpleDto> ProjectFundingSourceExpenditures { get; set; }
        public List<ProjectIndicatorExpectedValueSimpleDto> ProjectIndicatorExpectedValues { get; set; }
        public List<ProjectIndicatorReportedValueSimpleDto> ProjectIndicatorReportedValues { get; set; }
        public List<ProjectExternalLinkSimpleDto> ProjectExternalLinks { get; set; }
        //public List<ProjectImageSimpleDto> ProjectImages { get; set; }
        public EIPProjectSimpleDto EIPProject { get; set; }
        public List<ProjectNoteSimpleDto> ProjectNotes { get; set; }
    }

}