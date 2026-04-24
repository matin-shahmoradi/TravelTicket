using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Domain.Models
{
    public class Ticket : Entity<TicketId>
    {
        public string Origin { get; private set; } = default!;
        public string Destination { get; private set;} = default!;
        public Int16 SeatNumber { get; set; }

        public decimal Price { get; private set; } = default!;
        
        public static Ticket Create(TicketId ticketId,string origin, string destination, Int16 seatNumber, decimal price)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin, nameof(origin));
            ArgumentException.ThrowIfNullOrWhiteSpace(destination, nameof(destination));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(seatNumber, nameof(seatNumber));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price, nameof(price));

            var ticket = new Ticket
            {
                Id = ticketId,
                Origin = origin,
                Destination = destination,
                SeatNumber = seatNumber,
                Price = price
            };
            return ticket;
        }

    }
}
