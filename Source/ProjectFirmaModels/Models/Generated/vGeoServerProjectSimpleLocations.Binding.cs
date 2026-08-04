//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vGeoServerProjectSimpleLocations]
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using CodeFirstStoreFunctions;
using LtInfo.Common;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;

namespace ProjectFirmaModels.Models
{
    public partial class vGeoServerProjectSimpleLocations
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vGeoServerProjectSimpleLocations()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vGeoServerProjectSimpleLocations(int projectID, int primaryKey, string projectName, int tenantID, string tenantName, string projectDescription, string projectStage, string taxonomyLeaf, string primaryContactOrganization, string primaryContactPerson, int? planningDesignStartYear, int? implementationStartYear, int? completionYear, decimal? securedFunding, decimal? targetedFunding, decimal? estimatedTotalCost, DateTime projectLastUpdated) : this()
        {
            this.ProjectID = projectID;
            this.PrimaryKey = primaryKey;
            this.ProjectName = projectName;
            this.TenantID = tenantID;
            this.TenantName = tenantName;
            this.ProjectDescription = projectDescription;
            this.ProjectStage = projectStage;
            this.TaxonomyLeaf = taxonomyLeaf;
            this.PrimaryContactOrganization = primaryContactOrganization;
            this.PrimaryContactPerson = primaryContactPerson;
            this.PlanningDesignStartYear = planningDesignStartYear;
            this.ImplementationStartYear = implementationStartYear;
            this.CompletionYear = completionYear;
            this.SecuredFunding = securedFunding;
            this.TargetedFunding = targetedFunding;
            this.EstimatedTotalCost = estimatedTotalCost;
            this.ProjectLastUpdated = projectLastUpdated;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public vGeoServerProjectSimpleLocations(vGeoServerProjectSimpleLocations vGeoServerProjectSimpleLocations) : this()
        {
            this.ProjectID = vGeoServerProjectSimpleLocations.ProjectID;
            this.PrimaryKey = vGeoServerProjectSimpleLocations.PrimaryKey;
            this.ProjectName = vGeoServerProjectSimpleLocations.ProjectName;
            this.TenantID = vGeoServerProjectSimpleLocations.TenantID;
            this.TenantName = vGeoServerProjectSimpleLocations.TenantName;
            this.ProjectDescription = vGeoServerProjectSimpleLocations.ProjectDescription;
            this.ProjectStage = vGeoServerProjectSimpleLocations.ProjectStage;
            this.TaxonomyLeaf = vGeoServerProjectSimpleLocations.TaxonomyLeaf;
            this.PrimaryContactOrganization = vGeoServerProjectSimpleLocations.PrimaryContactOrganization;
            this.PrimaryContactPerson = vGeoServerProjectSimpleLocations.PrimaryContactPerson;
            this.PlanningDesignStartYear = vGeoServerProjectSimpleLocations.PlanningDesignStartYear;
            this.ImplementationStartYear = vGeoServerProjectSimpleLocations.ImplementationStartYear;
            this.CompletionYear = vGeoServerProjectSimpleLocations.CompletionYear;
            this.SecuredFunding = vGeoServerProjectSimpleLocations.SecuredFunding;
            this.TargetedFunding = vGeoServerProjectSimpleLocations.TargetedFunding;
            this.EstimatedTotalCost = vGeoServerProjectSimpleLocations.EstimatedTotalCost;
            this.ProjectLastUpdated = vGeoServerProjectSimpleLocations.ProjectLastUpdated;
            CallAfterConstructor(vGeoServerProjectSimpleLocations);
        }

        partial void CallAfterConstructor(vGeoServerProjectSimpleLocations vGeoServerProjectSimpleLocations);

        public int ProjectID { get; set; }
        public int PrimaryKey { get; set; }
        public string ProjectName { get; set; }
        public int TenantID { get; set; }
        public string TenantName { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectStage { get; set; }
        public string TaxonomyLeaf { get; set; }
        public string PrimaryContactOrganization { get; set; }
        public string PrimaryContactPerson { get; set; }
        public int? PlanningDesignStartYear { get; set; }
        public int? ImplementationStartYear { get; set; }
        public int? CompletionYear { get; set; }
        public decimal? SecuredFunding { get; set; }
        public decimal? TargetedFunding { get; set; }
        public decimal? EstimatedTotalCost { get; set; }
        public DateTime ProjectLastUpdated { get; set; }
    }
}