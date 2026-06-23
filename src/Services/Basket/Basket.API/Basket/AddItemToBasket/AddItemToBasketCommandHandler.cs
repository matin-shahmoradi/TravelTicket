using Basket.API.Common.Dtos.MapExtensions;
using Basket.API.Data.Repositories;
using Basket.API.Grpc;
using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Basket.AddItemToBasket
{
    internal sealed class AddItemToBasketCommandHandler(
        BacketDbContext basketContext,
        IBasketRepository basketRepository,
        IDistributedCache distributedCache,
        ICacheTicketRepository cache,
        ICurrentUser currentUser,
        ICatalogGrpcClient grpcClient,
        ILogger<AddItemToBasketCommandHandler> logger
        )
        : ICommandHandler<AddItemToBasketCommand, Result<ShoppingCartDto>>
    {
        public async Task<Result<ShoppingCartDto>> Handle(AddItemToBasketCommand command, CancellationToken cancellationToken)
        {
            var basket = await basketContext.ShoppingCarts
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.CustomerId == currentUser.UserId, cancellationToken);

            if (basket is null)
            {
                basket = ShoppingCart.Create(currentUser.UserId!);
                await basketRepository.StoreBasket(basket);
            }

            var ticketResult = await GetTicketWithFallbackAsync(command.AddItemToBasketDto.TicketId, cancellationToken);
            if (!ticketResult.IsSuccess)
            {
                if (!ticketResult.IsSuccess)
                    return Result<ShoppingCartDto>.Failure(ticketResult.Error!.Value);
            }
            var ticket = ticketResult.Value;

            basket.AddItem(
                ticketId: ticket!.TicketId,
                quantity: command.AddItemToBasketDto.Quantity,
                price: ticket.Price);

            await basketContext.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            await distributedCache.RemoveAsync(currentUser.UserId.ToString()!);

            // Set new cache
            await distributedCache.SetStringAsync(
                key: currentUser.UserId.ToString()!,
                value: JsonSerializer.Serialize(basket.MapToShoppingCartDto())
                );
            return Result<ShoppingCartDto>.Success(basket.MapToShoppingCartDto());
        }
        private async Task<Result<TicketReadModel>> GetTicketWithFallbackAsync(Guid ticketId, CancellationToken cancellationToken)
        {
            var cachedItem = await cache.ReadTicketFromCacheAsync(ticketId.ToString(), cancellationToken);

            if (cachedItem is not null)
            {
                var ticket = JsonSerializer.Deserialize<TicketReadModel>(cachedItem);

                if (ticket is null)
                {
                    logger.LogError("Failed to deserialize cached ticket. TicketId: {TicketId}", ticketId);
                    return Result<TicketReadModel>.Failure(
                        Error.Internal_Server("Failed to deserialize cached ticket"));
                }

                return Result<TicketReadModel>.Success(ticket);
            }

            logger.LogInformation("Cache miss for ticket {TicketId}. Calling Catalog gRPC.", ticketId);

            var catalogTicket = await GetTicketFromCatalogService(ticketId);
            if (catalogTicket is null)
            {
                logger.LogWarning("Ticket not found in catalog. TicketId: {TicketId}", ticketId);

                return Result<TicketReadModel>.Failure(Error.NotFoundError("Ticket not found"));
            }
            await cache.StoreTicketInCacheAsync(catalogTicket, cancellationToken);
            return Result<TicketReadModel>.Success(catalogTicket);
        }

        private async Task<TicketReadModel?> GetTicketFromCatalogService(Guid ticketId)
        {
            var ticketFromCatalog = await grpcClient.GetTicketByIdAsync(ticketId.ToString()) ?? null;
            return ticketFromCatalog;
        }
    }
}
