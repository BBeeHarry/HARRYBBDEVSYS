using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.Models.Shared
{
    public class Organization
    {
        public Organization()
        {
            Manager = new Employee();
        }

        public string OrgID { get; set; }
        public string OrgName { get; set; }
        public string OrgLevel { get; set; }
        
        public string parentOrgID { get; set; }
        public string parentOrgName { get; set; }

        //Org manager
        public Employee Manager { get; set; }
    }
}