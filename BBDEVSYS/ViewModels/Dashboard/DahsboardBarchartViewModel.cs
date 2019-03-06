using BBDEVSYS.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BBDEVSYS.ViewModels.Dashboard
{
    public class DahsboardBarchartViewModel
    {
        public DahsboardBarchartViewModel() {
            DatapointsList1 = new List<DataPoint>();
            DatapointsList2 = new List<DataPoint>();
        }
        public decimal Actual { get; set; }
        public decimal Accrued { get; set; }

        public decimal Total { get; set; }

        public decimal MonthFee { get; set; }

        public string MonthNameFee { get; set; }

        public string PriceCatalog { get; set; }

        public string Company { get; set; }



        public List<DataPoint> DatapointsList1 { get;  set; }
        public List<DataPoint> DatapointsList2 { get; set; }

      
    }
}