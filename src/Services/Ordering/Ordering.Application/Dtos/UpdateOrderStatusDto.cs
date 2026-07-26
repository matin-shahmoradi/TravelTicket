namespace Ordering.Application.Dtos
{
    public sealed record UpdateOrderStatusDto(Guid OrderId, string PaymentStatus)
    {
    }
}
