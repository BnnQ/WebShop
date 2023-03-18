using System.Security.Claims;
using AutoMapper;
using Homework.Data.Entities;
using Homework.Filters;
using Homework.Models.Claim;
using Homework.Utils.Extensions;
using Homework.ViewModels.Claim;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ClaimCreationDto = Homework.Models.Claim.ClaimCreationDto;

namespace Homework.Controllers;

[KeepModelErrorsOnRedirect]
[RetrieveModelErrorsFromRedirector]
public class ClaimController : Controller
{
    // GET: Claim
    private readonly UserManager<User> userManager;
    private readonly IMapper mapper;

    public ClaimController(UserManager<User> userManager, IMapper mapper)
    {
        this.userManager = userManager;
        this.mapper = mapper;
    }

    // GET: Claim
    public async Task<IActionResult> List(string userId)
    {
        var claims = await userManager.GetUserClaimsOrDefaultAsync(userId);
            
        var viewModel = new ListViewModel { Claims = claims, UserId = userId };
        return View(viewModel);
    }

    // GET: Claim/Create
    public IActionResult Create(string userId)
    {
        return View(new ClaimCreationDto { UserId = userId});
    }

    // POST: Claim/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClaimCreationDto claimCreationDto)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByIdAsync(claimCreationDto.UserId);
            var claim = mapper.Map<ClaimCreationDto, Claim>(claimCreationDto);

            var result = await userManager.AddClaimAsync(user, claim);
            if (result.Succeeded)
            {
                return RedirectToList(claimCreationDto.UserId);
            }
            else
            {
                AddModelErrors(result);
            }
        }

        return View(claimCreationDto);
    }

    // POST: Claim/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(ClaimDeletingDto claimDeletingDto, [FromServices] IEqualityComparer<ClaimInfoDto> claimDtoComparer)
    {
        if (!await userManager.IsUserHasClaimsAsync(claimDeletingDto.UserId))
        {
            return Problem("Entity set 'ShopContext.Claims' is null.");
        }

        var user = await userManager.FindByIdAsync(claimDeletingDto.UserId);
        var claim = await userManager.GetUserClaimOrDefaultAsync(claimDeletingDto, claimDtoComparer, mapper);
        if (claim is not null)
        {
            await userManager.RemoveClaimAsync(user, claim);
        }

        return RedirectToList(claimDeletingDto.UserId);
    }

    #region Utils
    private IActionResult RedirectToList(string userId)
    {
        return RedirectToAction(nameof(List), new { userId });
    }

    private void AddModelErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
    #endregion
}