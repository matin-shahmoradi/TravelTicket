using AuthService.Data;
using AuthService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories
{
    public class UserRoleQueryService(AuthDbContext authDbContext)
        : IUserRoleQueryService
    {
        public async Task<IReadOnlyList<string>> GetUserRolesAsync(string userId)
        {
            return await authDbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .Join(
                    authDbContext.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name!)
                .ToListAsync();
        }

        public async Task<bool> UserHasRoleAsync(string userId, string roleName)
        {
            var normalizedName = roleName.ToUpperInvariant();
            return await authDbContext.UserRoles
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .Join(
                    authDbContext.Roles.AsNoTracking(),
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new { ur, r })
                .AnyAsync(x =>
                    x.ur.UserId == userId &&
                    x.r.NormalizedName == normalizedName);
        }
    }
}
