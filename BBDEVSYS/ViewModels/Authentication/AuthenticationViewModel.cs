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

namespace BBDEVSYS.ViewModels.Authentication
{
    [FluentValidation.Attributes.Validator(typeof(AuthenticationViewModelValidator))]
    public class AuthenticationViewModel
    {
        [Display(Name = "Username", ResourceType = typeof(ResourceText))]
        public string Username { get; set; }

        //[Display(Name = "Password", ResourceType = typeof(ResourceText))]
        //public string Password { get; set; }


        //[Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        //[Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string UserId { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
    }
    public class ExternalLoginConfirmationViewModel
    {
        //[Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class AuthenticationViewModelValidator : AbstractValidator<AuthenticationViewModel>
    {

        public AuthenticationViewModelValidator()
        {
            ValidatorOptions.ResourceProviderType = typeof(ValidatorMessage);
            AuthenticationViewModel model = new AuthenticationViewModel();

            RuleFor(m => m.Username).NotEmpty();
            RuleFor(m => m.Password).NotEmpty();
        }

    }
}