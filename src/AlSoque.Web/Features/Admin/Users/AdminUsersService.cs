using AlSoque.Data;
using AlSoque.Web.Identity;
using Microsoft.AspNetCore.Identity;

namespace AlSoque.Web.Features.Admin.Users;

public record AdminUserRow(string Id, string Email, string FullName, string Country, string City, bool IsAdmin, bool IsBanned);

public class AdminUsersService(UserManager<ApplicationUser> userManager)
{
    public async Task<List<AdminUserRow>> GetAllAsync()
    {
        var rows = new List<AdminUserRow>();
        foreach (var user in userManager.Users.OrderBy(u => u.Email).ToList())
        {
            var isAdmin = await userManager.IsInRoleAsync(user, AppRoles.Admin);
            var isBanned = await userManager.IsLockedOutAsync(user);
            rows.Add(new AdminUserRow(user.Id, user.Email ?? string.Empty, user.FullName, user.Country, user.City, isAdmin, isBanned));
        }

        return rows;
    }

    public async Task SetAdminAsync(string userId, bool isAdmin)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return;
        }

        var currentlyAdmin = await userManager.IsInRoleAsync(user, AppRoles.Admin);
        if (isAdmin && !currentlyAdmin)
        {
            await userManager.AddToRoleAsync(user, AppRoles.Admin);
        }
        else if (!isAdmin && currentlyAdmin)
        {
            await userManager.RemoveFromRoleAsync(user, AppRoles.Admin);
        }
    }

    public async Task SetBannedAsync(string userId, bool banned)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return;
        }

        if (banned)
        {
            await userManager.SetLockoutEnabledAsync(user, true);
            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        }
        else
        {
            await userManager.SetLockoutEndDateAsync(user, null);
        }
    }
}
