namespace Ordering.Application.Dtos
{
    public record class CustomerDto(
        Guid CustomerId,
        string? nationalCode,
        string? Name,
        string? Email,
        string? PhoneNumber);
}
