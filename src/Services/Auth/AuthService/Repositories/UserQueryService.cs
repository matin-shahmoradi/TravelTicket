using AuthService.Data;
using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Model.DTOs.UserDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories
{
    public class UserQueryService(
        AuthDbContext authDbContext,
        UserManager<ApplicationUser> userManager)
        : IUserQueryService
    {
        public async Task<IEnumerable<UserResponseDto>> GetUsersAsync(CancellationToken cancellationToken)
        {
            var users = await authDbContext
                .Users
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return users.Select(x => new UserResponseDto(
                Username: x.UserName! ,
                Email: x.Email!,
                FirstName: x.FirstName,
                LastName: x.LastName,
                PhoneNumber: x.PhoneNumber!)
            ).ToList();
        }

        public async Task<UserResponseDto> GetUsersByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var getUser = await userManager.FindByEmailAsync(email);
            return new UserResponseDto
            (
                Username: getUser!.UserName!,
                Email: getUser.Email!,
                FirstName: getUser.FirstName,
                LastName: getUser.LastName,
                PhoneNumber: getUser.PhoneNumber!
            );
        }
    }
}
