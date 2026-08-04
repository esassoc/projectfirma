//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vGeoServerProjectAttributes]
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
    public partial class vGeoServerProjectAttributes
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vGeoServerProjectAttributes()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vGeoServerProjectAttributes(int projectID, int primaryKey, string projectDescription, string projectStage, string taxonomyLeaf, string primaryContactOrganization, string primaryContactPerson, int? planningDesignStartYear, int? implementationStartYear, int? completionYear, decimal? securedFunding, decimal? targetedFunding, decimal? estimatedTotalCost, DateTime projectLastUpdated) : this()
        {
            this.ProjectID = projectID;
            this.PrimaryKey = primaryKey;
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
        public vGeoServerProjectAttributes(vGeoServerProjectAttributes vGeoServerProjectAttributes) : this()
        {
            this.ProjectID = vGeoServerProjectAttributes.ProjectID;
            this.PrimaryKey = vGeoServerProjectAttributes.PrimaryKey;
            this.ProjectDescription = vGeoServerProjectAttributes.ProjectDescription;
            this.ProjectStage = vGeoServerProjectAttributes.ProjectStage;
            this.TaxonomyLeaf = vGeoServerProjectAttributes.TaxonomyLeaf;
            this.PrimaryContactOrganization = vGeoServerProjectAttributes.PrimaryContactOrganization;
            this.PrimaryContactPerson = vGeoServerProjectAttributes.PrimaryContactPerson;
            this.PlanningDesignStartYear = vGeoServerProjectAttributes.PlanningDesignStartYear;
            this.ImplementationStartYear = vGeoServerProjectAttributes.ImplementationStartYear;
            this.CompletionYear = vGeoServerProjectAttributes.CompletionYear;
            this.SecuredFunding = vGeoServerProjectAttributes.SecuredFunding;
            this.TargetedFunding = vGeoServerProjectAttributes.TargetedFunding;
            this.EstimatedTotalCost = vGeoServerProjectAttributes.EstimatedTotalCost;
            this.ProjectLastUpdated = vGeoServerProjectAttributes.ProjectLastUpdated;
            CallAfterConstructor(vGeoServerProjectAttributes);
        }

        partial void CallAfterConstructor(vGeoServerProjectAttributes vGeoServerProjectAttributes);

        public int ProjectID { get; set; }
        public int PrimaryKey { get; set; }
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