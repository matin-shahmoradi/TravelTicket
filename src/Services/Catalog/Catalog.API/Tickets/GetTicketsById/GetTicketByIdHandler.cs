using Catalog.API.Repository;

namespace Catalog.API.Tickets.GetTicketsById
{
    public record GetTicketByIdQuery(Guid Id) : IQuery<Result<Ticket>>;
    public sealed class GetTicketByIdQueryValidator : AbstractValidator<GetTicketByIdQuery>
    {
        public GetTicketByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Ticket id is required")
                .NotEqual(Guid.Empty).WithMessage("Invalid guid");
        }
    }
    internal sealed class GetTicketByIdQueryHandler(ITicketQueryRepository queryRepository)
        : IQueryHandler<GetTicketByIdQuery, Result<Ticket>>
    {
        public async Task<Result<Ticket>> Handle(GetTicketByIdQuery query, CancellationToken cancellationToken)
        {
            var ticket = await queryRepository.GetTicketByIdWithNoTracking(
                id: query.Id,
                cancellationToken);

            if (ticket is null)
            {
                return Result<Ticket>.Failure(Error.NotFoundError(message: $"Ticket {query.Id} Not Found!"));
            }
            return Result<Ticket>.Success(ticket);
        }
    }
}
