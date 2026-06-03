namespace AuthService.Model.DTOs.RegisterDtos
{
    public record RegisterRequestDto(
        string Email,
        string Password,
        string Firstname,
        string Lastname);
}
