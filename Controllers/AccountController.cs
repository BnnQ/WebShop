using AutoMapper;
using Homework.Data;
using Homework.Data.Entities;
using Homework.Utils;
using Homework.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Homework.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signManager;
        private readonly IMapper mapper;
        private readonly ShopContext shopContext;

        public AccountController(UserManager<User> userManager, SignInManager<User> signManager, IMapper mapper, ShopContext shopContext)
        {
            this.userManager = userManager;
            this.signManager = signManager;
            this.mapper = mapper;
            this.shopContext = shopContext;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(SignInViewModel signInViewModel, string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(signInViewModel.UserName))
            {
                return await ReturnWithErrorAsync($"User with this name does not exist.");
            }
            if (string.IsNullOrWhiteSpace(signInViewModel.Password))
            {
                return await ReturnWithErrorAsync("Incorrect password. Please try again.");
            }

            User user = await userManager.FindByNameAsync(signInViewModel.UserName);
            if (user is null)
            {
                return await ReturnWithErrorAsync($"Username '{signInViewModel.UserName}' does not exist.");
            }
            var result = await signManager.PasswordSignInAsync(user, signInViewModel.Password, isPersistent: signInViewModel.IsPersistent, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    return RedirectToAdmin();
                }
                else
                {
                    return RedirectToLocal(url: returnUrl);
                }
            }

            string errorMessage;
            if (await userManager.IsLockedOutAsync(user))
            {
                errorMessage = "Your account has been temporarily blocked due to a large number of failed login attempts. Please try again later.";
            }
            else
            {
                const int MaxFailedAccessAttempts = 5;
                int remainingAccessTries = MaxFailedAccessAttempts - await userManager.GetAccessFailedCountAsync(user);
                errorMessage = remainingAccessTries > 3 ? "Incorrect password. Please try again." : $"Incorrect password. Remaining tries: {remainingAccessTries}";
            }

            return await ReturnWithErrorAsync(errorMessage);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentNullException(nameof(provider));

            string redirectUrl = Url.Action(action: nameof(ExternalLoginCallback), controller: "Account", values: new { returnUrl })!;
            var properties = signManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var info = await signManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                return await RedirectToLoginAsync();
            }

            string email = info.Principal.FindFirstValue(ClaimTypes.Email);
            User user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                user = new User { UserName = email, Email = email };

                if (DateTime.TryParse(info.Principal.FindFirstValue(ClaimTypes.DateOfBirth), out DateTime dateOfBirth) 
                    && dateOfBirth.Year != 0)
                {
                    user.YearOfBirth = dateOfBirth.Year;
                }

                await userManager.RegisterCustomerAsync(user);
            }

            await userManager.AddLoginAsync(user, info);
            var result = await signManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }

            return await RedirectToLoginAsync();
        }

        [Authorize]
        [HttpGet("Logout")]

        public async Task<IActionResult> Logout()
        {
            await signManager.SignOutAsync();
            return RedirectToHome();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel registrationViewModel, string? returnUrl)
        {
            IdentityResult? registrationResult = null;
            if (ModelState.IsValid)
            {
                User user = mapper.Map<User>(registrationViewModel);
                string password = registrationViewModel.Password;
                    registrationResult = await userManager.RegisterCustomerAsync(user, password);

                if (registrationResult.Succeeded)
                {
                    await signManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
            }

            TryAddModelErrorsFromResult(registrationResult);
            return await RedirectToHomeWithErrorsAsync();
        }

        #region Utils
        private IActionResult RedirectToAdmin()
        {
            return RedirectToAction(controllerName: "Admin", actionName: "Home");
        }
        private IActionResult RedirectToHome()
        {
            return RedirectToAction(controllerName: "Shop", actionName: "Home");
        }
        private async Task<IActionResult> RedirectToHomeWithErrorsAsync()
        {
            return View("/Views/Shop/Home.cshtml", await shopContext.Banners.ToListAsync());
        }
        private IActionResult RedirectToLocal(string? url)
        {
            if (!string.IsNullOrWhiteSpace(url) && Url.IsLocalUrl(url))
            {
                return Redirect(url);
            }
            else
            {
                return RedirectToHome();
            }
        }
        private Task<IActionResult> RedirectToLoginAsync()
        {
            return ReturnWithErrorAsync("Something went wrong during external logging in.");
        }

        private void AddModelError(string errorMessage)
        {
            ModelState.AddModelError(string.Empty, errorMessage);
        }

        private Task<IActionResult> ReturnWithErrorAsync(string errorMessage)
        {
            AddModelError(errorMessage);
            return RedirectToHomeWithErrorsAsync();
        }

        private void TryAddModelErrorsFromResult(IdentityResult? result)
        {
            if (result?.Errors?.Any() is true)
            {
                foreach (IdentityError error in result.Errors)
                    AddModelError(error.Description);
            }
        }
        #endregion
    }
}