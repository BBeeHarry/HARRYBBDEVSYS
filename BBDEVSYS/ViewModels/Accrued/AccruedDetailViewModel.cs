using BBDEVSYS.Content.text;
using BBDEVSYS.ViewModels.Shared;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.Accrued
{
    [FluentValidation.Attributes.Validator(typeof(AccruedDetailViewModelValidator))]
    public class AccruedDetailViewModel : IViewModel
    {
        public AccruedDetailViewModel()
        {
            //AccruedList = new List<AccruedViewModel>();
            //AccruedData = new AccruedViewModel();
            AccruedItemSubList = new List<AccruedDetailSubViewModel>();
        }

        public int ACCRUED_ITEM_ID { get; set; }
        public string GL_ACCOUNT { get; set; }
        public string COST_CENTER { get; set; }
        public string COST_CENTER_FUND { get; set; }
        public string PAYMENT_ITEMS_FEE_ITEM { get; set; }
        public Nullable<int> ACCRUED_ID { get; set; }

        [Display(Name = "SEQUENCE", ResourceType = typeof(ResourceText))]
        public Nullable<int> SEQUENCE { get; set; }

        [Display(Name = "ACCRUED_MONTH", ResourceType = typeof(ResourceText))]
        public Nullable<int > ACCRUED_MONTH { get; set; }

        [Display(Name = "ACCRUED_YEAR", ResourceType = typeof(ResourceText))]
        public Nullable<int> ACCRUED_YEAR { get; set; }

        [Display(Name = "PERIOD_ACCRUED", ResourceType = typeof(ResourceText))]
        public string PERIOD_ACCRUED { get; set; }

        [Display(Name = "CCT_CODE", ResourceType = typeof(ResourceText))]
        public string CCT_CODE { get; set; }
        [Display(Name = "FUND", ResourceType = typeof(ResourceText))]
        public string FUND_CODE { get; set; }
     

        [Display(Name = "COMPANY_CODE", ResourceType = typeof(ResourceText))]
        public string COMPANY_CODE  { get;set;}

        [Display(Name = "PAYMENT_ITEMS_CODE", ResourceType = typeof(ResourceText))]
        public string PAYMENT_ITEMS_CODE { get; set; }

        [Display(Name = "TRANSACTION", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> TRANSACTIONS { get; set; }

        [Display(Name = "AMOUNT", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> AMOUNT { get; set; }

        [Display(Name = "INV_AMOUNT", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> INV_AMOUNT { get; set; }

        [Display(Name = "CURRENCY", ResourceType = typeof(ResourceText))]
        public string CURRENCY { get; set; }

        [Display(Name = "EDITION", ResourceType = typeof(ResourceText))]
        public int EDITION { get; set; }

        [Display(Name = "REMARK", ResourceType = typeof(ResourceText))]
        public string REMARK { get; set; }

        [Display(Name = "INV_NO", ResourceType = typeof(ResourceText))]
        public string INV_NO { get; set; }

        [Display(Name = "PRO_NO", ResourceType = typeof(ResourceText))]
        public string PRO_NO { get; set; }
        [Display(Name = "REMARK", ResourceType = typeof(ResourceText))]
        public string REMARK_INVOICE { get; set; }

        public Nullable<bool> ISPLAN { get; set; }

        [Display(Name = "CHANNELS", ResourceType = typeof(ResourceText))]
        public string CHANNELS { get; set; }


        [Display(Name = "FEE", ResourceType = typeof(ResourceText))]
        public string PAYMENT_ITEMS_NAME { get; set; }

        [Display(Name = "Charge", ResourceType = typeof(ResourceText))]
        public string CHARGED { get; set; }
        [Display(Name = "Supplier", ResourceType = typeof(ResourceText))]
        public string Supplier { get; set; }

        [Display(Name = "INV_MONTH", ResourceType = typeof(ResourceText))]
        public int INV_MONTH { get; set; }

        [Display(Name = "INV_YEAR", ResourceType = typeof(ResourceText))]
        public int INV_YEAR { get; set; }



        public List<AccruedDetailSubViewModel> AccruedItemSubList { get;  set; }


        //For screen control
        public string AccruedJSON { get; set; }
        public string ItemIndex { get; set; }
        public bool DeleteFlag { get; set; }
    }

    public partial class  AccruedDetailSubViewModel : IViewModel
    {
        public Nullable<int> ACCRUED_ITEM_ID { get; set; }
        public string PAYMENT_ITEMS_FEE_ITEM { get; set; }
        public Nullable<int> SEQUENCE { get; set; }
        public Nullable<decimal> TRANSACTIONS { get; set; }
        public Nullable<decimal> ACTUAL_AMOUNT { get; set; }
        public Nullable<decimal> TOTAL_CHARGE_AMOUNT { get; set; }
        public Nullable<decimal> RATE_TRANS { get; set; }
        public Nullable<decimal> RATE_AMT { get; set; }
        public string REMARK { get; set; }
        public Nullable<int> INV_MONTH { get; set; }
        public Nullable<int> INV_YEAR { get; set; }
        public string INV_NO { get; set; }
        public string PRO_NO { get; set; }
        public string CCT_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PAYMENT_ITEMS_CODE { get; set; }
        public Nullable<decimal> NET_AMOUNT { get; set; }
        public string CHANNELS { get; set; }
        public string GL_ACCOUNT { get; set; }
        public string COST_CENTER { get; set; }
        public string FUND_CODE { get; set; }
        public string PAYMENT_ITEMS_NAME { get; set; }
        public Nullable<int> ACCRUED_MONTH { get; set; }
        public Nullable<int> ACCRUED_YEAR { get; set; }
    }

    public class AccruedDetailViewModelValidator : AbstractValidator<AccruedDetailViewModel>
    {
        public AccruedDetailViewModelValidator()
        {
            //ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            //AccruedDetailViewModel model = new AccruedDetailViewModel();

            //RuleFor(m => m.IndicatorName).NotEmpty();
            //RuleFor(m => m.FormDisplayText).NotEmpty();
            //RuleFor(m => m.ReportDisplayText).NotEmpty();
            //RuleFor(m => m.ScoreType).NotEmpty();
            //RuleFor(m => m.Score).NotEmpty().When(m => string.Equals("02", m.ScoreType, StringComparison.OrdinalIgnoreCase));
            //RuleFor(m => m.FillType).NotEmpty();
            //RuleFor(m => m.FillUnit).NotEmpty().When(m => string.Equals("01", m.FillType, StringComparison.OrdinalIgnoreCase));
            ////RuleFor(m => m.CalculationClass).NotEmpty();
            //RuleFor(m => m.Status).NotEmpty();

            // UtilityService.SetRuleForStringLength<InvoiceViewModel, "model database">(this);
        }
    }
}