using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Homework.Data.Entities
{
    public class User : IdentityUser
    {
        [Display(Name = "Year of Birth")]
        public int YearOfBirth { get; set; }
        public double Balance { get; set; }
    }
}