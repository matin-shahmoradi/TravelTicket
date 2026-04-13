namespace Ordering.Application.Orders.Commands.UpdateOrder
{
    public record UpdateOrderCommand(OrderDto OrderDto) : ICommand<Result<OrderDto>>;

    public class UpdateOrderValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderValidator()
        {
            RuleFor(x => x.OrderDto.OrderName).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.OrderDto.CustomerId).NotNull().WithMessage("CustomerId is required.");
            RuleFor(x => x.OrderDto.OrderItems).NotEmpty().WithMessage("Order Items should not be empty.");
        }
    }
}
