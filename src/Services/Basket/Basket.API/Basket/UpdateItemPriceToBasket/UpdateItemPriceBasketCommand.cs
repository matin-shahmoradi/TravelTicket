namespace Basket.API.Basket.UpdateItemPriceInBasket
{
    public record UpdateItemPriceBasketCommand(Guid TicketId, decimal Price) : ICommand<Result<bool>>;

    public class UpdateItemPriceBasketCommandValidator : AbstractValidator<UpdateItemPriceBasketCommand>
    {
        public UpdateItemPriceBasketCommandValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty().WithMessage("ticket id cant be empty.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("ticket price cant be less than zero");
        }
    }
}
