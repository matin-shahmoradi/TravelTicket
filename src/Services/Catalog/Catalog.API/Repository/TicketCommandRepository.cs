using BuildingBlocks.Infrastracture.Outbox.Extensions;
using BuildingBlocks.Messaging;

namespace Catalog.API.Repository
{
    public class TicketCommandRepository(CatalogDbContext context) : ITicketCommandRepository
    {
        public async Task AddTicketAsync(Ticket ticket, CancellationToken cancellationToken)
        {

            await context.Tickets.AddAsync(ticket, cancellationToken);
        }

        public void DeleteTicket(Ticket ticket)
        {
            context.Tickets.Remove(ticket);
        }

        public void UpdateTicket(Ticket ticket)
        {
            context.Tickets.Update(ticket);
        }

        public void PublishIntegrationEvent(IIntegrationEvent integrationEvent)
        {
            context.OutboxMessages.AddIntegrationEvent(integrationEvent);
        }
    }
}
