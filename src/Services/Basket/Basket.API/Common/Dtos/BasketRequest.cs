namespace Basket.API.Common.Dtos
{
    public record BasketRequest(Guid TicketId,int Quantity,int Price, string Username);

}
