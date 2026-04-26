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
               .Produces<Result<Ticket>>(StatusCodes.Status200OK)
               .ProducesProblem(StatusCodes.Status400BadRequest)
               .ProducesProblem(StatusCodes.Status404NotFound)
               .ProducesProblem(StatusCodes.Status500InternalServerError)
               .WithSummary("this endpoint used for receive ticket with unique Id");
        }
    }
}
