namespace Basket.API.Model
{
    public class TicketReadModel
    {
        public Guid TicketId { get; set; }
        public string Origin { get; set; } = default!;
        public string Destination { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime TravelDate { get; set; }
        public decimal Price { get; set; }
    }
}
