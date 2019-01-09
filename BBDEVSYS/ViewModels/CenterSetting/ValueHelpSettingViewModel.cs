using BBDEVSYS.Content.text;
using BBDEVSYS.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.CenterSetting
{
    public class ValueHelpSettingViewModel :IViewModel
    {
        public ValueHelpSettingViewModel() {
            StatusValueHelp = new List<ValueHelpViewModel>();
            ValueHelpSettingList = new List<ValueHelpSettingItemViewModel>();
        }

        //Authorization
        public const string RoleForManageData = "Role_MA_Value_Help_Setting";
        public const string RoleForDisplayData = "Role_DS_Value_Help_Setting";

        public string ValueType { get; set; }
        [DisplayName("Key")]
        public string ValueKey { get; set; }

        [Display(Name = "Type", ResourceType = typeof(ResourceText))]
        public string ValueHelpText { get; set; }
        [DisplayName("ข้อความ")]
        public string ValueText { get; set; }
       
        public Nullable<int> Sequence { get; set; }
        [Display(Name = "STATUS", ResourceType = typeof(ResourceText))]
        public string Status { get; set; }

        public List<ValueHelpViewModel> StatusValueHelp { get;  set; }
        public List<ValueHelpSettingItemViewModel> ValueHelpSettingList { get; set; }
    }
}