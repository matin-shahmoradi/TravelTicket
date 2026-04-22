namespace Catalog.API.Tickets.GetTicketsById
{
    public class GetTicketByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("tickets/{id:guid}", async (ISender sender, Guid id) =>
            {
                var result = await sender.Send(new GetTicketByIdQuery(id));

                if (result.IsSuccess)
                    return Results.Ok(result);

                if (result.Error!.Value.ErrorType == ErrorType.NOT_FOUND)
                    return Results.NotFound();

                return Results.Problem();
            })
               .WithName("UpdateTickets")
               .Produces<Result<Ticket>>(200)
               .ProducesProblem(404)
               .ProducesProblem(500)
               .WithSummary("this endpoint used for receive ticket with unique Id");
        }
    }
}
