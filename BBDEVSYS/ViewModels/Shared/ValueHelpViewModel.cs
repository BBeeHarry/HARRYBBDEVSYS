using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.Shared
{
    public class ValueHelpViewModel
    {
        public string ValueType { get; set; }
        public string ValueKey { get; set; }
        public string ValueText { get; set; }
        public int ID { get; set; }
        public Nullable<int> Sequence { get; set; }
        public string Status { get; set; }
    }
}