using Basket.API.Common.Dtos.MapExtensions;
using Basket.API.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Basket.AddItemToBasket
{
    public class AddItemToBasketCommandHandler(
        BacketDbContext basketContext,
        IDistributedCache distributedCache,
        ICacheTicketRepository cache
        )
        : ICommandHandler<AddItemToBasketCommand, Result<ShoppingCartDto>>
    {
        public async Task<Result<ShoppingCartDto>> Handle(AddItemToBasketCommand command, CancellationToken cancellationToken)
        {
            //var basket = await basketRepository.GetBasket(command.AddItemToBasketDto.Username ,cancellationToken);   
            var basket = await basketContext.ShoppingCarts
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Username == command.AddItemToBasketDto.Username,cancellationToken);

            if (basket is null)
                return Result<ShoppingCartDto>.Failure(Error.NotFoundError(message: "basket not found"));


            var cachedItem = await cache.ReadTicketFromCacheAsync(
                command.AddItemToBasketDto.TicketId.ToString(),
                cancellationToken);

            if(cachedItem is null)
                return Result<ShoppingCartDto>.Failure(Error.NotFoundError(message: "ticket cache missed"));

            var sterilizedItem = JsonSerializer.Deserialize<TicketReadModel>(cachedItem);

            if(sterilizedItem is null)
                return Result<ShoppingCartDto>.Failure(Error.CustomError(mesaage: "failed to deserialize ticket"));

            basket.AddItem(
                ticketId: sterilizedItem!.TicketId,
                quantity: command.AddItemToBasketDto.Quantity,
                price: sterilizedItem.Price);

            await basketContext.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            await distributedCache.RemoveAsync(command.AddItemToBasketDto.Username);

            // Set new cache
            await distributedCache.SetStringAsync(
                key:command.AddItemToBasketDto.Username,
                value:JsonSerializer.Serialize(basket.MapToShoppingCartDto())
                );
            return Result<ShoppingCartDto>.Success(basket.MapToShoppingCartDto());
            

            // TODO: Implement fallback strategy when cache miss
            // - call Ticket API
            // - cache result
            // - then add to basket

            //return Result<ShoppingCartDto>.Failure(Error.NotFoundError());
        }
    }
}
