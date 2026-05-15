namespace Basket.API.Common.Dtos
{
    public record AddItemToBasketDto(Guid TicketId, string Username, int Quantity);
}
