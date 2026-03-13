namespace Basket.API.Basket.DeleteBasket
{
    public class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
    {
        public DeleteBasketCommandValidator()
        {
            RuleFor(x =>x.TravlerNumber)
                .NotEmpty().WithMessage("Number is required.");
        }
    }
}
