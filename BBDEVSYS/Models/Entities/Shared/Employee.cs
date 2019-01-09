using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.Models.Shared
{
    public class Employee
    {
        public string EmpNo { get; set; }
        public string TitleTH { get; set; }
        public string FirstnameTH { get; set; }
        public string LastnameTH { get; set; }
        public string TitleEN { get; set; }
        public string FirstnameEN { get; set; }
        public string LastnameEN { get; set; }
        public string Email { get; set; }
        public string ADUser { get; set; }
        public string MobileNo { get; set; }
        public string OfficeTel { get; set; }
        public string IDCard { get; set; }
        public string EmpStatus { get; set; }
        public string GradeCode { get; set; }
        public string GradeText { get; set; }
        public string PositionID { get; set; }
        public string OrgID { get; set; }
        public string ManagerEmpNo { get; set; }
        
        public string OrgName { get; set; }
        public string OrgLevel { get; set; }
        public string PosName { get; set; }
        public string PosLevel { get; set; }
    }
}