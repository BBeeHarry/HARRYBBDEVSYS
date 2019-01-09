using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.Shared
{
    public class AttachmentViewModel
    {
        public AttachmentViewModel()
        {
            DocumentTypeValueHelp = new List<ValueHelpViewModel>();
        }

        public int ID { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public double FileSizeMB { get; set; }
        public byte[] FileContent { get; set; }
        public string MimeType { get; set; }
        public DateTime AttachDate { get; set; }
        public string AttachBy { get; set; }
        public string DownloadURL { get; set; }
        public string SavedFileName { get; set; }
        public string FileExtension { get; set; }
        public string FileUniqueKey { get; set; }
        public bool ErrorFlag { get; set; }
        public bool DeleteFlag { get; set; }
        public bool ReadOnlyFlag { get; set; }
        public string DocumentFilePath { get; set; }
        //public string TargetFilePath { get; set; }

        public string ProcessCode { get; set; }
        //public Nullable<int> DataID { get; set; }
        public string DataKey { get; set; }
        public Nullable<System.DateTime> DocumentDate { get; set; }
        public string DocumentType { get; set; }
        public string Remark { get; set; }
        public string AttachmentGroup { get; set; }

        public string DocumentTypeValueType { get; set; }
        public List<ValueHelpViewModel> DocumentTypeValueHelp { get; set; }



    }
}