namespace Ordering.Application.Orders.Commands.UpdateOrder
{
    public record UpdateOrderCommand(UpdateOrderStatusDto UpdateOrderDto) : ICommand<Result<UpdateOrderStatusDto>>;

    public class UpdateOrderValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderValidator()
        {
            RuleFor(x => x.UpdateOrderDto.OrderId)
                .NotEmpty().WithMessage("Order cant be empty.");
        }
    }
}
