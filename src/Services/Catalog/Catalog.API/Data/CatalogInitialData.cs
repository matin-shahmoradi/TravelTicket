using Catalog.API.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data
{
    public static class CatalogInitialData
    {
        public static async Task Populate(this WebApplication app)
        {
            using var scoped = app.Services.CreateScope();

            var context = scoped.ServiceProvider.GetRequiredService<CatalogDbContext>();

            await context.Database.MigrateAsync();
            await InitialData(context);
        }

        private static async Task InitialData(CatalogDbContext context)
        {
            if (!await context.Tickets.AnyAsync())
            {
                var data = new List<Ticket>
                {
                    Ticket.Create(
                        id: TicketId.New(new Guid("2A35F10A-5439-4C15-B70B-8D3C2795B9A3")),
                        origin: "TEHRAN",
                        destination: "ALBORZ",
                        description : "Description",
                        travelDate : DateTime.Now.AddDays(7),
                        price: 5800000),

                    Ticket.Create(
                        id: TicketId.New(new Guid("5F2666E2-2B38-4542-A72E-A3C3561A0FF7")),
                        origin: "MASHHAD",
                        destination: "TEHRAN",
                        description: "Description",
                        travelDate: DateTime.Now.AddDays(7),
                        price: 8000000),


                    Ticket.Create(
                        id: TicketId.New(new Guid("AD3A3D99-0E1C-4902-B71E-6BC845E1CC6A")),
                        origin: "MALAYER",
                        destination: "TABRIZ",
                        description: "Description",
                        travelDate: DateTime.Now.AddDays(7),
                        price: 10000000),

                    Ticket.Create(
                        id: TicketId.New(new Guid("BE79C583-F5B0-4F7D-AC9F-5FDFFE936021")),
                        origin: "TEHRAN",
                        destination: "KISH",
                        description: "Description",
                        travelDate: DateTime.Now.AddDays(7),
                        price: 20000000),

                    Ticket.Create(
                        id: TicketId.New(new Guid("175E6E6C-9A8B-4D64-9471-1A46F148337A")),
                        origin: "KERMANSHAH",
                        destination: "KERMAN",
                        description: "Description",
                        travelDate: DateTime.Now.AddDays(7),
                        price: 5000000),
                };

                await context.Tickets.AddRangeAsync(data);
            }

        }
    }
}
