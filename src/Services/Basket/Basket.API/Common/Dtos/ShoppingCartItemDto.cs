namespace Basket.API.Common.Dtos
{
    public class ShoppingCartItemDto
    {
        public Guid TicketId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
