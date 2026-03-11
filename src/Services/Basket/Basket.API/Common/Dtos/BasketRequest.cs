namespace Basket.API.Common.Dtos
{
    public class BasketRequest
    {
        public List<ShoppingCartItem> Items { get; set; } = new();
        public string TravlerNumber { get; set; } = default!;
    }
}
