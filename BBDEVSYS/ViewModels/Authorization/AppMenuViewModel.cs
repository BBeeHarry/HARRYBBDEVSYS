using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.Authorization
{
    public class AppMenuViewModel
    {

        public AppMenuViewModel() {
            IsRoleForManage = false;
            IsRoleForDisplay = false;
            CheckRoleForManage = false;
            CheckRoleForDisplay = false;
        }

        public string MenuCode { get; set; }
        public string ResourceName { get; set; }
        public string MenuText { get; set; }
        public string MenuType { get; set; }
        public string ParentMenuCode { get; set; }
        public Nullable<int> Sequence { get; set; }
        public string RoleForManage { get; set; }
        public string RoleForDisplay { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
        public string Icon { get; set; }
        public bool IsRoleForManage { get; set; }
        public bool IsRoleForDisplay { get; set; }
        public bool CheckRoleForManage { get; set; }
        public bool CheckRoleForDisplay { get; set; }
        public int Level { get; set; }
    }
}