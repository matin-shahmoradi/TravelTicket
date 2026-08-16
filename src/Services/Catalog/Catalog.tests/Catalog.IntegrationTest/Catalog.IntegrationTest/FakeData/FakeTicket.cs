using AutoFixture;
using Bogus;
using Catalog.API.Data;
using Catalog.API.Domain.ValueObjects;
using Catalog.API.Models;
using Catalog.API.Tickets.CreateTicket;

namespace Catalog.IntegrationTest.FakeData
{
    public static class FakeTicket
    {
        static Fixture fixture = new Fixture();
        public static Ticket CreateFakeTicket()
        {
            return fixture.Create<Ticket>();
        }

        public static CreateTicketRequestDTO CreateFakeRequestDto()
        {
            var ticketFaker = new Faker<CreateTicketRequestDTO>();

            return ticketFaker.CustomInstantiator(
                f => new CreateTicketRequestDTO(
                    Origin: f.Address.City(),
                    Destination: f.Address.City(),
                    Description: f.Lorem.Sentence(),
                    Date: DateTime.SpecifyKind(f.Date.Future(), DateTimeKind.Utc),
                    Price: f.Finance.Amount(2000, 50000)));
        }

        public static List<Ticket> GetFakeTickets(int count, bool useNewSeed = false)
        {
            return TicketFacker(useNewSeed).Generate(count);
        }

        private static Faker<Ticket> TicketFacker(bool useNewSeed)
        {
            int seed = 0;
            if (useNewSeed)
                seed = Random.Shared.Next(10, int.MaxValue);

            return new Faker<Ticket>()
                .RuleFor(x => x.Id, x => TicketId.New())
                .RuleFor(x => x.Origin, x => CatalogInitialData.Tickets.Select(t => t.Origin).First())
                .RuleFor(x => x.Destination, x => CatalogInitialData.Tickets.Select(t => t.Destination).First())
                .RuleFor(x => x.Description, x => CatalogInitialData.Tickets.Select(t => t.Description).First())
                .RuleFor(x => x.TravelDate, x => CatalogInitialData.Tickets.Select(t => t.TravelDate).Last())
                .RuleFor(x => x.Price, x => CatalogInitialData.Tickets.Select(t => t.Price).ElementAt(2))
                .UseSeed(seed);
        }
    }
}
