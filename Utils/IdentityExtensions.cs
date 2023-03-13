using Homework.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Homework.Utils
{
    public static partial class IdentityExtensions
    {
        public static async Task<IdentityResult> RegisterCustomerAsync(this UserManager<User> userManager, User user, string? password = null) 
        {
            return await userManager.RegisterUserWithRoleAsync(
                user: user,
                role: "Customer",
                password: password);
        }

        public static async Task<IdentityResult> RegisterAdminAsync(this UserManager<User> userManager, User user, string password)
        {
            return await userManager.RegisterUserWithRoleAsync(user, password, "Admin");
        }

        private static async Task<IdentityResult> RegisterUserWithRoleAsync(this UserManager<User> userManager, User user, string role, string? password = null)
        {
            IdentityResult registrationResult;
            if (!string.IsNullOrWhiteSpace(password))
            {
                registrationResult = await userManager.CreateAsync(user, password);
            }
            else
            {
                registrationResult = await userManager.CreateAsync(user);
            }

            if (!registrationResult.Succeeded)
                return registrationResult;

            return await userManager.AddToRoleAsync(user, role);
        }
    }
}