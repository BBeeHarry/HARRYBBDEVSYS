using BBDEVSYS.Content.text;
using BBDEVSYS.ViewModels.Shared;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.AccruedReport
{
    [FluentValidation.Attributes.Validator(typeof(AccruedSummaryReportViewModelValidator))]
    public class AccruedSummaryReportViewModel//:IViewModel
    {
        public AccruedSummaryReportViewModel()
        {
        }

        //Authorization
        public const string RoleForManageData = "";
        public const string RoleForDisplayData = "Role_RP_Accred_Summary";


        public int ItemNo { get; set; }
        public string GL_ACCOUNT { get; set; }
        public string COST_CENTER { get; set; }
        public string FUND_CODE { get; set; }
        public string PAYMENT_ITEMS_NAME { get; set; }

        public Nullable<decimal> TRANSACTIONS { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public Nullable<decimal> INV_AMOUNT { get; set; }

        public string CURRENCY { get; set; }

        public string Supplier { get; set; }
        public string REMARK { get; set; }

        public string INV_NO { get; set; }
        public string PRO_NO { get; set; }
    }
    public class AccruedSummaryReportViewModelValidator : AbstractValidator<AccruedSummaryReportViewModel>
    {
        public AccruedSummaryReportViewModelValidator()
        {
            ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            AccruedDetailReportViewModel model = new AccruedDetailReportViewModel();
            //RuleFor(m => m.FormPeriodID ).NotEmpty();
        }
    }
    
}