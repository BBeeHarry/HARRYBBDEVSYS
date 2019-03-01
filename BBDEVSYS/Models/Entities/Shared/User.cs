using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BBDEVSYS.Models.Entities;

namespace BBDEVSYS.Models.Shared
{
    public class User
    {
        public User()
        {
            RoleList = new List<AppSingleRole>();
        }

        public string ADUser { get; set; }
        public string UserType { get; set; }
        public string UserCode { get; set; }
        public string DisplayNameTH { get; set; }
        public string DisplayNameEN { get; set; }
        public string Email { get; set; }

        //User Authorize

        public string AuthorizeAdmin { get; set; }

        public List<AppSingleRole> RoleList { get; set; }

        public string SideMenuHTML { get; set; }
    }
}