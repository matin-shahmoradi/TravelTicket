using BuildingBlocks.Extensions;

namespace Catalog.API.Tickets.CreateTicket
{
    public class CreateTicketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/tickets", async (
                ISender sender,
                HttpContext context,
                [FromBody] TicketRequestDTO createRequest) =>
            {
                var requestResult = await sender.Send(new CreateTicketCommand(createRequest));
                if (requestResult.IsSuccess)
                {
                    return Results.Created($"/tickets/{requestResult.Value}", requestResult.Value);
                }

                return requestResult.ToHttpResult(context);
            })
                .WithName("CreateTicket")
                .Produces(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("this endpoint used for Create Ticket")
                .RequireAuthorization("AdminOnly");
        }
    }
}
