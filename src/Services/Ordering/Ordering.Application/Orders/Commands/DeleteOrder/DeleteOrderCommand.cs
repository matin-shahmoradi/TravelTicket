using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Application.Orders.Commands.DeleteOrder
{
    public record DeleteOrderCommand(OrderId OrderId) : ICommand<Result<OrderId>>;

    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order Id cannot be empty.");
        }
    }
}
