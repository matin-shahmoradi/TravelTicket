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

            ticket.Update(
                origin: command.UpdateTicketRequest.Origin,
                destination: command.UpdateTicketRequest.Destination,
                description: command.UpdateTicketRequest.Description,
                travelDate: command.UpdateTicketRequest.Date,
                price: command.UpdateTicketRequest.Price
                );

            session.Update(ticket);
            await session.SaveChangesAsync();

            return Result<Ticket>.Success(ticket);
        }

    }
}
