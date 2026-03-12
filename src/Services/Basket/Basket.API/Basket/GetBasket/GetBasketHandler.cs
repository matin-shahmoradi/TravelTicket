namespace Basket.API.Basket.GetBasket
{
    public record GetBasketQuery(string travlerNumber) : IQuery<Result<ShoppingCart>>;
    internal class GetBasketQueryHandler(IBasketRepository basketRepository)
        : IQueryHandler<GetBasketQuery, Result<ShoppingCart>>
    {
        public async Task<Result<ShoppingCart>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
        {
            var basket = await basketRepository.GetBasket(request.travlerNumber);
            if (basket is null)
            {
                return Result<ShoppingCart>.Failure
                    (Error.NotFoundError(message: "Basket not found!"));
            }

            return Result<ShoppingCart>.Success(basket);
        }
    }
}
