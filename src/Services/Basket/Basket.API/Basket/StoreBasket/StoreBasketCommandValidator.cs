namespace Basket.API.Basket.StoreBasket
{
    public class StoreBasketCommandValidator: AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketCommandValidator()
        {
            RuleFor(x => x.BasketDto.Username)
                .NotEmpty().WithMessage("Shopping cart username cant be empty.");

            RuleFor(x => x.BasketDto.Quantity)
                .NotEmpty().WithMessage("Shopping cart quantity cant be empty.")
                .GreaterThan(0).WithMessage("Shopping cart price should be greater than zero");

            RuleFor(x => x.BasketDto.TicketId)
                .NotEmpty().WithMessage("Shopping cart ticketId cant be empty.");
        }
    }
}
