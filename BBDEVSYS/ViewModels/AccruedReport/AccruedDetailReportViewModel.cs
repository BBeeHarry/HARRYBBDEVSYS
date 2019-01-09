
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

    [FluentValidation.Attributes.Validator(typeof(AccruedDetailReportViewModelValidator))]
    public class AccruedDetailReportViewModel : IViewModel
    {
        public AccruedDetailReportViewModel()
        {
            MonthLst = new List<SelectListItem>();
            YearLst = new List<SelectListItem>();
            CompanyLst = new List<SelectListItem>();
            BusinessUnitLst = new List<SelectListItem>();
            feeTypeLst = new List<SelectListItem>();
            channelsList = new List<SelectListItem>();
        }
        //Authorization
        public const string RoleForManageData = "";
        public const string RoleForDisplayData = "Role_RP_Accred_Detail";


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


        public string Total_AMT { get; set; }

        public string CHARGE_GROUP_MDR { get; set; }
        public string CHARGE_GROUP_TRAN { get; set; }

        [Display(Name = "CompanyCode", ResourceType = typeof(ResourceText))]
        public string COMPANY_CODE { get; set; }
        [Display(Name = "BUSINESS_UNIT", ResourceType = typeof(ResourceText))]
        public string BUSINESS_UNIT { get; set; }
        public string CompanyCode { get; set; }
        [Display(Name = "Months", ResourceType = typeof(ResourceText))]
        public int MonthValue { get; set; }

        public int START_MONTH { get; set; }
        public int END_MONTH { get; set; }
        [Display(Name = "Years", ResourceType = typeof(ResourceText))]
        public int YearValue { get; set; }
        public int START_YEAR { get; set; }
        public int END_YEAR { get; set; }


        [Display(Name = "FEE_TYPE", ResourceType = typeof(ResourceText))]
        public string FEE_TYPE { get; set; }

        [Display(Name = "CHANNELS", ResourceType = typeof(ResourceText))]
        public string CHANNELSValue { get; set; } 



        public List<SelectListItem> MonthLst { get; set; }
        public List<SelectListItem> YearLst { get; set; }
        public List<SelectListItem> CompanyLst { get; set; }
        public List<SelectListItem> feeTypeLst { get;  set; }
        public List<SelectListItem> channelsList { get;  set; }
        public List<SelectListItem> BusinessUnitLst { get;  set; }

        public class AccruedDetailReportViewModelValidator : AbstractValidator<AccruedDetailReportViewModel>
        {
            public AccruedDetailReportViewModelValidator()
            {
                ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
                AccruedDetailReportViewModel model = new AccruedDetailReportViewModel();
                //RuleFor(m => m.FormPeriodID ).NotEmpty();
            }
        }
    }
}