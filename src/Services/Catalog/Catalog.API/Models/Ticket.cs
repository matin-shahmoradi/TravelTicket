namespace Catalog.API.Models
{
    public class Ticket
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Origin { get; set; } = default!;
        public string Destination { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public string TravlerName { get; set; } = default!;
        public string TravlerNumber { get; set; } = default!;
    }
}
