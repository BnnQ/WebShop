using AutoMapper;
using Homework.Data.Entities;
using Homework.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Homework.Filters;
using Homework.Models.Claim;
using Homework.Utils.Extensions;

namespace Homework.Controllers;

[KeepModelErrorsOnRedirect]
public class AccountController : Controller
{
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signManager;
    private readonly IMapper mapper;
    private readonly IWebHostEnvironment environment;

    public AccountController(UserManager<User> userManager, SignInManager<User> signManager, IMapper mapper, IWebHostEnvironment environment)
    {
        this.userManager = userManager;
        this.signManager = signManager;
        this.mapper = mapper;
        this.environment = environment;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        return ReturnWithError(returnUrl, "To perform this action you need to sign in.");   
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(SignInViewModel signInViewModel, string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(signInViewModel.UserName))
        {
            return ReturnWithError(returnUrl, "User with this name does not exist.");
        }
        if (string.IsNullOrWhiteSpace(signInViewModel.Password))
        {
            return ReturnWithError(returnUrl, "Incorrect password. Please try again.");
        }

        User user = await userManager.FindByNameAsync(signInViewModel.UserName);
        if (user is null)
        {
            return ReturnWithError(returnUrl, $"Username '{signInViewModel.UserName}' does not exist.");
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

        return ReturnWithError(returnUrl, errorMessage);
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
    public async Task<IActionResult> ExternalLoginCallback([FromServices] IEqualityComparer<ClaimInfoDto> claimDtoComparer, string returnUrl = "/")
    {
        var info = await signManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            return RedirectToLogin(returnUrl);
        }

        string email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var user = await userManager.FindByEmailAsync(email);
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
            await userManager.TryAddUserClaimsAsync(user, info.Principal.Claims, claimDtoComparer, mapper);
            return RedirectToLocal(returnUrl);
        }

        return RedirectToLogin(returnUrl);
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
            var user = mapper.Map<User>(registrationViewModel);
            string password = registrationViewModel.Password;
            registrationResult = await userManager.RegisterCustomerAsync(user, password);

            if (registrationResult.Succeeded)
            {
                await signManager.SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
        }

        TryAddModelErrorsFromResult(registrationResult);
        return RedirectToHome();
    }

    #region Utils
    private IActionResult RedirectToAdmin()
    {
        return RedirectToAction(controllerName: "Admin", actionName: "Home");
    }
        
    private IActionResult RedirectToHome()
    {
        return RedirectToAction(controllerName: "Home", actionName: "Home");
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
    private IActionResult RedirectToLogin(string? returnUrl)
    {
        return ReturnWithError(returnUrl, "Something went wrong.");
    }

    private void AddModelError(string errorMessage)
    {
        ModelState.AddModelError(string.Empty, errorMessage);
    }
    
    private static Tuple<string, string> GetControllerAndAction(string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl) || returnUrl[0] != '/')
        {
            throw new ArgumentNullException(returnUrl);
        }

        int firstSlashIndex = returnUrl.IndexOf('/');
        if (firstSlashIndex < 0 || firstSlashIndex == returnUrl.Length - 1)
        {
            throw new FormatException();
        }

        int secondSlashIndex = returnUrl.IndexOf('/', firstSlashIndex + 1);
        if (secondSlashIndex < 0)
        {
            secondSlashIndex = returnUrl.Length;
        }

        string controller = returnUrl.Substring(firstSlashIndex + 1, secondSlashIndex - firstSlashIndex - 1);
        string action = returnUrl.Substring(secondSlashIndex + 1);

        return Tuple.Create(controller, action);
    }

    private IActionResult ReturnWithError(string? returnUrl, string errorMessage)
    {
        AddModelError(errorMessage);

        var resultReturnUrl = "/";
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            var (controller, action) = GetControllerAndAction(returnUrl);
            if (System.IO.File.Exists($"{environment.ContentRootPath}/Views/{controller}/{action}.cshtml"))
                resultReturnUrl = returnUrl;
            else if (System.IO.File.Exists($"{environment.ContentRootPath}/Views/{controller}/List.cshtml"))
                resultReturnUrl = $"/{controller}/List";
            else if (System.IO.File.Exists($"{environment.ContentRootPath}/Views/{controller}/Home.cshtml"))
                resultReturnUrl = $"/{controller}/Home";
        }
        
        return RedirectToLocal(resultReturnUrl);
    }

    private void TryAddModelErrorsFromResult(IdentityResult? result)
    {
        if (result?.Errors?.Any() is true)
        {
            foreach (var error in result.Errors)
                AddModelError(error.Description);
        }
    }
    #endregion
}