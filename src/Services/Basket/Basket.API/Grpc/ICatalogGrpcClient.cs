namespace Basket.API.Grpc
{
    public interface ICatalogGrpcClient
    {
        Task<TicketReadModel?> GetTicketByIdAsync(string ticketId);
    }
}
