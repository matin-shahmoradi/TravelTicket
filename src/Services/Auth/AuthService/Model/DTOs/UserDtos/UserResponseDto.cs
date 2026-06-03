namespace AuthService.Model.DTOs.UserDtos
{
    public sealed class UserResponseDto
    {
        public string Id { get; init; } = default!;
        public string? UserName { get; init; }
        public string Email { get; init; } = default!;
        public string? PhoneNumber { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
    }
}
