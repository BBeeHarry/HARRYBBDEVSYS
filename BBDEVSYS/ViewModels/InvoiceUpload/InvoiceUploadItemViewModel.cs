using BBDEVSYS.Content.text;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.Shared;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.InvoiceUpload
{
    [FluentValidation.Attributes.Validator(typeof(InvoiceUploadItemViewModelValidator))]
    public class InvoiceUploadItemViewModel : IViewModel
    {

        public InvoiceUploadItemViewModel() { }
        
        public int INV_UPLOAD_ID { get; set; }
        public string INV_NO { get; set; }
        public string PRO_NO { get; set; }
        public string CCT_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        //public Nullable<int> INV_MONTH { get; set; }
        //public Nullable<int> INV_YEAR { get; set; }

        public string INV_MONTH { get; set; }
        public string INV_YEAR { get; set; }

        public string PAYMENT_ITEMS_CODE { get; set; }
        public string INV_REC_BY { get; set; }
        //public Nullable<System.DateTime> INV_REC_DATE { get; set; }
        public string INV_REC_DATE { get; set; }

        public string PRO_REC_DATE { get; set; }

        public string INV_APPROVED_BY { get; set; }

        //public Nullable<System.DateTime> INV_APPROVED_DATE { get; set; }
        //public Nullable<System.DateTime> INV_DUE_DATE { get; set; }
        public string INV_APPROVED_DATE { get; set; }
        public string INV_DUE_DATE { get; set; }
        public string PRO_DUE_DATE { get; set; }

        public string IS_STATUS { get; set; }
        public string CONDITION_PYM { get; set; }
        //public Nullable<decimal> NET_AMOUNT { get; set; }
        //public Nullable<decimal> WHT { get; set; }
        //public Nullable<decimal> DEDUCT_WHT_AMOUNT { get; set; }

        //public Nullable<decimal> INCLUDE_VAT_AMOUNT { get; set; }
        //public Nullable<decimal> VAT { get; set; }
        //public Nullable<System.DateTime> PRO_REC_DATE { get; set; }

        //public Nullable<decimal> TRANSACTIONS { get; set; }
        //public Nullable<decimal> ACTUAL_AMOUNT { get; set; }
        //public Nullable<decimal> RATE_TRANS { get; set; }
        //public Nullable<decimal> RATE_AMT { get; set; }
        //public Nullable<decimal> TOTAL_CHARGE_AMOUNT { get; set; }

        public string NET_AMOUNT { get; set; }
        public string WHT { get; set; }
        public string DISCOUNT { get; set; }
        public string DEDUCT_WHT_AMOUNT { get; set; }

        public string INCLUDE_VAT_AMOUNT { get; set; }
        public string VAT { get; set; }

        public string TRANSACTIONS { get; set; }
        public string ACTUAL_AMOUNT { get; set; }
        public string RATE_TRANS { get; set; }
        public string RATE_AMT { get; set; }
        public string TOTAL_CHARGE_AMOUNT { get; set; }

        public string REMARK { get; set; }
        public string CURRENCY { get; set; }
        public string PAYMENT_ITEMS_FEE_ITEM { get; set; }
        public Nullable<int> SEQUENCE { get; set; }


        public string FEE { get; set; }
        public string CHARGED { get; set; }
        public string CHANNELS { get; set; }

        public string GL_ACCOUNT { get; set; }
        public string FUND_CODE { get; set; }

        public bool DeleteFlag { get; set; }
        public string ItemIndex { get; set; }

        public string Message { get; set; }
        public string MessageAt { get; set; }
        public bool ErrorFlag { get; set; }


    }
    public class InvoiceUploadItemViewModelValidator : AbstractValidator<InvoiceUploadItemViewModel>
    {
        public InvoiceUploadItemViewModelValidator()
        {
            ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            InvoiceUploadItemViewModel model = new InvoiceUploadItemViewModel();



            UtilityService.SetRuleForStringLength<InvoiceUploadItemViewModel, FEE_INVOICE_UPLOAD_ITEM_LOG>(this);
        }
    }
}