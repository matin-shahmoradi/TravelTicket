using BuildingBlocks.Abstractions;
using Catalog.API.Repository;

namespace Catalog.API.Tickets.UpdateTicket
{
    public record UpdateTicketRequestDTO(
        string? Origin,
        string? Destination,
        string? Description,
        DateTime? Date,
        decimal? Price);
    public record UpdateTicketCommand(Guid Id, UpdateTicketRequestDTO UpdateTicketRequest) : ICommand<Result<Ticket>>;
    internal sealed class UpdateTicketCommandHandler(
        ITicketQueryRepository queryRepository,
        ITicketCommandRepository commandRepository,
        IUnitOfWork unitOfWork
        )
        : ICommandHandler<UpdateTicketCommand, Result<Ticket>>
    {
        public async Task<Result<Ticket>> Handle(UpdateTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await queryRepository.GetTicketById(command.Id);

            if (ticket is null)
            {
                return Result<Ticket>.Failure(Error.NotFoundError(message: "Ticket Not Found!"));
            }

            var request = command.UpdateTicketRequest;

            if (request.Origin is not null)
            {
                ticket.ChangeOrigin(request.Origin);
            }

            if (request.Destination is not null)
            {
                ticket.ChangeDestination(request.Destination);
            }

            if (request.Description is not null)
            {
                ticket.ChangeDescription(request.Description);
            }

            if (request.Price.HasValue)
            {
                ticket.ChangePrice(request.Price.Value);
            }

            if (request.Date.HasValue)
            {
                ticket.ChangeTravelDate(request.Date.Value);
            }

            try
            {
                commandRepository.UpdateTicket(ticket);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<Ticket>.Success(ticket);
            }
            catch (Exception ex)
            {
                return Result<Ticket>.Failure(Error.Internal_Server(message: ex.Message));
            }
        }
    }
}
