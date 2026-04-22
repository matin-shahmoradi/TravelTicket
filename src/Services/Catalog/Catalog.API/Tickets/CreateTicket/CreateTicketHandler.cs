namespace Catalog.API.Tickets.CreateTicket
{
    public record CreateTicketCommand(TicketRequestDTO CreateTicketRequest) : ICommand<Result<Guid>>;
    public sealed class CreateTicketCommandHandler(IDocumentSession session) : ICommandHandler<CreateTicketCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            // Creat new Ticket object.
            var ticket = new Ticket
            {
                Origin = request.CreateTicketRequest.Origin,
                Destination = request.CreateTicketRequest.Destination,
                Description = request.CreateTicketRequest.Description,
                Date = request.CreateTicketRequest.Date,
                Price = request.CreateTicketRequest.Price,
                TravlerName = request.CreateTicketRequest.TravlerName,
                TravlerNumber = request.CreateTicketRequest.TravlerNumber
            };

            // Save to database.
            session.Store(ticket);
            await session.SaveChangesAsync(cancellationToken);

            // return result using Result Pattern.
            return Result<Guid>.Success(ticket.Id);
        }
    }
}
