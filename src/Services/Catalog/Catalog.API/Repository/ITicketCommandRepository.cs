using BuildingBlocks.Messaging;

namespace Catalog.API.Repository
{
    public interface ITicketCommandRepository
    {
        public Task AddTicketAsync(Ticket ticket, CancellationToken cancellationToken = default);
        public void UpdateTicket(Ticket ticket);
        public void DeleteTicket(Ticket ticket);
        public void PublishIntegrationEvent(IIntegrationEvent integrationEvent);
    }
}
