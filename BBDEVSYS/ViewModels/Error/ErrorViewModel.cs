using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.Error
{
    public class ErrorViewModel
    {
        public string message { get; set; }
        public string source { get; set; }
        public string stacktrace { get; set; }
        public string innerexception { get; set; }
    }
}