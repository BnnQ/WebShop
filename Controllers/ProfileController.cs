using System.Security.Claims;
using Homework.Data.Entities;
using Homework.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Homework.Controllers;

[Authorize(policy: "Authenticated")]
public class ProfileController : Controller
{
    private readonly UserManager<User> userManager;

    public ProfileController(UserManager<User> userManager)
    {
        this.userManager = userManager;
    }

    
    public async Task<IActionResult> Home()
    {
        var user = await GetUserAsync();
        return View(new HomeViewModel { UserName = user.UserName, UserBalance = user.Balance });
    }

    [HttpPost]
    public async Task<IActionResult> TopUpBalance(int topUpAmount)
    {
        var user = await GetUserAsync();
        user.Balance += topUpAmount;
        await userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Home));
    }
    
    #region Utils
    private async Task<User> GetUserAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userManager.FindByIdAsync(userId);

        return user;
    }
    #endregion
}