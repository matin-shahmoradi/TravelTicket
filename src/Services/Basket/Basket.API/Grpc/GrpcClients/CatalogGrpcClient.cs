using Catalog.Grpc;

namespace Basket.API.Grpc.GrpcClients
{
    public class CatalogGrpcClient(CatalogGrpcService.CatalogGrpcServiceClient client) : ICatalogGrpcClient
    {
        public async Task<TicketReadModel?> GetTicketByIdAsync(string ticketId)
        {
            try
            {
                var response = await client.GetTicketByIdAsync(new GetTicketByIdRequest
                {
                    TicketId = ticketId
                });

                return new TicketReadModel
                {
                    TicketId = Guid.Parse(response.TicketId),
                    Origin = response.Origin,
                    Destination = response.Destination,
                    Description = response.Decription,
                    TravelDate = DateTime.Parse(response.TravelDate),
                    Price = (decimal)response.Price,
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
