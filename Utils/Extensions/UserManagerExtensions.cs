using System.Security.Claims;
using AutoMapper;
using Homework.Models.Claim;
using Microsoft.AspNetCore.Identity;

namespace Homework.Utils.Extensions;

public static class UserManagerExtensions
{
    public static async Task<List<Claim>?> GetUserClaimsOrDefaultAsync<TUser>(this UserManager<TUser> userManager, string userId) where TUser : IdentityUser<string>
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return null;
        
        var claims = await userManager.GetClaimsAsync(user);
        return claims?.ToList();
    }

    public static async Task<Claim?> GetUserClaimOrDefaultAsync<TUser>(this UserManager<TUser> userManager, ClaimInfoDto claimDto, IEqualityComparer<ClaimInfoDto> claimDtoEqualityComparer, IMapper mapper) where TUser : IdentityUser<string>
    {
        var userClaims = await userManager.GetUserClaimsOrDefaultAsync(claimDto.UserId);
        if (userClaims?.Any() is not true)
            return null;
        
        var claim = userClaims.FirstOrDefault(claim => claimDtoEqualityComparer.Equals(mapper.Map<Claim, ClaimInfoDto>(claim), claimDto));
        return claim;
    }

    public static async Task<bool> IsUserHasClaimAsync<TUser>(this UserManager<TUser> userManager, TUser user, Claim claim, IEqualityComparer<ClaimInfoDto> claimDtoComparer, IMapper mapper) where TUser : IdentityUser<string>
    {
        var userClaims = await userManager.GetUserClaimsOrDefaultAsync(user.Id);
        if (userClaims?.Any() is not true)
            return false;

        var claimInfoDto = mapper.Map<Claim, ClaimInfoDto>(claim);
        return userClaims.Any(item => claimDtoComparer.Equals(mapper.Map<Claim, ClaimInfoDto>(item), claimInfoDto));
    }
    public static async Task<bool> IsUserHasClaimsAsync<TUser>(this UserManager<TUser> userManager, TUser user)
        where TUser : IdentityUser<string>
    {
        return (await userManager.GetUserClaimsOrDefaultAsync(user.Id))?.Any() is true;
    }
    
    public static async Task<bool> TryAddUserClaimAsync<TUser>(this UserManager<TUser> userManager, TUser user, Claim claim, IEqualityComparer<ClaimInfoDto> claimDtoComparer, IMapper mapper) where TUser : IdentityUser<string>
    {
        if (await userManager.IsUserHasClaimAsync(user, claim, claimDtoComparer, mapper)) return false;
        
        var result = await userManager.AddClaimAsync(user, claim);
        return result.Succeeded;
    }
    
    public static async Task<bool> TryAddUserClaimsAsync<TUser>(this UserManager<TUser> userManager, TUser user, IEnumerable<Claim> claims, IEqualityComparer<ClaimInfoDto> claimDtoComparer, IMapper mapper) where TUser : IdentityUser<string>
    {
        var userClaims = await userManager.GetUserClaimsOrDefaultAsync(user.Id);
        if (userClaims is null)
            return false;

        var result = false;
        foreach (var claim in claims)
        {
            if (!await userManager.IsUserHasClaimAsync(user, claim, claimDtoComparer, mapper))
            {
                if ((await userManager.AddClaimAsync(user, claim)).Succeeded)
                    result = true;
            }
        }

        return result;
    }

    public static async Task<bool> IsUserHasClaimsAsync<TUser>(this UserManager<TUser> userManager, string userId)
        where TUser : IdentityUser<string>
    {
        var user = await userManager.FindByIdAsync(userId);
        return await userManager.IsUserHasClaimsAsync(user);
    } 
}