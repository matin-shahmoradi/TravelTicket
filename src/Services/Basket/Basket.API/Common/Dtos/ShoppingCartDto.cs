namespace Basket.API.Common.Dtos
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; } = default!;
        public List<ShoppingCartItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}
