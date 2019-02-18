
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BBDEVSYS.ViewModels.AccruedReport;
using System.ComponentModel.DataAnnotations;
using BBDEVSYS.Content.text;
using BBDEVSYS.Models.Shared;
using FluentValidation;
using BBDEVSYS.ViewModels.Shared;
using System.Web.Mvc;


namespace BBDEVSYS.ViewModels.AccruedReport
{

    [FluentValidation.Attributes.Validator(typeof(AccruedReportViewModelValidator))]
    public class AccruedReportViewModel 
    {
       
      
        public string CHANNELS { get; set; }
        public string FEE { get; set; }
        public string CHARGE { get; set; }

        public string Jan { get; set; }
        public string Feb { get; set; }
        public string Mar { get; set; }
        public string Apr { get; set; }
        public string May { get; set; }
        public string Jun { get; set; }
        public string Jul { get; set; }
        public string Aug { get; set; }
        public string Sep { get; set; }
        public string Oct { get; set; }
        public string Nov { get; set; }

        public string Dec { get; set; }

        public string Jan18 { get; set; }
        public string Feb18 { get; set; }
        public string Mar18 { get; set; }
        public string Apr18 { get; set; }
        public string May18 { get; set; }
        public string Jun18 { get; set; }
        public string Jul18 { get; set; }
        public string Aug18 { get; set; }
        public string Sep18 { get; set; }
        public string Oct18 { get; set; }
        public string Nov18 { get; set; }
        public string Dec18 { get; set; }


        public string Jan19 { get; set; }
        public string Feb19 { get; set; }
        public string Mar19 { get; set; }
        public string Apr19 { get; set; }
        public string May19 { get; set; }
        public string Jun19 { get; set; }
        public string Jul19 { get; set; }
        public string Aug19 { get; set; }
        public string Sep19 { get; set; }
        public string Oct19 { get; set; }
        public string Nov19 { get; set; }
        public string Dec19 { get; set; }


      

        //Dictionary<string, object> properties = new Dictionary<string, object>();

        //public object this[string name]
        //{
        //    get
        //    {
        //        if (properties.ContainsKey(name))
        //        {
        //            return properties[name];
        //        }
        //        return null;
        //    }
        //    set
        //    {
        //        properties[name] = value;
        //    }
        //}

        //  public Nullable<bool> ISPLAN { get; set; 
        public class AccruedReportViewModelValidator : AbstractValidator<AccruedReportViewModel>
        {
            public AccruedReportViewModelValidator()
            {
                ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
                AccruedReportViewModel model = new AccruedReportViewModel();
                //RuleFor(m => m.FormPeriodID ).NotEmpty();
            }
        }


     
    }

}