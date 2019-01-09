
using BBDEVSYS.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.CenterSetting
{
    public class ValueHelpSettingItemViewModel:IViewModel
    {
        public ValueHelpSettingItemViewModel()
        {
            StatusValueHelp = new List<ValueHelpViewModel>();
        }
        public string ValueType { get; set; }
        
        public string ValueKey { get; set; }
       
        public string ValueText { get; set; }
        public Nullable<int> Sequence { get; set; }
        public string Status { get; set; }

        public bool DeleteFlag { get; set; }

        public List<ValueHelpViewModel> StatusValueHelp { get; set; }
    }
}