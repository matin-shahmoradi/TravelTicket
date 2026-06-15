namespace Basket.API.Basket.AddItemToBasket
{
    public record AddItemToBasketCommand(AddItemToBasketDto AddItemToBasketDto) : ICommand<Result<ShoppingCartDto>>
    {
    }

    public class AddItemToBasketCommandValidator : AbstractValidator<AddItemToBasketCommand>
    {
        public AddItemToBasketCommandValidator()
        {
            RuleFor(x => x.AddItemToBasketDto.Quantity)
                .GreaterThan(0).WithMessage("Quantity should be greater than zero");

            RuleFor(x => x.AddItemToBasketDto.TicketId)
                .NotEmpty().WithMessage("ticket id cant be empty.");
        }
    }
}
