using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.Models.Shared
{
    public class Product
    {
        public string InvoiceUploadCode { get; set; }
        public string DescriptionTH { get; set; }
        public string SalesUnit { get; set; }
        public Nullable<decimal> UnitPerPack { get; set; }
        public Nullable<decimal> LitrePerPack { get; set; }
        public string ProductHieLevel1 { get; set; }
        public string ProductHieLevel2 { get; set; }
        public string ProductHieLevel3 { get; set; }
        public string ProductHieLevel4 { get; set; }
        public string ProductHieLevel5 { get; set; }
        public string ProductHieLevel6 { get; set; }
        public string ProductHieLevel7 { get; set; }
        public string InvoiceUploadGroup { get; set; }
        public Nullable<System.DateTime> LastInterface { get; set; }
        public string InvoiceUploadType { get; set; }

        public string ProductNameWithCode { get; set; }
    }
}