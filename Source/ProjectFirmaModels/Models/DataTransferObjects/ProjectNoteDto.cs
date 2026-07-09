using System;

namespace ProjectFirmaModels.Models.DataTransferObjects
{

    public class ProjectNoteSimpleDto
    {
        public int ProjectNoteID { get; set; }
        public int ProjectID { get; set; }
        public string Note { get; set; }
        public int? CreatePersonID { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdatePersonID { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreatePersonFullName { get; set; }
        public string UpdatePersonFullName { get; set; }
        public Guid? CreatePersonGuid{ get; set; }
        public Guid? UpdatePersonGuid { get; set; }
        // Email is the join key for matching note authors to local People (both systems moved
        // off the shared Keystone GUID to Auth0).
        public string CreatePersonEmail { get; set; }
        public string UpdatePersonEmail { get; set; }
    }

}