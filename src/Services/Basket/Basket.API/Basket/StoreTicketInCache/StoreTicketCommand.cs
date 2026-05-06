namespace Basket.API.Basket.StoreTicket
{
    public record StoreTicketCommand(TicketReadModel Ticket) : ICommand<Result<Unit>>
    {
    }
}
