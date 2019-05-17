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
using System.Web.Mvc;

namespace BBDEVSYS.ViewModels.CenterSetting.PaymentItems
{
    [FluentValidation.Attributes.Validator(typeof(PaymentItemsViewModelValidator))]

    public class PaymentItemsViewModel:IViewModel

    {
        public PaymentItemsViewModel()
        {
            CompanyLst = new List<SelectListItem>();
            pymItemsChargeList = new List<PaymentItemsChargeViewModel>();
            ChannelsList = new List<ValueHelpViewModel>();
            DurationList = new List<ValueHelpViewModel>();
            IsActionList = new List<ValueHelpViewModel>();

        }
        //Authorization
        public const string RoleForManageData = "Role_MA_PaymentItems";
        public const string RoleForDisplayData = "Role_DS_PaymentItems";
        [Display(Name = "PAYMENT_ITEMS_CODE", ResourceType = typeof(ResourceText))]
        public string PAYMENT_ITEMS_CODE { get; set; }
        [Display(Name = "PAYMENT_ITEMS_NAME", ResourceType = typeof(ResourceText))]
        public string PAYMENT_ITEMS_NAME { get; set; }
        [Display(Name = "COMPANY_CODE", ResourceType = typeof(ResourceText))]
        public string COMPANY_CODE { get; set; }
        public string LASTMODIFIED_BY { get; set; }
        public Nullable<System.DateTime> LASTMODIFIED_DATE { get; set; }
        [Display(Name = "REMARK", ResourceType = typeof(ResourceText))]
        public string REMARK { get; set; }
        [Display(Name = "Supplier", ResourceType = typeof(ResourceText))]
        public string Supplier { get; set; }
        public string CCT_CODE { get; set; }
        [Display(Name = "CHANNELS", ResourceType = typeof(ResourceText))]
        public string CHANNELS { get; set; }
        [Display(Name = "GLAccount", ResourceType = typeof(ResourceText))]
        public string GL_ACCOUNT { get; set; }
        [Display(Name = "CostCenter", ResourceType = typeof(ResourceText))]
        public string COST_CENTER { get; set; }
        [Display(Name = "FUND", ResourceType = typeof(ResourceText))]
        public string FUND_CODE { get; set; }
        [Display(Name = "DURATION", ResourceType = typeof(ResourceText))]
        public string DURATION { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(ResourceText))]
        public Nullable<bool> IS_ACTIVE { get; set; }

        //Screen List
        public int ITEM { get; set; }
        public string  LASTMODIFIED_BY_NAME { get; set; }
        public string COMPANY_NAME { get; set; }

        public Nullable<int> GROUP_SEQ_CHANNELS { get; set; }
        public Nullable<int> SEQ_CHANNELS { get; set; }

        public List<PaymentItemsChargeViewModel> pymItemsChargeList { get; set; }
        public List<SelectListItem> CompanyLst { get;  set; }
        public List<ValueHelpViewModel> ChannelsList { get;  set; }
        public List<ValueHelpViewModel> DurationList { get;  set; }
        public List<ValueHelpViewModel> IsActionList { get;  set; }
    }
    public partial class PaymentItemsChargeViewModel:IViewModel
    {
        public PaymentItemsChargeViewModel() {
            ChargeTypeList = new List<ValueHelpViewModel>();
            IsActionList = new List<ValueHelpViewModel>();
        }

        public Nullable<int> PAYMENT_ITEMS_ID { get; set; }
        public int SEQUENCE { get; set; }
        public string PAYMENT_ITEMS_FEE_NAME { get; set; }
        public string CHARGE_TYPE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PAYMENT_ITEMS_NAME { get; set; }

        //For screen control
        public string AccruedJSON { get; set; }
        public string ItemIndex { get; set; }
        public bool DeleteFlag { get; set; }
        public List<ValueHelpViewModel> ChargeTypeList { get;  set; }

        public Nullable<bool> IS_ACTIVE { get; set; }
        public List<ValueHelpViewModel> IsActionList { get;  set; }
    }

    public  class PaymentItemsViewModelValidator : AbstractValidator<PaymentItemsViewModel>
    {
        public PaymentItemsViewModelValidator()
        {
            ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            PaymentItemsViewModel model = new PaymentItemsViewModel();

            RuleFor(m => m.COMPANY_CODE).NotEmpty();
            //RuleFor(m => m.PAYMENT_ITEMS_CODE).NotEmpty();
            RuleFor(m => m.PAYMENT_ITEMS_NAME).NotEmpty();

            RuleFor(m => m.GL_ACCOUNT).NotEmpty();
            RuleFor(m => m.COST_CENTER).NotEmpty();
            RuleFor(m => m.CHANNELS).NotEmpty();

            RuleFor(m => m.DURATION).NotEmpty();
            RuleFor(m => m.IS_ACTIVE).NotEmpty();


            //RuleFor(m => m.StartDate)
            //    .NotEmpty()
            //    .LessThan(m => m.EndDate)
            //    .WithLocalizedMessage(typeof(ValidatorMessage), "lessthan_error", UtilityService.GetDisplayName(model, m => m.EndDate));

            //RuleFor(m => m.Score).NotEmpty().When(m => string.Equals("02", m.ScoreType, StringComparison.OrdinalIgnoreCase));
            //RuleFor(m => m.FillType).NotEmpty();
            //RuleFor(m => m.FillUnit).NotEmpty().When(m => string.Equals("01", m.FillType, StringComparison.OrdinalIgnoreCase));
            ////RuleFor(m => m.CalculationClass).NotEmpty();
            //RuleFor(m => m.Status).NotEmpty();

            UtilityService.SetRuleForStringLength<PaymentItemsViewModel, PAYMENT_ITEMS>(this);
        }
    }
}