using Basket.API.Common.Dtos.MapExtensions;
using BuildingBlocks.Abstractions;

namespace Basket.API.Basket.GetBasket
{
    public record GetBasketQuery : IQuery<Result<ShoppingCartDto>>;
    internal class GetBasketQueryHandler(
        IBasketRepository basketRepository,
        ICurrentUser currentUser)
        : IQueryHandler<GetBasketQuery, Result<ShoppingCartDto>>
    {
        public async Task<Result<ShoppingCartDto>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
        {
            var basket = await basketRepository.GetBasket(currentUser.UserId);
            if (basket is null)
            {
                return Result<ShoppingCartDto>.Failure
                    (Error.NotFoundError(message: "Basket not found!"));
            }
            var dto = basket.MapToShoppingCartDto();
            return Result<ShoppingCartDto>.Success(dto);
        }
    }
}
