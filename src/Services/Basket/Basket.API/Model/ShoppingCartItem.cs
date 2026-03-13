namespace Basket.API.Model
{
    public class ShoppingCartItem
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid TicketId { get; set; }
    }
}
