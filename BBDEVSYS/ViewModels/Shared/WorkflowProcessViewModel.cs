using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.Shared
{
    public class WorkflowProcessViewModel
    {
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public string K2WorkflowProcess { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
    }
}