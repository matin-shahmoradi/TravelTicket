using Catalog.API.Domain.Exception;
using Catalog.API.Domain.ValueObjects;

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
            if (string.IsNullOrWhiteSpace(origin))
            {
                throw new DomainException("Origin cant be null or empty");
            }
            if (string.IsNullOrWhiteSpace(destination))
            {
                throw new DomainException("destination cant be null or empty");
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new DomainException("description cant be null or empty");
            }
            if (origin.Equals(destination, StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException("origin and destination can't be the same");
            }
            if (price < 0)
            {
                throw new DomainException("price should be greater than zero");
            }
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
            var priceChanged = this.Price != price;

            this.Origin = origin;
            this.Destination = destination;
            this.Description = description;
            this.TravelDate = travelDate;
            this.Price = price;

            AddDomainEvents(new TicketUpdatedEvent(this));

            if (priceChanged)
            {
                AddDomainEvents(new TicketPriceChangedEvent(this));
            }
        }

        public void ChangeOrigin(string newOrigin)
        {
            if (string.IsNullOrWhiteSpace(newOrigin))
                throw new DomainException("new origin can't be empty");

            if (this.Origin == newOrigin)
                return;

            this.Origin = newOrigin;

            AddDomainEvents(new TicketUpdatedEvent(this));
        }
        public void ChangeDestination(string newDestination)
        {
            if (string.IsNullOrWhiteSpace(newDestination))
                throw new DomainException("new Destination can't be empty");

            if (this.Destination == newDestination)
                return;

            this.Destination = newDestination;

            AddDomainEvents(new TicketUpdatedEvent(this));
        }

        public void ChangeDescription(string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
                throw new DomainException("new Description can't be empty");

            if (this.Description == newDescription)
                return;

            this.Description = newDescription;

            AddDomainEvents(new TicketUpdatedEvent(this));
        }

        public void ChangeTravelDate(DateTime newTravelDate)
        {
            if (TravelDate == newTravelDate)
                return;

            this.TravelDate = newTravelDate;

            AddDomainEvents(new TicketUpdatedEvent(this));
        }

        public void ChangePrice(decimal newPrice)
        {
            if (decimal.IsNegative(newPrice))
                throw new DomainException("price can't be less than zero");

            if (this.Price == newPrice) return;

            this.Price = newPrice;

            AddDomainEvents(new TicketPriceChangedEvent(this));
        }
    }
}
