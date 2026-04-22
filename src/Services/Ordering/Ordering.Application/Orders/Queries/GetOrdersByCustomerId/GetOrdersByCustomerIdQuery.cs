namespace Ordering.Application.Orders.Queries.GetOrdersByPhoneNumber
{
    public record GetOrdersByCustomerIdQuery(Guid CustomerId) : IQuery<Result<GetOrderByCustomerIdResult>>;

    public class GetOrdersByCustomerIdQueryValidator : AbstractValidator<GetOrdersByCustomerIdQuery>
    {
        public GetOrdersByCustomerIdQueryValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer Id cant be empty.");
        }
    }
}
