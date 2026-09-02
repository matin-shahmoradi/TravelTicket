using Bogus;
using Catalog.API.Data;
using Catalog.API.Domain.ValueObjects;
using Catalog.API.Models;
using Catalog.API.Tickets.CreateTicket;

namespace Catalog.IntegrationTest.FakeData
{
    public class FakeTicket
    {
        private readonly CatalogDbContext _context;
        private readonly Faker _faker;

        public FakeTicket(CatalogDbContext context)
        {
            _context = context;
            _faker = new Faker();
        }
        public async Task SeedTicket(int count, bool useNewSeed = false)
        {
            int seed = 0;
            if (useNewSeed)
                seed = Random.Shared.Next(10, int.MaxValue);

            var ticketFaker = new Faker<Ticket>()
                .CustomInstantiator(
                    t => Ticket.Create(
                       id: new TicketId(t.Random.Guid()),
                       origin: t.Address.City(),
                       destination: t.Address.City(),
                       description: t.Lorem.Text(),
                       travelDate: DateTime.SpecifyKind(t.Date.Future(), DateTimeKind.Utc),
                       price: t.Finance.Amount()))
                .UseSeed(seed)
                .Generate(count);

            await _context.Tickets.AddRangeAsync(ticketFaker);
            await _context.SaveChangesAsync();
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

        public static Ticket CreateFakeTicket()
        {
            return new Faker<Ticket>()
                .CustomInstantiator(
                    t => Ticket.Create(
                       id: new TicketId(t.Random.Guid()),
                       origin: t.Address.City(),
                       destination: t.Address.City(),
                       description: t.Lorem.Text(),
                       travelDate: DateTime.SpecifyKind(t.Date.Future(), DateTimeKind.Utc),
                       price: t.Finance.Amount()));
        }
    }
}
