using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Homework.ViewModels.Account
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Please enter an user name.")]
        [Display(Name = "User name")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Please enter an email.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please enter your year of birth.")]
        [Range(1900, 2100, ErrorMessage = "Please enter a valid year.")]
        [Display(Name = "Year of birth")]
        public int YearOfBirth { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = null!;

        [BindNever]
        [ValidateNever]
        public bool IsModelValid { get; set; }
    }
}