namespace Ordering.Application.Dtos
{
    public record OrderItemDto(
        Guid OrderId,
        Guid TicketId,
        int Quantity,
        decimal Price);
}
