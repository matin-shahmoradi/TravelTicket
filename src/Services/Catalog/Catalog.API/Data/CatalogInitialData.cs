using Marten.Schema;

namespace Catalog.API.Data
{
    public class CatalogInitialData : IInitialData
    {
        public async Task Populate(IDocumentStore store, CancellationToken cancellation)
        {
            using var session = store.LightweightSession();

            if(await session.Query<Ticket>().AnyAsync())
                return;

            session.Store<Ticket>(InitialData());
            await session.SaveChangesAsync();

        }

        private static IEnumerable<Ticket> InitialData()
        {
            return new List<Ticket>
            {
                new Ticket()
                {
                    Id = new Guid("2A35F10A-5439-4C15-B70B-8D3C2795B9A3"),
                    Origin = "TEHRAN",
                    Destination = "ALBORZ",
                    Description = "Description",
                    Date = DateTime.Now.AddDays(7),
                    Price = 5800000,
                    TravlerName = "MATIN SHAHMMORADI",
                    TravlerNumber = "09035647823"
                },
                new Ticket()
                {
                    Id = new Guid("5F2666E2-2B38-4542-A72E-A3C3561A0FF7"),
                    Origin = "MASHHAD",
                    Destination = "TEHRAN",
                    Description = "Description",
                    Date = DateTime.Now.AddDays(7),
                    Price = 8000000,
                    TravlerName = "ALI SHAHMMORADI",
                    TravlerNumber = "09035647823"
                },
                new Ticket()
                {
                    Id = new Guid("AD3A3D99-0E1C-4902-B71E-6BC845E1CC6A"),
                    Origin = "MALAYER",
                    Destination = "TABRIZ",
                    Description = "Description",
                    Date = DateTime.Now.AddDays(7),
                    Price = 10000000,
                    TravlerName = "KAMRAN",
                    TravlerNumber = "09035647823"
                },
                new Ticket()
                {
                    Id = new Guid("BE79C583-F5B0-4F7D-AC9F-5FDFFE936021"),
                    Origin = "TEHRAN",
                    Destination = "KISH",
                    Description = "Description",
                    Date = DateTime.Now.AddDays(7),
                    Price = 20000000,
                    TravlerName = "MAKHDO",
                    TravlerNumber = "09900000000"
                },
                new Ticket()
                {
                    Id = new Guid("175E6E6C-9A8B-4D64-9471-1A46F148337A"),
                    Origin = "KERMANSHAH",
                    Destination = "KERMAN",
                    Description = "Description",
                    Date = DateTime.Now.AddDays(7),
                    Price = 5000000,
                    TravlerName = "MILAD REZAEI",
                    TravlerNumber = "09124657820"
                }
            };
        }
    }

}
