using Basket.API.basket.BasketCheckout;
using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Basket.API.basket.Checkout
{
    public class BasketCheckOutCommandHandler(
        ICurrentUser currentUser,
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<BasketCheckOutCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(BasketCheckOutCommand command, CancellationToken cancellationToken)
        {
            var getUserBasket = await basketRepository
                .GetBasket(currentUser.UserId, QueryTrackingBehavior.TrackAll, cancellationToken);

            if (getUserBasket is null)
                return Result<bool>.Failure(Error.NotFoundError("basket Not Found!"));

            getUserBasket.CheckoutBasket();

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
