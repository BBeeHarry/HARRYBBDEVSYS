using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.Shared
{
    public class IViewModel
    {
        public int ID { get; set; }

        public string MODIFIED_BY { get; set; }

        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }

        public string CREATE_BY { get; set; }

        public Nullable<System.DateTime> CREATE_DATE { get; set; }

        public string FormState { get; set; }

        public string FormAction { get; set; }

        public string SubFormState { get; set; } //State for sub screen (Tab, popup)

        public string SubFormAction { get; set; } //State for sub screen (Tab, popup)


     
    }
}