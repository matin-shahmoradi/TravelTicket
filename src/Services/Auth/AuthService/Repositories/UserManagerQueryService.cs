using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Model.DTOs.UserDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories
{
    public class UserManagerQueryService(
        UserManager<ApplicationUser> userManager)
        : IUserManagerQueryService
    {
        public async Task<IReadOnlyList<UserResponseDto>> GetUsersAsync(CancellationToken cancellationToken)
        {
            return await userManager
                .Users
                .AsNoTracking()
                .Select(x => new UserResponseDto
                {
                    Id = x.Id,
                    Email = x.Email!,
                    UserName = x.UserName,
                    PhoneNumber = x.PhoneNumber,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                }).ToListAsync(cancellationToken);
        }
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId, bool isTracking = true, CancellationToken cancellationToken = default)
        {
            return await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await userManager.Users
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken)
        {
            var normalizedEmail = email.ToUpperInvariant();
            return !await userManager.Users
                .AsNoTracking()
                .AnyAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
        }
    }
}
