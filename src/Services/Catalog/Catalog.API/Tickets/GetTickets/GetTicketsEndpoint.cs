namespace Catalog.API.Tickets.GetTickets
{
    public record GetProductRequest(int PageNumber = 1 , int PageSize = 10);
    public class GetTicketsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("tickets", async ([AsParameters]GetProductRequest request,ISender sender ) =>
            {
                var query = await sender.Send(new GetTicketQuery(request));
                if (query.IsSuccess)
                {
                    return Results.Ok(query);
                }
                return Results.Problem();
            })
              .Produces(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status404NotFound)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("this endpoint used for receive tickets");      
        }
    }
}
