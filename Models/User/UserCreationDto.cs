using Homework.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Homework.Models.User
{
    public class UserCreationDto
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; } = null!;

        [Required]
        [Range(1900, 2010)]
        [Display(Name = "Year of Birth")]
        public int YearOfBirth { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public double Balance { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public List<RoleCheckBoxViewModel> Roles { get; set; } = null!;
    }
}