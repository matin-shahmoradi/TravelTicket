namespace Catalog.API.Tickets.DeleteTicket
{
    public class DeleteTicketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("tickets/{id}", async (ISender sender, Guid id) =>
            {
                var ticket = await sender.Send(new DeleteTicketCommand(id));
                if (ticket.IsSuccess)
                    Results.NoContent();

                Results.Problem();
            })
                .WithName("DeleteTickets")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithSummary("this endpoint used for Delete ticket")
                .RequireAuthorization("AdminOnly");
        }
    }
}
