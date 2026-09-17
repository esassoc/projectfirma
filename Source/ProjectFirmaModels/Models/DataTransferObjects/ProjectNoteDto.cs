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
        // Email is the join key for matching note authors to local People. There is no cross-system
        // person GUID (each system's Auth0 sub is tenant-specific, so it can't be used to join).
        public string CreatePersonEmail { get; set; }
        public string UpdatePersonEmail { get; set; }
    }

}