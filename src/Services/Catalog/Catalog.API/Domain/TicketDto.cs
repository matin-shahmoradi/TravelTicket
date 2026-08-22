namespace Catalog.API.Domain
{
    public sealed record class TicketDto
        (
            Guid Id,
            string Origin,
            string Destination,
            string Description,
            DateTime TravelDate,
            decimal Price
        );
}
