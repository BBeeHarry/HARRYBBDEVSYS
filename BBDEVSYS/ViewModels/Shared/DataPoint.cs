using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BBDEVSYS.ViewModels.Shared
{
    [DataContract]
    public class DataPoint
    {

        public DataPoint(DateTime x, decimal y)
        {
            this.XX = x;
            this.Y = y;
        }
        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "x")]
        public DateTime XX;

     

        public DataPoint(decimal y, string label)
        {
            this.Y = y;
            this.Label = label;
        }

        public DataPoint(decimal x, decimal y)
        {
            this.X = x;
            this.Y = y;
        }


        public DataPoint(decimal x, decimal y, string label)
        {
            this.X = x;
            this.Y = y;
            this.Label = label;
        }

        public DataPoint(decimal x, decimal y, decimal z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public DataPoint(decimal x, decimal y, decimal z, string label)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Label = label;
        }


        //Explicitly setting the name to be used while serializing to JSON. 
        [DataMember(Name = "label")]
        public string Label = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<decimal> Y = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "x")]
        public Nullable<decimal> X = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "z")]
        public Nullable<decimal> Z = null;
    }
}