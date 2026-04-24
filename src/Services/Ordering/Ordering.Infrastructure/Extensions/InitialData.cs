using Ordering.Domain.ValueObjects;
using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Infrastructure.Extensions
{
    internal class InitialData
    {
        public static IEnumerable<Customer> Customers => new List<Customer>
        {
            Customer.Create(
                id: CustomerId.New(new Guid("F71A45A3-52DD-4F8C-8D5A-A65ECC255F6C")),
                name: "matin shahmoradi",
                nationalCode: "2987854008",
                phoneNumber: "09030000000",
                email: "Matinshahmoradi01@gmail.com"
                ),
            Customer.Create(
                id: CustomerId.New(new Guid("0DE247FA-A269-4E6E-B6D7-46389A936A27")),
                name: "saman hosseiny",
                nationalCode: "2945984008",
                phoneNumber: "09120000000",
                email: "samanmaman01@gmail.com"
                )
        };

        public static IEnumerable<Ticket> Tickets => new List<Ticket>
        {
            Ticket.Create(
                ticketId: TicketId.New(new Guid("763B5A58-0F26-434A-B4E4-26ED2ACE3C08")),
                origin: "TEHRAN",
                destination: "TABRIZ",
                seatNumber: 13,
                price: 19700000
                ),
            Ticket.Create(
                ticketId: TicketId.New(new Guid("A2B80E07-29B3-4684-B19C-41B1C429367C")),
                origin: "AHVAZ",
                destination: "MASHHAD",
                seatNumber: 18,
                price: 30000000
                ),
            Ticket.Create(
                ticketId: TicketId.New(new Guid("B248E13A-06C5-4F17-87DF-F80D3065370C")),
                origin: "TEHRAN",
                destination: "SARI",
                seatNumber: 22,
                price: 10000000
                ),
            Ticket.Create(
                ticketId: TicketId.New(new Guid("225A716F-AEDC-433E-9910-25D720393045")),
                origin: "TEHRAN",
                destination: "RASHT",
                seatNumber: 22,
                price: 13000000
                ),
        };

        public static IEnumerable<Order> OrdersWithItems
        {
            get
            {
                var payment1 = Payment.New("Matin shahmoradi", "6280589689660258", "07/09", "789", 1);
                var payment2 = Payment.New("Saman hosseiny", "6212589689660258", "07/09", "598", 2);

                var order1 = Order.Create(
                   id: OrderId.New(Guid.NewGuid()),
                   customerId: CustomerId.New(new Guid("F71A45A3-52DD-4F8C-8D5A-A65ECC255F6C")),
                   orderName: OrderName.New("ORD-Matin shahmoradi"),
                   payment: payment1
                   );

                order1.Add(
                    ticketId: TicketId.New(new Guid("763B5A58-0F26-434A-B4E4-26ED2ACE3C08")),
                    quantity: 1,
                    price: 19700000);

                var order2 = Order.Create(
                   id: OrderId.New(Guid.NewGuid()),
                   customerId: CustomerId.New(new Guid("0DE247FA-A269-4E6E-B6D7-46389A936A27")),
                   orderName: OrderName.New("ORD-Saman Hosseiny"),
                   payment: payment2
                   );

                order2.Add(
                    ticketId: TicketId.New(new Guid("B248E13A-06C5-4F17-87DF-F80D3065370C")),
                    quantity: 1,
                    price: 30000000);
                return new List<Order> { order1, order2 };
            }
        }
    }
}
