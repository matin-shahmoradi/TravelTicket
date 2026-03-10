namespace Catalog.API.Common.DTOs
{
    public record TicketRequestDTO(
        string Origin,
        string Destination,
        string Description,
        DateTime Date,
        decimal Price,
        string TravlerName,
        string TravlerNumber);
}
