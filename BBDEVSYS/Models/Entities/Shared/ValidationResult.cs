using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.Models.Shared
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            ErrorFlag = false;
            ModelStateErrorList = new List<ModelStateError>();
        }

        public string MessageType { get; set; }
        public string Message { get; set; }
        public bool ErrorFlag { get; set; }
        public string AdditionalInfo1 { get; set; }
        public string AdditionalInfo2 { get; set; }
        public string AdditionalInfo3 { get; set; }

        public List<ModelStateError> ModelStateErrorList { get; set; }
    }
}