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

namespace BBDEVSYS.ViewModels.AccruedUpload
{
    [FluentValidation.Attributes.Validator(typeof(AccruedUploadItemViewModelValidator))]
    public class AccruedUploadItemViewModel : IViewModel
    {

        public AccruedUploadItemViewModel()
        {
            List<AccruedUploadItemSubViewModel> accruedItemSubList = new List<AccruedUploadItemSubViewModel>();
        }

        [Display(Name = "ID", ResourceType = typeof(ResourceText))]
        public int ACCRUED_UPLOAD_ID { get; set; }
        public Nullable<int> PERIOD_MONTH { get; set; }
        public Nullable<int> PERIOD_YEAR { get; set; }
        public string COMPANY_CODE { get; set; }
        public Nullable<decimal> TOTAL_AMT { get; set; }
        public string APPROVED_BY { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }
        public string INV_NO { get; set; }
        public string PRO_NO { get; set; }
        public Nullable<int> SEQUENCE { get; set; }
        public Nullable<int> ACCRUED_MONTH { get; set; }
        public Nullable<int> ACCRUED_YEAR { get; set; }
        public string PERIOD_ACCRUED { get; set; }
        public string CCT_CODE { get; set; }
        public string PAYMENT_ITEMS_CODE { get; set; }
        public Nullable<decimal> TRANSACTIONS { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public Nullable<decimal> INV_AMOUNT { get; set; }
        public string CURRENCY { get; set; }
        public string REMARK_INVOICE { get; set; }
        public Nullable<bool> ISPLAN { get; set; }
        public Nullable<int> EDITION { get; set; }
        public string REMARK { get; set; }


        public bool DeleteFlag { get; set; }
        public string ItemIndex { get; set; }

        public string Message { get; set; }
        public string MessageAt { get; set; }
        public bool ErrorFlag { get; set; }


        public List<AccruedUploadItemSubViewModel> accruedItemSubList { get; set; }


    }

    public partial class AccruedUploadItemSubViewModel : IViewModel
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
    public class AccruedUploadItemViewModelValidator : AbstractValidator<AccruedUploadItemViewModel>
    {
        public AccruedUploadItemViewModelValidator()
        {
            ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            AccruedUploadItemViewModel model = new AccruedUploadItemViewModel();



            UtilityService.SetRuleForStringLength<AccruedUploadItemViewModel, FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG>(this);
        }
    }
}
