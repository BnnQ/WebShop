using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Homework.ViewModels.Account
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [Display(Name = "User name")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me")]
        public bool IsPersistent { get; set; }

        [BindNever]
        [ValidateNever]
        public bool IsModelValid { get; set; }

        [BindNever]
        [ValidateNever]
        public string ReturnUrl { get; set; } = "/";
    }
}