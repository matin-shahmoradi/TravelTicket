namespace Ordering.Application.Orders.Queries.GetOrderById
{
    public record class GetOrderByIdQuery(Guid OrderId) : IQuery<Result<OrderDto>>;

    public sealed class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderByIdQueryValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Order Id can't be empty.");
        }
    }
}
