using BuildingBlocks.Extensions;
using BuildingBlocks.Pagination;

namespace Catalog.API.Tickets.GetTickets
{
    public record GetTicketRequest(PageRequest PageRequest);
    public class GetTicketsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("tickets", async (
                HttpContext context,
                ISender sender,
               [FromQuery] int pageNumber,
               [FromQuery] int pageSize) =>
            {
                var result = await sender.Send(new GetTicketQuery(pageNumber, pageSize));
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return result.ToHttpResult(context);
            })
              .Produces(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status404NotFound)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("this endpoint used for receive tickets");
        }
    }
}
