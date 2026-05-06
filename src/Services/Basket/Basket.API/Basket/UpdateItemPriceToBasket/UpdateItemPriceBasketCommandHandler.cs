using Microsoft.EntityFrameworkCore;

namespace Basket.API.Basket.UpdateItemPriceInBasket
{
    public class UpdateItemPriceBasketCommandHandler(BacketDbContext basketDb) 
        : ICommandHandler<UpdateItemPriceBasketCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateItemPriceBasketCommand command, CancellationToken cancellationToken)
        {
            var getItemToUpdate = await basketDb
                .ShoppingCartItems
                .Where(t => t.TicketId == command.TicketId)
                .ToListAsync(cancellationToken);

            if (!getItemToUpdate.Any())
            {
                return Result<bool>.Failure(
                    Error.NotFoundError(
                        message:$"Ticket with ticket id {command.TicketId} Not Found!"));
            }

            foreach(var item in getItemToUpdate)
            {
                item.UpdatePrice(command.Price);
            }
            await basketDb.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
