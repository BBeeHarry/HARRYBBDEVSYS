using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.Models.Shared
{
    public class ARMTrCountWebarm
    {
        public string AgentCode { get; set; }
        public string ShopName { get; set; }
        public DateTime BuyDate { get; set; }
        public short DD { get; set; }
        public short MM { get; set; }
        public short YY { get; set; }
        public int CountData { get; set; }
        public int Qty { get; set; }
        public int Week { get; set; }
        public bool FlagUse { get; set; }
    }
}