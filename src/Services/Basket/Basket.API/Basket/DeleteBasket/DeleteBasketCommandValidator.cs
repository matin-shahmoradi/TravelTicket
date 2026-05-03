namespace Basket.API.Basket.DeleteBasket
{
    public class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
    {
        public DeleteBasketCommandValidator()
        {
            RuleFor(x =>x.Username)
                .NotEmpty().WithMessage("username cant be empty.");
        }
    }
}
