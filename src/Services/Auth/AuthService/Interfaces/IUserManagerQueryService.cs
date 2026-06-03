using AuthService.Model;
using AuthService.Model.DTOs.UserDtos;

namespace AuthService.Interfaces
{
    public interface IUserManagerQueryService
    {
        Task<IReadOnlyList<UserResponseDto>> GetUsersAsync(CancellationToken cancellationToken);
        Task<ApplicationUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
        Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken);
    }
}
