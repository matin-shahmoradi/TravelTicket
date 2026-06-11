using AuthService.Data;
using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Model.DTOs.UserDtos;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories
{
    public class UserManagerQueryService(
        AuthDbContext authDbContext)
        : IUserManagerQueryService
    {
        public async Task<IReadOnlyList<UserResponseDto>> GetUsersAsync(CancellationToken cancellationToken)
        {
            return await authDbContext
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
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await authDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await authDbContext.Users
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken)
        {
            var normalizedEmail = email.ToUpperInvariant();
            return !await authDbContext.Users
                .AsNoTracking()
                .AnyAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
        }
    }
}
