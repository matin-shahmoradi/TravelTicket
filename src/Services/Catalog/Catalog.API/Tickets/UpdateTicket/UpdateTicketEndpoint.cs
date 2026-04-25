namespace Catalog.API.Tickets.UpdateTicket
{
    public class UpdateTicketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("tickets/{id:guid}", async (Guid id, TicketRequestDTO ticketRequest, ISender sender) =>
            {
                var result = await sender.Send(new UpdateTicketCommand(id, ticketRequest));
                if (result.IsSuccess)
                {
                    return Results.NoContent();
                }
                return Results.Problem();
            })
                .WithName("UpdateTicket")
                .Produces<Result<Ticket>>(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithSummary("this endpoint used for update tickets");     
        }
    }
}
