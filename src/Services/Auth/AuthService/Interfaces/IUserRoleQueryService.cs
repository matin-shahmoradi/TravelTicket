namespace AuthService.Interfaces
{
    public interface IUserRoleQueryService
    {
        Task<IReadOnlyList<string>> GetUserRolesAsync(string userId);
        Task<bool> UserHasRoleAsync(string userId, string roleName);
    }
}
