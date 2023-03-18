using AutoMapper;
using Homework.Filters;
using Homework.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework.Controllers;

[Authorize("AdminOnly")]
[KeepModelErrorsOnRedirect]
[RetrieveModelErrorsFromRedirector]
public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IMapper mapper;

    public RoleController(RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        this.roleManager = roleManager;
        this.mapper = mapper;
    }

    // GET: Role
    public async Task<IActionResult> List()
    {
        return View(await roleManager.Roles.ToListAsync());
    }

    // GET: Role/Create
    public IActionResult Create()
    {
        return View(new RoleCreationDto());
    }

    // POST: Role/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleCreationDto roleCreationDto)
    {
        if (ModelState.IsValid)
        {
            var role = mapper.Map<RoleCreationDto, IdentityRole>(roleCreationDto);
            var result = await roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(List));
            }
            else
            {
                AddModelErrors(result);
            }
        }

        return View(roleCreationDto);
    }

    // GET: Role/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || roleManager.Roles?.Any() is false)
        {
            return NotFound();
        }

        var role = await roleManager.FindByIdAsync(id);
        if (role is null)
        {
            return NotFound();
        }

        var roleEditingDto = mapper.Map<IdentityRole, RoleEditingDto>(role);
        return View(roleEditingDto);
    }

    // POST: Role/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RoleEditingDto roleEditingDto)
    {
        if (ModelState.IsValid)
        {
            var role = await roleManager.FindByIdAsync(roleEditingDto.Id);
            if (role is null)
            {
                return NotFound();
            }
            mapper.Map(roleEditingDto, role);

            var result = await roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToList();
            }
            else
            {
                AddModelErrors(result);
            }
        }

        return View(roleEditingDto);
    }

    // GET: Role/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || roleManager.Roles?.Any() is false)
        {
            return NotFound();
        }

        var role = await roleManager.FindByIdAsync(id);
        if (role is null)
        {
            return NotFound();
        }

        return View(role);
    }

    // POST: Role/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        if (roleManager.Roles?.Any() is false)
        {
            return Problem("Entity set 'ShopContext.Roles' is null.");
        }
        var role = await roleManager.FindByIdAsync(id);
        if (role is not null)
        {
            await roleManager.DeleteAsync(role);
        }

        return RedirectToList();
    }

    #region Utils
    private IActionResult RedirectToList()
    {
        return RedirectToAction(nameof(List));
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