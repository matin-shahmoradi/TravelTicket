using FluentValidation;

namespace Basket.API.Basket.StoreBasket
{
    public class StoreBasketCommandValidator: AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketCommandValidator()
        {
            RuleFor(x => x.BasketDto.Items)
                .NotNull().WithMessage("Shopping cart items cant be null.");
        }
    }
}
