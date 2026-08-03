using BuildingBlocks.Abstractions;
using Catalog.API.Domain.ValueObjects;
using Catalog.API.Repository;

namespace Catalog.API.Tickets.CreateTicket
{
    public record CreateTicketRequestDTO(
        string Origin,
        string Destination,
        string Description,
        DateTime Date,
        decimal Price);
    public record CreateTicketCommand(CreateTicketRequestDTO CreateTicketRequest) : ICommand<Result<Guid>>;
    public sealed class CreateTicketCommandHandler(
        ITicketCommandRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateTicketCommandHandler> logger)
        : ICommandHandler<CreateTicketCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            // Create new Ticket object.
            var ticket = Ticket.Create(
                id: TicketId.New(),
                origin: request.CreateTicketRequest.Origin,
                destination: request.CreateTicketRequest.Destination,
                description: request.CreateTicketRequest.Description,
                travelDate: request.CreateTicketRequest.Date,
                price: request.CreateTicketRequest.Price
                );

            // Save to database.
            try
            {
                await repository.AddTicketAsync(ticket, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Ticket {id} Created successfully", ticket.Id.Value);
                return Result<Guid>.Success(ticket.Id.Value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create ticket {id}", ticket.Id.Value);
                return Result<Guid>.Failure(Error.Internal_Server(ex.Message));
            }
        }
    }
}
