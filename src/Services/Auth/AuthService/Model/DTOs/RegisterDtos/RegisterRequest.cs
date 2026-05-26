namespace AuthService.Model.DTOs.RegisterDtos
{
    public record RegisterRequest(
        string Email,
        string Password,
        string Firstname,
        string Lastname)
    {
    }
}
