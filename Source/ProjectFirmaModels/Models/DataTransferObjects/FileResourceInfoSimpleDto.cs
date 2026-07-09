using System;


namespace ProjectFirmaModels.Models.DataTransferObjects
{
    public class FileResourceInfoSimpleDto
    {
        public int FileResourceInfoID { get; set; }
        public int FileResourceMimeTypeID { get; set; }
        public string OriginalBaseFilename { get; set; }
        public string OriginalFileExtension { get; set; }
        public Guid FileResourceInfoGUID { get; set; }
        public int CreatePersonID { get; set; }
        // Email is the join key for matching the create person to local People. There is no
        // cross-system person GUID (each system's Auth0 sub is tenant-specific).
        public string CreatePersonEmail { get; set; }
        public DateTime CreateDate { get; set; }
        public FileResourceDatumSimpleDto FileResourceDatum { get; set; }
        public FileResourceMimeTypeSimpleDto FileResourceMimeType { get; set; }
    }

}