using Google.Apis.PeopleService.v1;
using Homework.Authentication;
using Homework.Data;
using Homework.Data.Entities;
using Homework.Models.Claim;
using Homework.Services;
using Homework.Services.Abstractions;
using Homework.Services.MapperProfiles;
using Homework.Services.MapperProfiles.Category;
using Homework.Services.MapperProfiles.Claim;
using Homework.Services.MapperProfiles.Manufacturer;
using Homework.Services.MapperProfiles.Product;
using Homework.Services.MapperProfiles.Role;
using Homework.Services.MapperProfiles.User;
using Homework.Utils.Comparers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Homework.Utils.Extensions
{
    public static class StartupExtensions
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ShopContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ShopContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                if (!builder.Environment.IsProduction())
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 3;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                }
                else
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                }

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(builder.Environment.IsProduction() ? 60 : 1);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";

                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            });

            builder.Services.AddAuthentication()
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                options.SaveTokens = true;
                options.Scope.Add(PeopleServiceService.ScopeConstants.UserBirthdayRead);
                options.Events = new GoogleOAuthEvents();
            })
            .AddGitHub(options =>
            {
                options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
                options.Scope.Add("user:email");
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ProductManagement", policy => policy.RequireRole("Admin", "Manager"));
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });

            builder.Services.AddAutoMapper(config =>
            {
                config.AddProfile<RegistrationUserProfile>();
                config.AddProfile<ProductCreationProfile>();
                config.AddProfile<ProductEditingProfile>();
                config.AddProfile<CategoryCreationProfile>();
                config.AddProfile<CategoryEditingProfile>();
                config.AddProfile<ManufacturerCreationProfile>();
                config.AddProfile<UserEditingProfile>();
                config.AddProfile<RoleCreationProfile>();
                config.AddProfile<RoleEditingProfile>();
                config.AddProfile<ClaimCreationProfile>();
                config.AddProfile<ClaimDeletingProfile>();
                config.AddProfile<ClaimInfoProfile>();
                config.AddProfile<ClaimInfoCreationProfile>();
            });

            builder.Services.AddTransient<IFileNameGenerator, UniqueFileNameGenerator>()
                            .AddTransient<SlashFilePathNormalizer>()
                            .AddTransient<BackSlashFilePathNormalizer>()
                            .AddTransient<IFormImageProcessor, ProductImageSaver>()
                            .AddTransient<IEqualityComparer<ClaimInfoDto>, ClaimInfoDtoEqualityComparer>();

            builder.Services.AddControllersWithViews();

            return builder;
        }

        public static void Configure(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "shop_list",
                pattern: "Shop/List/{categoryName}",
                defaults: new { controller = "Shop", action = "List" });
            app.MapControllerRoute(
                name: "shop_details",
                pattern: "Shop/Details/{productId:int}",
                defaults: new { controller = "Shop", action = "Details" });
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Shop}/{action=Home}/{id?}");
        }
    }
}