using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using SourceCode.Workflow.Client;

namespace BBDEVSYS.ViewModels.Shared
{
    public class TaskViewModel
    {
        //public Actions Actions { get; }
        public string AllocatedUser { get; set; }
        public string Data { get; set; }
        public int ID { get; set; }
        public string Folio { get; set; }
        public string SerialNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Folder { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string RequesterName { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public int DataID { get; set; }
    }
}