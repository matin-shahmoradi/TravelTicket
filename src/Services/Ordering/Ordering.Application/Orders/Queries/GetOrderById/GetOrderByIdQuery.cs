namespace Ordering.Application.Orders.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid Id) : IQuery<Result<GetOrderByIdQueryResult>>;
    public record GetOrderByIdQueryResult(OrderDto OrderDto)
    {
        public static GetOrderByIdQueryResult New(OrderDto value) => new GetOrderByIdQueryResult(value);
    }

    public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderByIdQueryValidator()
        {
            RuleFor(i => i.Id).NotEmpty().WithMessage("Order Id cant be Empty.");
        }
    }
}
