using Basket.API.Data.Repositories;

namespace Basket.API.Basket.StoreTicket
{
    public class StoreTicketCommandHandler(ICacheTicketRepository cacheTicketRepository)
        : ICommandHandler<StoreTicketCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(StoreTicketCommand command, CancellationToken cancellationToken)
        {
            await cacheTicketRepository.StoreTicketInCacheAsync(command.Ticket, cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
