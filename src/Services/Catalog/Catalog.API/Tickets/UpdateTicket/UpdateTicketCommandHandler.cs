namespace Catalog.API.Tickets.UpdateTicket
{
    public record UpdateTicketCommand(Guid Id, TicketRequestDTO UpdateTicketRequest) : ICommand<Result<Ticket>>;
    internal sealed class UpdateTicketCommandHandler(IDocumentSession session)
        : ICommandHandler<UpdateTicketCommand, Result<Ticket>>
    {
        public async Task<Result<Ticket>> Handle(UpdateTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await session.LoadAsync<Ticket>(command.Id, cancellationToken);

            if (ticket is null)
            {
                return Result<Ticket>.Failure(Error.CustomError("Ticket not found!", 404, ErrorType.NOT_FOUND));
            }

            ticket.Origin = command.UpdateTicketRequest.Origin;
            ticket.Destination = command.UpdateTicketRequest.Destination;
            ticket.Description = command.UpdateTicketRequest.Description;
            ticket.Date = command.UpdateTicketRequest.Date;
            ticket.Price = command.UpdateTicketRequest.Price;
            ticket.TravlerName = command.UpdateTicketRequest.TravlerName;
            ticket.TravlerNumber = command.UpdateTicketRequest.TravlerNumber;

            session.Update(ticket);
            await session.SaveChangesAsync();

            return Result<Ticket>.Success(ticket);
        }
    }
}
