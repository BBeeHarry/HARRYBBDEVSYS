using BBDEVSYS.Content.text;
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
    [FluentValidation.Attributes.Validator(typeof(AccruedUploadViewModelValidator))]
    public class AccruedUploadViewModel:IViewModel
    {
        public AccruedUploadViewModel()
        {
            AccruedUploadItemList = new List<AccruedUploadItemViewModel>();
            AttachmentList = new List<AttachmentViewModel>();
        }
        //Authorization
        public const string RoleForManageData = "Role_MA_Accrued_Upload";
        public const string RoleForDisplayData = "Role_DS_Accrued_Upload";

        public const string ProcessCode = "AccruedUploadProcess";


        [Display(Name = "PERIOD_MONTH", ResourceType = typeof(ResourceText))]
        public Nullable<int> PERIOD_MONTH { get; set; }
        [Display(Name = "PERIOD_YEAR", ResourceType = typeof(ResourceText))]
        public Nullable<int> PERIOD_YEAR { get; set; }
        [Display(Name = "REMARK", ResourceType = typeof(ResourceText))]
        public string REMARK { get; set; }


        public string UPLOAD_BY { get; set; }
        public Nullable<System.DateTime> UPLOAD_DATE { get; set; }

        public string UploadStatus { get; set; }

        public string Message { get; set; }

        //Text
        public string AccruedTypeName { get; set; }
        public string UploadByName { get; set; }

        //ValueHelpList
        public List<AccruedUploadItemViewModel> AccruedUploadItemList { get; set; }
        public List<AttachmentViewModel> AttachmentList { get; set; }

        //Screen 
        public virtual bool UserSuperAdmin { get; set; }
        public virtual string FileName { get; set; }
    }


    public class AccruedUploadViewModelValidator : AbstractValidator<AccruedUploadViewModel>
    {
        public AccruedUploadViewModelValidator()
        {

            ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            AccruedUploadViewModel model = new AccruedUploadViewModel();
            //UtilityService.SetRuleForStringLength<AccruedUploadViewModel, "">(this);
        }
    }
}