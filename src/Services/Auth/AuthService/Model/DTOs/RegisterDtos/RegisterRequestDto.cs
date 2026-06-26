namespace AuthService.Model.DTOs.RegisterDtos
{
    public record RegisterRequestDto(
        string Email,
        string Password,
        string PhoneNumber,
        string Firstname,
        string Lastname);
}
