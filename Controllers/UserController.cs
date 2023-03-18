using AutoMapper;
using Homework.Data.Entities;
using Homework.Filters;
using Homework.Models.User;
using Homework.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework.Controllers
{
    [Authorize("AdminOnly")]
    [KeepModelErrorsOnRedirect]
    [RetrieveModelErrorsFromRedirector]
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        // GET: User
        public async Task<IActionResult> List()
        {
            return View(await userManager.Users.ToListAsync());
        }

        // GET: User/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new UserCreationDto()
            {
                Roles = await GetRolesCheckBoxAsync()
            };

            return View(viewModel);
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreationDto userCreationDto)
        {
            if (ModelState.IsValid)
            {
                string userName = userCreationDto.UserName;
                string email = userCreationDto.Email;
                var user = new User { UserName = userName, NormalizedUserName = userName.ToUpper(), Email = email, NormalizedEmail = email.ToUpper(), YearOfBirth = userCreationDto.YearOfBirth, Balance = userCreationDto.Balance };
                var result = await userManager.CreateAsync(user, userCreationDto.Password);

                if (result.Succeeded)
                {
                    var selectedRoles = userCreationDto.Roles.Where(role => role.IsChecked)
                                                                   .Select(role => role.Name);
                    await userManager.AddToRolesAsync(user, selectedRoles);
                    return RedirectToAction(nameof(List));
                }

                AddModelErrors(result);
            }

            return View(userCreationDto);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || userManager.Users?.Any() is false)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            UserEditingDto userEditingDto = mapper.Map<User, UserEditingDto>(user);
            var roles = await GetRolesCheckBoxAsync();
            foreach (var role in  roles)
            {
                role.IsChecked = await userManager.IsInRoleAsync(user, role.Name);
            }

            userEditingDto.Roles = roles;
            return View(userEditingDto);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditingDto userEditingDto)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(userEditingDto.Id);
                if (user is null)
                {
                    return NotFound();
                }

                mapper.Map(userEditingDto, user);

                var userRoles = await userManager.GetRolesAsync(user);
                var selectedRoles = userEditingDto.Roles.Where(role => role.IsChecked)
                                                        .Select(role => role.Name);

                var rolesToAdd = selectedRoles.Except(userRoles);
                var rolesToRemove = userRoles.Except(selectedRoles);

                await userManager.AddToRolesAsync(user, rolesToAdd);
                await userManager.RemoveFromRolesAsync(user, rolesToRemove);

                IdentityResult result = null!;
                if (!string.IsNullOrWhiteSpace(userEditingDto.Password))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    result = await userManager.ResetPasswordAsync(user, token, userEditingDto.Password);

                    if (!result.Succeeded)
                    {
                        AddModelErrors(result);
                        userEditingDto.Roles = await GetRolesCheckBoxAsync();
                        return View(userEditingDto);
                    }
                }

                if (userEditingDto.LockoutEnabled && !await userManager.IsLockedOutAsync(user))
                {
                    await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddHours(1));
                }
                else if (!userEditingDto.LockoutEnabled && await userManager.IsLockedOutAsync(user))
                {
                    await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now);
                }

                result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToList();
                }

                AddModelErrors(result);
                userEditingDto.Roles = await GetRolesCheckBoxAsync();
            }

            return View(userEditingDto);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || userManager.Users?.Any() is false)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (userManager.Users?.Any() is false)
            {
                return Problem("Entity set 'ShopContext.Users' is null.");
            }
            var user = await userManager.FindByIdAsync(id);
            if (user is not null)
            {
                await userManager.DeleteAsync(user);
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

        private async Task<List<RoleCheckBoxViewModel>> GetRolesCheckBoxAsync()
        {
            var roles = await roleManager.Roles.ToListAsync();

            var checkBoxList = roles.Select(role => new RoleCheckBoxViewModel
            {
                Name = role.Name,
                IsChecked = false
            }).ToList();

            return checkBoxList;
        }
        #endregion
    }
}
