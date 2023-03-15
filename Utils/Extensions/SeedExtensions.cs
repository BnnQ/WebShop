using Homework.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Homework.Utils.Extensions
{
    public static class SeedExtensions
    {
        private const string UserId = "b74ddd14-6340-4840-95c2-db12554843e5";
        private const string AdminRoleId = "fab4fac1-c546-41de-aebc-a14da6895711";
        private const string CustomerRoleId = "c7b013f0-5201-4317-abd8-c211f91b7330";

        public static ModelBuilder SeedUsers(this ModelBuilder builder)
        {
            var user = new User
            {
                Id = UserId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                LockoutEnabled = false,
                PhoneNumber = "+380687507133"
            };

            PasswordHasher<User> passwordHasher = new();
            user.PasswordHash = passwordHasher.HashPassword(user, "1111");

            builder.Entity<User>().HasData(user);
            return builder;
        }

        public static ModelBuilder SeedRoles(this ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = AdminRoleId, Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
                new IdentityRole { Id = CustomerRoleId, Name = "Customer", ConcurrencyStamp = "2", NormalizedName = "CUSTOMER" }
                );

            return builder;
        }

        public static ModelBuilder SeedUserRoles(this ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { RoleId = AdminRoleId, UserId = UserId }
                );

            return builder;
        }
    }
}