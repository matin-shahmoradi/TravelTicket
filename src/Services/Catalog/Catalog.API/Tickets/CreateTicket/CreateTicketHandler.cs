using Catalog.API.Domain.ValueObjects;

namespace Catalog.API.Tickets.CreateTicket
{
    public record CreateTicketCommand(TicketRequestDTO CreateTicketRequest) : ICommand<Result<Guid>>;
    public sealed class CreateTicketCommandHandler(IDocumentSession session) : ICommandHandler<CreateTicketCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            // Creat new Ticket object.
            var ticket = Ticket.Create(
                id: TicketId.New(),
                origin: request.CreateTicketRequest.Origin,
                destination: request.CreateTicketRequest.Destination,
                description: request.CreateTicketRequest.Description,
                travelDate: request.CreateTicketRequest.Date,
                price: request.CreateTicketRequest.Price
                );

            // Save to database.
            session.Store(ticket);
            await session.SaveChangesAsync(cancellationToken);

            // return result using Result Pattern.
            return Result<Guid>.Success(ticket.Id.Value);
        }
    }
}
