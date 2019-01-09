using BBDEVSYS.Content.text;
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
    [FluentValidation.Attributes.Validator(typeof(InvoiceDetailViewModelValidator))]
    public class InvoiceDetailViewModel:IViewModel
    {
        public InvoiceDetailViewModel()
        {
            PaymentItemsFeeItemList = new List<SelectListItem>();
        }
        
        [Display(Name = "INV_NO", ResourceType =typeof(ResourceText))]
        public string INV_NO { get; set; }

        [Display(Name = "PAYMENT_ITEMS_FEE_ITEM", ResourceType = typeof(ResourceText))]
        public string PAYMENT_ITEMS_FEE_ITEM { get; set; }

        public string PAYMENT_ITEMS_FEE_ITEM_Code { get; set; }
        public string PAYMENT_ITEMS_FEE_ITEM_dis { get; set; }

        [Display(Name = "INV_ITEM_NO", ResourceType = typeof(ResourceText))]
        public string INV_ITEM_NO { get; set; }

        [Display(Name = "SEQUENCE", ResourceType = typeof(ResourceText))]
        public int SEQUENCE { get; set; }

        [Display(Name = "TRANSACTION", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> TRANSACTIONS { get; set; }

        [Display(Name = "ACTUAL_AMOUNT", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> ACTUAL_AMOUNT { get; set; }

        

        [Display(Name = "TOTAL_CHARGE_AMOUNT", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> TOTAL_CHARGE_AMOUNT { get; set; }

        

        [Display(Name = "RATE_TRANS", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> RATE_TRANS { get; set; }

        [Display(Name = "RATE_AMT", ResourceType = typeof(ResourceText))]
        public Nullable<decimal> RATE_AMT { get; set; }

        [Display(Name = "REMARK", ResourceType = typeof(ResourceText))]
        public string REMARK { get; set; }

        public Nullable<int> INV_MONTH { get; set; }
        public Nullable<int> INV_YEAR { get; set; }

       
        public string COMPANY_CODE { get; set; }
        public string PAYMENT_ITEMS_CODE { get; set; }
        public string CHANNELS { get; set; }
        public string GL_ACCOUNT { get; set; }
        public string COST_CENTER { get; set; }
        public string FUND_CODE { get; set; }
        public string PAYMENT_ITEMS_NAME { get; set; }




        public bool DeleteFlag { get; set; }
        public List<SelectListItem> PaymentItemsFeeItemList { get;  set; }
    }

    public class InvoiceDetailViewModelValidator :AbstractValidator<InvoiceDetailViewModel>
    {
        public InvoiceDetailViewModelValidator()
        {
            //ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            //InvoiceDetailViewModel model = new InvoiceDetailViewModel();
        }
    }
}