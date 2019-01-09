using BBDEVSYS.Content.text;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.Shared;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.ViewModels.InvoiceUpload
{
    [FluentValidation.Attributes.Validator(typeof(InvoiceUploadViewModelValidator))]
    public class InvoiceUploadViewModel:IViewModel
    {
        public InvoiceUploadViewModel()
        {
            InvoiceUploadItemList = new List<InvoiceUploadItemViewModel>();
            AttachmentList = new List<AttachmentViewModel>();

            MonthLst = new List<SelectListItem>();
            YearLst = new List<SelectListItem>();
            CompanyLst = new List<SelectListItem>();
            UserApprovedList = new List<SelectListItem>();
        }
        //Authorization
        public const string RoleForManageData = "Role_MA_Invoice_Upload";
        public const string RoleForDisplayData = "Role_DS_Invoice_Upload";

        public const string ProcessCode = "InvoiceUploadProcess";
        
        [Display(Name = "INV_MONTH", ResourceType = typeof(ResourceText))]
        public Nullable<int> INV_MONTH { get; set; }
        [Display(Name = "INV_YEAR", ResourceType = typeof(ResourceText))]
        public Nullable<int> INV_YEAR { get; set; }
        [Display(Name = "INV_YEAR", ResourceType = typeof(ResourceText))]
        public string COMPANY_CODE { get; set; }
        [Display(Name = "REMARK", ResourceType = typeof(ResourceText))]
        public string REMARK { get; set; }
        public string CompanyName { get; set; }
        public string MonthName { get; set; }

        public string UPLOAD_BY { get; set; }
        public Nullable<System.DateTime> UPLOAD_DATE { get; set; }

        public string UploadStatus { get; set; }

        public string Message { get; set; }

        //Text
        public string InvoiceTypeName { get; set; }
        public string UploadByName { get; set; }

        //ValueHelpList
        public List<InvoiceUploadItemViewModel> InvoiceUploadItemList { get; set; }
        public List<AttachmentViewModel> AttachmentList { get; set; }
        public List<SelectListItem> MonthLst { get; set; }
        public List<SelectListItem> YearLst { get; set; }
        public List<SelectListItem> CompanyLst { get; set; }
        public List<SelectListItem> UserApprovedList { get; set; }
        //Screen 
        public virtual bool UserSuperAdmin { get; set; }
        public virtual string FileName { get; set; }
    }

    public class InvoiceUploadViewModelValidator : AbstractValidator<InvoiceUploadViewModel>
    {
        public InvoiceUploadViewModelValidator()
        {

            ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            InvoiceUploadViewModel model = new InvoiceUploadViewModel();
            //UtilityService.SetRuleForStringLength<InvoiceUploadViewModel, "">(this);
        }
    }
}