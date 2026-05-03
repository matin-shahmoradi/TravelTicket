using Basket.API.Common.Dtos.MapExtensions;
using Basket.API.Data.Repository;

namespace Basket.API.Basket.GetBasket
{
    public record GetBasketQuery(string username) : IQuery<Result<ShoppingCartDto>>;
    internal class GetBasketQueryHandler(IBasketRepository basketRepository)
        : IQueryHandler<GetBasketQuery, Result<ShoppingCartDto>>
    {
        public async Task<Result<ShoppingCartDto>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
        {
            var basket = await basketRepository.GetBasket(request.username);
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
