using BBDEVSYS.Content.text;
using BBDEVSYS.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.Authorization
{
    public class AssignUserRoleViewModel : IViewModel
    {
        public AssignUserRoleViewModel()
        {
            AssignUserTypeList = new List<ValueHelpViewModel>();
            AssignUserRoleItemList = new List<AssignUserRoleItemViewModel>();
            Photo = new AttachmentPhotoViewModel();
        }
        //Authorization
        public const string RoleForManageData = "Role_MA_Assign_User_Role";
        public const string RoleForDisplayData = "Role_DS_Assign_User_Role";

        public const string ProcessCode = "AssignUserRoleProcess";

        [Display(Name = "Code", ResourceType = typeof(ResourceText))]
        public string AssignUserCode { get; set; }
        [Display(Name = "FirstnameTH", ResourceType = typeof(ResourceText))]
        public string AssignUserName { get;set;}
        [Display(Name = "Username", ResourceType = typeof(ResourceText))]
        public string UserName { get; set; }
        public string AssignAvailableReason { get; set; }

        [Display(Name = "Type", ResourceType = typeof(ResourceText))]
        public string AssignUserType { get; set; }
        public string AssignUserRoleIcon { get; set; }

        
        public AttachmentPhotoViewModel Photo { get; set; }
        public List<ValueHelpViewModel> AssignUserTypeList { get;  set; }
        public List<AssignUserRoleItemViewModel> AssignUserRoleItemList { get;  set; }
        public AssignUserRoleItemViewModel AssignUserRoleItem { get; set; }
    }
}