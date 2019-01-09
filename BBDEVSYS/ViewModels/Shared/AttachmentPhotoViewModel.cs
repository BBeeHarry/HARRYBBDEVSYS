using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.Shared
{
    public class AttachmentPhotoViewModel
    {
        public string PreviewPhoto { get; set; }
        public string PreviewNoPhoto { get; set; }
        public bool DeletedPhotoFlag { get; set; }
        public string PhotoSavedFilename { get; set; }
        public string FilenameToDelete { get; set; }
    }
}