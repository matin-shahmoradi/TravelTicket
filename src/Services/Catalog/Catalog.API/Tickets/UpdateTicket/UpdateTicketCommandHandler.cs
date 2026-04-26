using Catalog.API.Domain.ValueObjects;

namespace Catalog.API.Tickets.UpdateTicket
{
    public record UpdateTicketCommand(Guid Id, TicketRequestDTO UpdateTicketRequest) : ICommand<Result<Ticket>>;
    internal sealed class UpdateTicketCommandHandler(ICatalogDbContext CatalogDb)
        : ICommandHandler<UpdateTicketCommand, Result<Ticket>>
    {
        public async Task<Result<Ticket>> Handle(UpdateTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await CatalogDb.Tickets.FindAsync(TicketId.New(command.Id),cancellationToken);

            if (ticket is null)
            {
                return Result<Ticket>.Failure(Error.NotFoundError(message: "Ticket Not Found!"));
            }

            ticket.Update(
                origin: command.UpdateTicketRequest.Origin,
                destination: command.UpdateTicketRequest.Destination,
                description: command.UpdateTicketRequest.Description,
                travelDate: command.UpdateTicketRequest.Date,
                price: command.UpdateTicketRequest.Price
                );

            CatalogDb.Tickets.Update(ticket);
            await CatalogDb.SaveChangesAsync(cancellationToken);

            return Result<Ticket>.Success(ticket);
        }

    }
}
