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

namespace BBDEVSYS.ViewModels.Adjustrefund
{
    [FluentValidation.Attributes.Validator(typeof(AdjustrefundUploadViewModelValidator))]
    public class AdjustrefundUploadViewModel : IViewModel
    {
        public AdjustrefundUploadViewModel()
        {
           
            AttachmentList = new List<AttachmentViewModel>();

          
        }
        //Authorization
        public const string RoleForManageData = "Role_MA_Invoice_Upload";
        public const string RoleForDisplayData = "Role_DS_Invoice_Upload";

        public const string ProcessCode = "InvoiceUploadProcess";
        
        [Display(Name = "INV_MONTH", ResourceType = typeof(ResourceText))]
        public Nullable<int> INV_MONTH { get; set; }
      
        public string UPLOAD_BY { get; set; }
        public Nullable<System.DateTime> UPLOAD_DATE { get; set; }

        public string UploadStatus { get; set; }

        public string Message { get; set; }

        //Text
        public string InvoiceTypeName { get; set; }
        public string UploadByName { get; set; }

      
        //Screen 
        public virtual bool UserSuperAdmin { get; set; }
        public virtual string FileName { get; set; }
        public List<AttachmentViewModel> AttachmentList { get;  set; }
    }

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