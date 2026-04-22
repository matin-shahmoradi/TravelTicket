namespace Ordering.Application.Orders.Queries.GetOrdersByPhoneNumber
{
    public record GetOrdersByPhoneNumberQuery(string phoneNumber) : IQuery<Result<GetOrdersByPhoneNumberResult>>;


    public record GetOrdersByPhoneNumberResult(IEnumerable<OrderDto> orderDtos)
    {
        public static GetOrdersByPhoneNumberResult New(IEnumerable<OrderDto> value) => new GetOrdersByPhoneNumberResult(value);
    }

    public class GetOrdersByPhoneNumberQueryValidator : AbstractValidator<GetOrdersByPhoneNumberQuery>
    {
        public GetOrdersByPhoneNumberQueryValidator()
        {
            RuleFor(p => p.phoneNumber).NotEmpty().WithMessage("Phone Number cant be Empty.");
        }
    }
}
