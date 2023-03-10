using Homework.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Homework.Utils
{
    public static partial class IdentityExtensions
    {
        public static async Task<IdentityResult> RegisterCustomerAsync(this UserManager<User> userManager, User user, string password) 
        {
            return await userManager.RegisterUserWithRoleAsync(user, password, "Customer");
        }

        public static async Task<IdentityResult> RegisterAdminAsync(this UserManager<User> userManager, User user, string password)
        {
            return await userManager.RegisterUserWithRoleAsync(user, password, "Admin");
        }

        private static async Task<IdentityResult> RegisterUserWithRoleAsync(this UserManager<User> userManager, User user, string password, string role)
        {
            IdentityResult registrationResult = await userManager.CreateAsync(user, password);
            if (!registrationResult.Succeeded)
                return registrationResult;

            return await userManager.AddToRoleAsync(user, role);
        }
    }
}