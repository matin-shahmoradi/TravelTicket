using BuildingBlocks.Extensions;

namespace Catalog.API.Tickets.DeleteTicket
{
    public class DeleteTicketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("tickets/{id}", async (
                HttpContext context,
                ISender sender,
                Guid id) =>
            {
                var result = await sender.Send(new DeleteTicketCommand(id));
                if (result.IsSuccess)
                    return Results.NoContent();

                return result.ToHttpResult(context);
            })
                .WithName("DeleteTickets")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithSummary("this endpoint used for Delete tickets")
                .RequireAuthorization("AdminOnly");
        }
    }
}
