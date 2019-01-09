using BBDEVSYS.Content.text;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBDEVSYS.ViewModels.Posting
{
    [FluentValidation.Attributes.Validator(typeof(PaymentPostingViewModelValidator))]
    public class PaymentPostingViewModel
    {
        #region model payment posting timeliness
        public string TRANID { get; set; }
        public Nullable<System.DateTime> SYS_CREATION_DATE { get; set; }
        public Nullable<System.DateTime> POSTING_DATE { get; set; }
        public Nullable<System.DateTime> SYS_RECEIVE_DATE { get; set; }
        public Nullable<System.DateTime> CUSTOMER_PYM_DATE { get; set; }
        public string BAN { get; set; }
        public string ORIGINAL_BAN { get; set; }
        public string TAX_INV_NUMBER { get; set; }
        public Nullable<decimal> ORIGINAL_AMT { get; set; }
        public Nullable<decimal> ACTV_AMT { get; set; }
        public string SOURCE_ID { get; set; }
        public string SOURCE_TYPE { get; set; }
        public string DEPOSIT_BANK_CODE { get; set; }
        public string PAY_POINT_NO { get; set; }
        public string PYM_GL_CODE { get; set; }
        public string BILL_MONTH { get; set; }
        public string ACTV_CODE { get; set; }
        public string PYM_METHOD { get; set; }
        public string PYM_SUB_METHOD { get; set; }
        public string BANK_CODE { get; set; }
        public string BANK_ACCOUNT_NO { get; set; }
        public string CHECK_NO { get; set; }
        public string CF_BK_VOUCHER { get; set; }
        public string CR_CARD_NO { get; set; }
        public string Credit_Card_Type { get; set; }
        public string CR_CARD_EXP_DATE { get; set; }
        public string ISSUE_BANK { get; set; }
        public string CF_CC_VOUCHER { get; set; }
        public string BAN_COMPANY { get; set; }
        public string BILLING_NAME { get; set; }
        public string PRODUCT_ID { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string ID_ENTITY { get; set; }
        public string HOUSE_NO { get; set; }
        public string S_TAMBON { get; set; }
        public string S_AMPHUR { get; set; }
        public string S_PROVINCE { get; set; }
        public string S_ZIP_CODE { get; set; }
        public string ACCOUNT_TYPE { get; set; }
        public string ACCOUNT_SUB_TYPE { get; set; }
        public string BAN_AFFILIATED_IND { get; set; }
        public string P_MTD_BAN { get; set; }
        public string BAN_CR_NO { get; set; }
        public string BAN_STATUS { get; set; }
        public string OPERATOR_ID { get; set; }
        public string APPLICATION_ID { get; set; }
        public string BILL_CYCLE { get; set; }
        public string VOUCHER_INV { get; set; }
        public Nullable<System.DateTime> INV_DUE_DATE { get; set; }
        public Nullable<System.DateTime> INV_STATUS_DATE { get; set; }
        public string INV_STATUS { get; set; }
        public Nullable<int> CYCLE_RUN_MONTH { get; set; }
        public Nullable<int> CYCLE_RUN_YEAR { get; set; }
        public string AGING { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string AR_system { get; set; }
        public string File_Name_AR { get; set; }
        public string Customer_Type { get; set; }
        public string PAYMENT_CHANNEL { get; set; }
        public string SUB_PAYMENT_CHANNEL { get; set; }
        public Nullable<System.DateTime> WH_Date { get; set; }
        public string WH_No { get; set; }
        public string WH_Amount { get; set; }
        public string WH_Rate { get; set; }
        public string Cons_Bill_Ind { get; set; }

        #endregion

        public PaymentPostingViewModel()
        {
            MonthNameList = new List<string>();
            YearEngList = new List<int>();

            ReportNameType = new List<string>();

            pymPostingLst = new List<PaymentPostingViewModel>();
        }
        [Display(Name = "StartPaymentDate", ResourceType = typeof(ResourceText))]
        public Nullable<System.DateTime> StartDate { get; set; }
        [Display(Name = "EndPaymentDate", ResourceType = typeof(ResourceText))]
        public Nullable<System.DateTime> EndDate { get; set; }

        public List<string> MonthNameList { get; set; }
        public List<int> YearEngList { get;  set; }
        public List<string> ReportNameType { get;  set; }
        public List<PaymentPostingViewModel> pymPostingLst { get;  set; }
    }

    public class PaymentPostingViewModelValidator : AbstractValidator<PaymentPostingViewModel>
    {
        public PaymentPostingViewModelValidator()
        {
            //ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            //CALevelComparisonViewModel model = new CALevelComparisonViewModel();
            //RuleFor(m => m.FormPeriodID).NotEmpty();
        }
    }
}