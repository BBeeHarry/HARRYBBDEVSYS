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
    
    public partial class PAYMENT_ITEMS_CHAGE
    {
        public int ID { get; set; }
        public Nullable<int> SEQUENCE { get; set; }
        public Nullable<int> PAYMENT_ITEMS_ID { get; set; }
        public string PAYMENT_ITEMS_FEE_NAME { get; set; }
        public string CHARGE_TYPE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PAYMENT_ITEMS_NAME { get; set; }
        public Nullable<bool> IS_ACTIVE { get; set; }
    }
}
