namespace Basket.API.Common.Dtos
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
        public List<ShoppingCartItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}
