using Catalog.API.Domain.ValueObjects;

namespace Catalog.API.Tickets.DeleteTicket
{
    public record DeleteTicketCommand(Guid Id) : ICommand<Result<bool>>;
    internal sealed class DeleteTicketCommandHandler(ICatalogDbContext catalogDb)
        : ICommandHandler<DeleteTicketCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteTicketCommand command, CancellationToken cancellationToken)
        {
            var existTicket = await catalogDb.Tickets.FindAsync(TicketId.New(command.Id),cancellationToken);
            if (existTicket is null)
            {
                return Result<bool>.Failure(Error.NotFoundError(message:"Ticket Not Found!"));
            }

            catalogDb.Tickets.Remove(existTicket);
            await catalogDb.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
