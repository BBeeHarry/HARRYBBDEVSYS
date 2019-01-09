
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

        //  public Nullable<bool> ISPLAN { get; set; }







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