using BBDEVSYS.Content.text;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.Shared;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.ViewModels.Adjustrefund
{
    [FluentValidation.Attributes.Validator(typeof(AdjustrefundUploadViewModelValidator))]
    public class AdjustrefundUploadViewModel : IViewModel
    {
        public AdjustrefundUploadViewModel()
        {

            AttachmentList = new List<AttachmentViewModel>();
            adjList = new List<AdjustrefundUploadViewModelItems>();
            adjDataTable = new DataTable();
            UserRequestList = new List<ValueHelpViewModel>();
            

        }
        //Authorization
        public const string RoleForManageData = "Role_MA_Adjust_Refund_Role";
        public const string RoleForDisplayData = "Role_DS_Adjust_Refund_Role";

        public const string ProcessCode = "AdjustrefundUploadProcess";

        [DisplayName("Sheet")]
        public string AttachSheet { get; set; }


        [DisplayName("User Id")]
        public string UserRequest { get; set; }




        //[Required(ErrorMessage = "Please select file.")]

        //[FileExt(Allow = ".xls,.xlsx", ErrorMessage = "Only excel file")]
        [DisplayName("Browse File")]
        public HttpPostedFileBase[] files { get; set; }
        

        public List<AttachmentViewModel> AttachmentList { get; set; }
        public List<AdjustrefundUploadViewModelItems> adjList { get;  set; }
        public DataTable adjDataTable { get;  set; }

        public  string NameFormView { get; set; }
        public List<ValueHelpViewModel> UserRequestList { get;  set; }
    }
    public class AdjustrefundUploadViewModelItems {
        #region Data Field 
        public string SR_STATUS { get; set; }
        public Nullable<System.DateTime> SR_OPEN_DATE { get; set; }
        public string SR_NO { get; set; }
        public string CATEGORY { get; set; }
        public string SUB_CATEGORY { get; set; }
        public string ISSUE { get; set; }
        public string BAN_1 { get; set; }
        public string PRIM_RESOURCE { get; set; }
        public string SR_DETAILS { get; set; }
        public string SR_DIVISION { get; set; }
        public string SR_OWNER { get; set; }
        public string BAN_INCORRECT { get; set; }
        public string BAN_2 { get; set; }
        public Nullable<decimal> AR_BALANCE_1 { get; set; }
        public string BAN_1_1 { get; set; }
        public string CUSTOMER_NAME_1 { get; set; }
        public string ACCOUNT_TYPE_1 { get; set; }
        public string COMP_CODE_1 { get; set; }
        public string AR_BALANCE_1_1 { get; set; }
        public string BEN_STATUS_1 { get; set; }
        public string IDENT_1 { get; set; }
        public string CONV_IND_1 { get; set; }
        public string CONV_CODE_1 { get; set; }
        public string T_FORM_ACCOUNT_BC_ID { get; set; }
        public string BAN_2_1 { get; set; }
        public string CUSTOMER_NAME_2 { get; set; }
        public string ACCOUNT_TYPE_2 { get; set; }
        public string COMP_CODE_2 { get; set; }
        public Nullable<decimal> AR_BALANCE_2 { get; set; }
        public string BEN_STATUS_2 { get; set; }
        public string IDENT_2 { get; set; }
        public string CONV_IND_2 { get; set; }
        public string CONV_CODE_2 { get; set; }
        public string T_TO_ACCOUNT_BC_ID { get; set; }
        public string RECEIPT_NO { get; set; }
        public Nullable<decimal> PAY_AMOUNT { get; set; }
        public Nullable<System.DateTime> DEPOSIT_DATE { get; set; }
        public string SOURCE_ID { get; set; }
        public string DOC_BILL_TYPE { get; set; }
        public string FILE_NAME { get; set; }

        #endregion
    }
    //public class FileExt : ValidationAttribute
    //{
    //    public string Allow;
    //    protected  ValidationResult IsValid(object value, FluentValidation.ValidationContext validationContext)
    //    {
    //        if (value != null)
    //        {
    //            string extension = ((System.Web.HttpPostedFileBase)value).FileName.Split('.')[1];
    //            if (Allow.Contains(extension))
    //                return ValidationResult.Success;
    //            else
    //                return new ValidationResult(ErrorMessage);
    //        }
    //        else
    //            return ValidationResult.Success;
    //    }
    //}
    public class AdjustrefundUploadViewModelValidator : AbstractValidator<AdjustrefundUploadViewModel>
    {
        public AdjustrefundUploadViewModelValidator()
        {

            ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            //InvoiceUploadViewModel model = new InvoiceUploadViewModel();
            //UtilityService.SetRuleForStringLength<InvoiceUploadViewModel, "">(this);
        }
    }
}