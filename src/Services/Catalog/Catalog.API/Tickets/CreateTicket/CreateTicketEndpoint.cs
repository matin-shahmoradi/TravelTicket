using Catalog.API.CatalogExtensions;


namespace Catalog.API.Tickets.CreateTicket
{
    public class CreateTicketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/tickets" , async ([FromBody] TicketRequestDTO createRequest, ISender sender) =>
            {
                var requestResult = await sender.Send(new CreateTicketCommand(createRequest));
                if (requestResult.IsSuccess)
                {
                    return Results.Created($"/tickets/{requestResult.Value}", requestResult.Value);
                }

                return requestResult.ToHttpResult();
            })
                .WithName("CreateTicket")
                .Produces(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("this endpoint used for Create Ticket");
        }
    }
}
