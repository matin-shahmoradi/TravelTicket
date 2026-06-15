using Catalog.API.Domain.ValueObjects;
using Catalog.Grpc;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
namespace Catalog.API.Grpc
{
    public class CatalogRpcService(ICatalogDbContext catalogDbContext) : CatalogGrpcService.CatalogGrpcServiceBase
    {
        public override async Task<GetTicketByIdResponse> GetTicketById(GetTicketByIdRequest request, ServerCallContext context)
        {
            var id = TicketId.New(Guid.Parse(request.TicketId));
            var ticket = await catalogDbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);
            if (ticket is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Ticket with id : {request.TicketId} Not Found!"));

            return new GetTicketByIdResponse
            {
                TicketId = ticket.Id.Value.ToString(),
                Origin = ticket.Origin,
                Destination = ticket.Destination,
                TravelDate = ticket.TravelDate.ToString(),
                Price = (double)ticket.Price,
            };
        }
    }
}
