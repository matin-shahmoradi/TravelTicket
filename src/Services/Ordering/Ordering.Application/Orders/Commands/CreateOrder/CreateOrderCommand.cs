namespace Ordering.Application.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(OrderDto OrderDto) : ICommand<Result<Guid>>;

    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.OrderDto.OrderItems).NotEmpty().WithMessage("Order Items should not be empty.");
        }
    }
}
