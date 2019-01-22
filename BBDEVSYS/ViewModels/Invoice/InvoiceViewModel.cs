
using BBDEVSYS.Content.text;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.CenterSetting.PaymentItems;
using BBDEVSYS.ViewModels.Shared;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.ViewModels.Invoice
{
    [FluentValidation.Attributes.Validator(typeof(InvoiceViewModelValidator))]
    public class InvoiceViewModel : IViewModel
    {

        public InvoiceViewModel()
        {
            MonthLst = new List<SelectListItem>();
            YearLst = new List<SelectListItem>();
            CompanyLst = new List<SelectListItem>();

            InvoiceDetailList = new List<InvoiceDetailViewModel>();
            PaymentItemList = new List<SelectListItem>();
            //InvoiceDetailItem = new InvoiceDetailViewModel();

            UserCreateList = new List<SelectListItem>();
            UserReceiveList = new List<SelectListItem>();
            UserApprovedList = new List<SelectListItem>();

            StatusList = new List<ValueHelpViewModel>();
        }

        //Authorization
        public const string RoleForManageData = "Role_MA_Invoice";
        public const string RoleForDisplayData = "Role_DS_Invoice";

        


        [Display(Name = "INV_NO", ResourceType =typeof(ResourceText))]
        public string INV_NO { get; set; }

        [Display(Name = "PRO_NO", ResourceType = typeof(ResourceText))]
        public string PRO_NO { get; set; }

        [Display(Name = "CCT_CODE", ResourceType = typeof(ResourceText))]
        public string CCT_CODE { get; set; }

        [Display(Name = "COMPANY_CODE", ResourceType = typeof(ResourceText))]
        public string COMPANY_CODE { get; set; }

        [Display(Name = "INV_MONTH", ResourceType = typeof(ResourceText))]
        public int INV_MONTH { get; set; }

        [Display(Name = "INV_YEAR", ResourceType = typeof(ResourceText))]
        public int INV_YEAR { get; set; }

        [Display(Name = "PAYMENT_ITEMS_CODE", ResourceType = typeof(ResourceText))]
        public string PAYMENT_ITEMS_CODE { get; set; }

        [Display(Name = "INV_REC_BY", ResourceType = typeof(ResourceText))]
        public string INV_REC_BY { get; set; }

        [Display(Name = "INV_REC_DATE", ResourceType = typeof(ResourceText))]
        public Nullable<System.DateTime> INV_REC_DATE { get; set; }
        [Display(Name = "PRO_REC_DATE", ResourceType = typeof(ResourceText))]
        public Nullable<System.DateTime> PRO_REC_DATE { get; set; }


        [Display(Name = "INV_APPROVED_BY", ResourceType = typeof(ResourceText))]
        public string INV_APPROVED_BY { get; set; }

        [Display(Name = "INV_APPROVED_DATE", ResourceType = typeof(ResourceText))]
        public Nullable<System.DateTime> INV_APPROVED_DATE { get;set;}

        [Display(Name = "INV_DUE_DATE", ResourceType = typeof(ResourceText))]
        public Nullable<System.DateTime> INV_DUE_DATE { get; set; }
        [Display(Name = "PRO_DUE_DATE", ResourceType = typeof(ResourceText))]
        public Nullable<System.DateTime> PRO_DUE_DATE { get; set; }


        [Display(Name = "CONDITION_PYM", ResourceType = typeof(ResourceText))]
        public string CONDITION_PYM { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(ResourceText))]
        public string STATUS { get; set; }

        [Display(Name = "NET_AMOUNT", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> NET_AMOUNT { get; set; }
        [Display(Name = "NET_CHARGE_AMOUNT", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> DEDUCT_WHT_AMOUNT { get; set; }

        [Display(Name = "CURRENCY", ResourceType = typeof(ResourceText))]
        public string CURRENCY { get; set; }
        [Display(Name = "INCLUDE_VAT_AMOUNT", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> INCLUDE_VAT_AMOUNT { get; set; }

        [Display(Name = "VAT", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> VAT { get; set; }

        [Display(Name = "WHT", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> WHT { get; set; }
        [Display(Name = "DISCOUNT", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> DISCOUNT { get; set; }

        [Display(Name = "REMARK", ResourceType = typeof(ResourceText))]
        public string REMARK { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(ResourceText))]
        public string IS_STATUS { get; set; }
        public string CHANNELS { get; set; }
        public string GL_ACCOUNT { get; set; }
        public string COST_CENTER { get; set; }
        public string FUND_CODE { get; set; }
        public string PAYMENT_ITEMS_NAME { get; set; }
        public int ITEM { get; set; }

       
        public Nullable<bool> UPLOAD_TYPE { get; set; }

        //-------entitites--------//


        public string COMPANY_NAME { get; set; }
        public string MONTH_NAME { get; set; }
        public string YEAR_NAME { get; set; }
        public string INV_REC_BY_TEXT { get; set; }

        [Display(Name = "CompanyCode", ResourceType = typeof(ResourceText))]
        public string CompanyCode { get; set; }
        [Display(Name = "Months", ResourceType = typeof(ResourceText))]
        public int MonthValue { get; set; }
        [Display(Name = "Years", ResourceType = typeof(ResourceText))]
        public int YearValue { get; set; }
        

     





        public List<SelectListItem> MonthLst { get; set; }
        public List<SelectListItem> YearLst { get; set; }
        public List<SelectListItem> CompanyLst { get; set; }

        public List<InvoiceDetailViewModel> InvoiceDetailList { get;  set; }
        //public InvoiceDetailViewModel InvoiceDetailItem { get;  set; }
        public List<SelectListItem> UserCreateList { get; set; }
        public List<SelectListItem> UserReceiveList { get;  set; }
        public List<SelectListItem> UserApprovedList { get;  set; }



        public string STATUS_NAME { get; set; }
        public string UPLOAD_TYPE_NAME { get; set; }
        public List<ValueHelpViewModel> StatusList { get;  set; }
        public List<SelectListItem> PaymentItemList { get;  set; }

        //--List
        [Display(Name = "INV_MONTH_FROM", ResourceType = typeof(ResourceText))]
        public int INV_MONTH_FROM { get; set; }
        [Display(Name ="INV_MONTH_TO",ResourceType =typeof(ResourceText))]
        public int INV_MONTH_TO { get; set; }


        [Display(Name ="INV_YEAR_FROM",ResourceType =typeof(ResourceText) )]
        public int INV_YEAR_FROM { get; set; }

        [Display(Name ="INV_YEAR_TO",ResourceType =typeof(ResourceText))]
        public int INV_YEAR_TO { get; set; }

    }

    public class InvoiceViewModelValidator : AbstractValidator<InvoiceViewModel>
    {
        public InvoiceViewModelValidator()
        {
            ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            InvoiceViewModel model = new InvoiceViewModel();

            RuleFor(m => m.COMPANY_CODE).NotEmpty(); 
            RuleFor(m => m.INV_MONTH).NotEmpty(); 
            RuleFor(m => m.INV_YEAR).NotEmpty();

            RuleFor(m => m.INV_NO).NotEmpty();//.When(m => !m.IS_STATUS.Equals("4"));
            //RuleFor(m => m.INV_NO).NotEmpty().When(m => !m.IS_STATUS.EndsWith("4"));
            //RuleFor(m => m.INV_NO).Must(m => (model.UPLOAD_TYPE ?? false ) == false|| !model.IS_STATUS.Equals(4) ).NotEmpty();

            //---waitning test UPload Invoice
            // RuleFor(m => m.PRO_NO).Must(m => (model.UPLOAD_TYPE ?? false) == false).NotEmpty();
            RuleFor(m => m.IS_STATUS).Must(m => (model.UPLOAD_TYPE ?? false) == false ).NotEmpty();

            RuleFor(m => m.VAT).Must(m => (model.UPLOAD_TYPE ?? false) == false ).NotEmpty();
            RuleFor(m => m.WHT).Must(m => (model.UPLOAD_TYPE ?? false) == false ).NotEmpty();


            RuleFor(m => m.INV_REC_DATE).Must(m => (model.UPLOAD_TYPE ?? false) == false ).NotEmpty();
            RuleFor(m => m.INV_DUE_DATE).Must(m => (model.UPLOAD_TYPE ?? false) == false ).NotEmpty();

            //Check List 
           

            //RuleFor(m => m.INV_MONTH_FROM).InclusiveBetween(1, 12)
            //    .LessThanOrEqualTo(m => m.INV_MONTH_TO).When(m=>m.INV_YEAR_FROM==m.INV_YEAR_TO).WithLocalizedMessage(typeof(ValidatorMessage), "lessthanandequal_error", UtilityService.GetDisplayName(model, m => m.INV_MONTH_TO));

            //RuleFor(m => m.INV_MONTH_TO).InclusiveBetween(1, 12);

            //RuleFor(m => m.INV_MONTH_FROM).Must(m => model.INV_MONTH_TO != null).NotEmpty();
            //RuleFor(m => m.INV_MONTH_TO).Must(m => model.INV_MONTH_FROM != null).NotEmpty();



            //RuleFor(m => m.INV_YEAR_FROM).LessThanOrEqualTo(m => m.INV_YEAR_TO).WithLocalizedMessage(typeof(ValidatorMessage), "lessthanandequal_error", UtilityService.GetDisplayName(model, m => m.INV_YEAR_TO));


            //RuleFor(m => m.INV_YEAR_FROM).Must(m => model.INV_YEAR_TO != null).NotEmpty();
            //RuleFor(m => m.INV_YEAR_TO).Must(m => model.INV_YEAR_FROM != null).NotEmpty();




            //RuleFor(m => m.StartDate)
            //    .NotEmpty()
            //    .LessThan(m => m.EndDate)
            //    .WithLocalizedMessage(typeof(ValidatorMessage), "lessthan_error", UtilityService.GetDisplayName(model, m => m.EndDate));

            //RuleFor(m => m.Score).NotEmpty().When(m => string.Equals("02", m.ScoreType, StringComparison.OrdinalIgnoreCase));
            //RuleFor(m => m.FillType).NotEmpty();
            //RuleFor(m => m.FillUnit).NotEmpty().When(m => string.Equals("01", m.FillType, StringComparison.OrdinalIgnoreCase));
            ////RuleFor(m => m.CalculationClass).NotEmpty();
            //RuleFor(m => m.Status).NotEmpty();

            UtilityService.SetRuleForStringLength<InvoiceViewModel, FEE_INVOICE>(this);
        }
    }

}