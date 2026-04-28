using BuildingBlocks.DDD;
using Catalog.API.Domain.ValueObjects;
using Catalog.API.Events;

namespace Catalog.API.Models
{
    public class Ticket : Aggregate<TicketId>
    {
        public string Origin { get; private set; } = default!;
        public string Destination { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public DateTime TravelDate { get; private set; }
        public decimal Price { get; private set; }

        public static Ticket Create(
            TicketId id,
            string origin,
            string destination,
            string description,
            DateTime travelDate,
            decimal price)
        {
            var ticket = new Ticket
            {
                Id = id,
                Origin = origin,
                Destination = destination,
                Description = description,
                TravelDate = travelDate,
                Price = price
            };
            ticket.AddDomainEvents(new TicketCreatedEvent(ticket));
            return ticket;
        }

        public void Update(
            string origin,
            string destination,
            string description,
            DateTime travelDate,
            decimal price)
        {

            Origin = origin;
            Destination = destination;
            Description = description;
            TravelDate = travelDate;
            Price = price;

            AddDomainEvents(new TicketUpdatedEvent(this));
        }
    }
}
