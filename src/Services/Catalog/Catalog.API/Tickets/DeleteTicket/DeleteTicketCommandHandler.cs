namespace Catalog.API.Tickets.DeleteTicket
{
    public record DeleteTicketCommand(Guid Id) : ICommand<Result<bool>>;
    internal sealed class DeleteTicketCommandHandler(IDocumentSession session)
        : ICommandHandler<DeleteTicketCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteTicketCommand command, CancellationToken cancellationToken)
        {
            var existTicket = await session.LoadAsync<Ticket>(command.Id, cancellationToken);
            if (existTicket is null)
            {
                return Result<bool>.Failure(Error.NotFoundError(message:"Ticket Not Found!"));
            }

            session.Delete(existTicket);
            await session.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}
