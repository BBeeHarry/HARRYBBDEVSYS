using BBDEVSYS.Content.text;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.ViewModels.Shared;

namespace BBDEVSYS.ViewModels.Authorization
{
    public class AssignUserRoleItemViewModel:IViewModel
    {
        public AssignUserRoleItemViewModel()
        {
            CompositeRoleList = new List<AppCompositeRole>();
        }
       
        //User Role
        public string Username { get; set; }
        [Display(Name ="STATUS",ResourceType =typeof(ResourceText))]
        public string StatusText { get; set; }
        [Display(Name = "ACRName", ResourceType = typeof(ResourceText))]
        public Nullable<int> CompositeRoleID { get; set; }
        public List<AppCompositeRole> CompositeRoleList { get; set; }
        public string PhotoFileName { get; set; }

        public bool DeleteFlag { get; set; }


    }
}