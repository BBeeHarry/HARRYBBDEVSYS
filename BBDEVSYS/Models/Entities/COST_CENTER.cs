//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BBDEVSYS.Models.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class COST_CENTER
    {
        public int ID { get; set; }
        public string CCT_CODE { get; set; }
        public string GL_ACCOUNT { get; set; }
        public string COST_CENTER1 { get; set; }
        public string COMPANY_CODE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string CREATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public string REMARK { get; set; }
        public string FUND_CODE { get; set; }
    }
}
