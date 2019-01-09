using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using FluentValidation;
using BBDEVSYS.Content.text;
using BBDEVSYS.ViewModels.Shared;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.Services.Shared;

namespace BBDEVSYS.ViewModels.Authorization
{
    [FluentValidation.Attributes.Validator(typeof(AppCompositeRoleViewModelValidator))]
    public class AppCompositeRoleViewModel: IViewModel
    {
        public AppCompositeRoleViewModel()
        {
            AppMenuItems = new List<AppMenuViewModel>();
            StatusValueHelp = new List<ValueHelpViewModel>();
        }

        //Authorization
        public const string RoleForManageData = "Role_MA_App_Composite_Role";
        public const string RoleForDisplayData = "Role_DS_App_Composite_Role";

        [Display(Name = "ACRName", ResourceType = typeof(ResourceText))]
        public string Name { get; set; }
        [Display(Name = "ACEDesc", ResourceType = typeof(ResourceText))]
        public string Description { get; set; }
        [Display(Name = "STATUS", ResourceType = typeof(ResourceText))]
        public string Status { get; set; }

        public bool testcheckbox { get; set; }

        public string StatusText { get; set; }

        public string AppMenuSelectedJSON { get; set; }
        public List<AppMenuViewModel> AppMenuItems { get; set; }

        //Value Help
        public List<ValueHelpViewModel> StatusValueHelp; 
    }

    //Validator
    public class AppCompositeRoleViewModelValidator : AbstractValidator<AppCompositeRoleViewModel>
    {

        public AppCompositeRoleViewModelValidator()
        {
            ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            AppCompositeRoleViewModel model = new AppCompositeRoleViewModel();
            
            RuleFor(m => m.Name).NotEmpty();
            RuleFor(m => m.Description).NotEmpty();
            RuleFor(m => m.Status).NotEmpty();

            UtilityService.SetRuleForStringLength<AppCompositeRoleViewModel, AppCompositeRole>(this);

        }

    }
}