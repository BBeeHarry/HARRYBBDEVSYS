using BBDEVSYS.Content.text;
using BBDEVSYS.ViewModels.Shared;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.ViewModels.Accrued
{
    [FluentValidation.Attributes.Validator(typeof(AccruedViewModelValidator))]
    public class AccruedViewModel : IViewModel
    {
        public AccruedViewModel()
        {
            AccruedItemList = new List<AccruedDetailViewModel>();
            AccruedItem = new AccruedDetailViewModel();

            MonthLst = new List<SelectListItem>();
            YearLst = new List<SelectListItem>();
            CompanyLst = new List<SelectListItem>();
            UserApprovedList = new List<SelectListItem>();
        }
        //Authorization
        public const string RoleForManageData = "Role_MA_Accrued";
        public const string RoleForDisplayData = "Role_DS_Accrued";


        public int ItemNo { get; set; }


        public int ACCRUED_ID { get; set; }

        [Display(Name = "PERIOD_MONTH", ResourceType = typeof(ResourceText))]
        public int PERIOD_MONTH { get; set; }
       

        [Display(Name = "PERIOD_YEAR", ResourceType = typeof(ResourceText))]
        public int PERIOD_YEAR { get; set; }



        [Display(Name = "COMPANY_CODE", ResourceType = typeof(ResourceText))]
        public string COMPANY_CODE { get; set; }
       
        [Display(Name = "SUM_AMT", ResourceType = typeof(ResourceText))]
        public decimal TOTAL_AMT { get; set; }

        [Display(Name = "INV_APPROVED_BY", ResourceType = typeof(ResourceText))]
        public string APPROVED_BY { get; set; }

        [Display(Name = "INV_APPROVED_DATE", ResourceType = typeof(ResourceText))]
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }

        [Display(Name = "REMARK", ResourceType = typeof(ResourceText))]
        public string REMARK { get; set; }

        public Nullable<bool> UPLOAD_TYPE { get; set; }
        public List<AccruedDetailViewModel> AccruedItemList { get; set; }
        public AccruedDetailViewModel AccruedItem { get; set; }

        //Screen List
        public string POSITION_NAME { get; set; }
        public string APPROVED_BY_NAME { get; set; }
        public string CREATE_BY_NAME { get; set; }
        public string PERIOD_MONTH_NAME { get; set; }
        public string COMPANY_CODE_NAME { get; set; }



        public List<SelectListItem> MonthLst { get; set; }
        public List<SelectListItem> YearLst { get; set; }
        public List<SelectListItem> CompanyLst { get; set; }
        public List<SelectListItem> UserApprovedList { get; set; }
        #region generate item
        //public string PERIOD_ACCRUED { get; set; }
        //public Nullable<decimal> INV_AMOUNT { get; set; }
        //public Nullable<decimal> TRANSACTIONS { get; set; }
        //public Nullable<decimal> AMOUNT { get; set; }
        //public string CURRENCY { get; set; }
        //public int EDITION { get; set; }
        //public string REMARKS { get; set; }
        #endregion
    }

    public class AccruedViewModelValidator : AbstractValidator<AccruedViewModel>
    {

        public AccruedViewModelValidator()
        {
            ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            AccruedViewModel model = new AccruedViewModel();

            RuleFor(m => m.COMPANY_CODE).NotEmpty();
            RuleFor(m => m.PERIOD_MONTH).NotEmpty();
            RuleFor(m => m.PERIOD_YEAR).NotEmpty();
            RuleFor(m => m.APPROVED_BY).NotEmpty();
            RuleFor(m => m.APPROVED_DATE).NotEmpty();
            //RuleFor(m => m.Score).NotEmpty().When(m => string.Equals("02", m.ScoreType, StringComparison.OrdinalIgnoreCase));
            //RuleFor(m => m.FillType).NotEmpty();
            //RuleFor(m => m.FillUnit).NotEmpty().When(m => string.Equals("01", m.FillType, StringComparison.OrdinalIgnoreCase));
            ////RuleFor(m => m.CalculationClass).NotEmpty();
            //RuleFor(m => m.Status).NotEmpty();

            // UtilityService.SetRuleForStringLength<InvoiceViewModel, "model database">(this);
        }
    }
}