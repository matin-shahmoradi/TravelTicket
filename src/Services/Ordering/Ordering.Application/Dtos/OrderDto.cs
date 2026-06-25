using Ordering.Domain.Enums;

namespace Ordering.Application.Dtos
{
    public record OrderDto(
        Guid Id,
        CustomerDto Customer,
        OrderStatus OrderStatus,
        List<OrderItemDto> OrderItems)
    {

    }
}
