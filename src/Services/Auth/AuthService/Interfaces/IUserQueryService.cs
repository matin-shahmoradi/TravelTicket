using AuthService.Model.DTOs.UserDtos;

namespace AuthService.Interfaces
{
    public interface IUserQueryService
    {
        Task<IEnumerable<UserResponseDto>> GetUsersAsync(CancellationToken cancellationToken);
        Task<UserResponseDto> GetUsersByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
