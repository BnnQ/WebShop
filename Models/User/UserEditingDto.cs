using Homework.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Homework.Models.User
{
    public class UserEditingDto
    {
        public string Id { get; set; } = null!;

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = null!;

        [Required]
        [Range(1900, 2010)]
        [Display(Name = "Year of Birth")]
        public int YearOfBirth { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public double Balance { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? Password { get; set; }

        [Display(Name = "Lockout Enabled")]
        public bool LockoutEnabled { get; set; }

        public IList<RoleCheckBoxViewModel> Roles { get; set; } = null!;
    }
}