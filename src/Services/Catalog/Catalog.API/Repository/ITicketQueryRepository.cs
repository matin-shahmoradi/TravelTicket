namespace Catalog.API.Repository
{
    public interface ITicketQueryRepository
    {
        Task<Ticket?> GetTicketById(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Ticket?> GetTicketByIdWithNoTracking(Guid id, CancellationToken cancellationToken = default);
    }
}
