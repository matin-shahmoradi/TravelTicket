namespace Ordering.Application.Dtos
{
    public record GetOrderByCustomerIdResult(IEnumerable<OrderDto> Orders)
    {
        public static GetOrderByCustomerIdResult New(IEnumerable<OrderDto> value) => new GetOrderByCustomerIdResult(value);
    };
}
