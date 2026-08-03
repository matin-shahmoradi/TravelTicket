namespace Catalog.API.Tickets.UpdateTicket
{
    public class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
    {
        public UpdateTicketCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Ticket id is required");

            RuleFor(x => x.UpdateTicketRequest.Origin)
                .NotEmpty().WithMessage("Origin is required.")
                .When(x => x.UpdateTicketRequest.Origin is not null);

            RuleFor(x => x.UpdateTicketRequest.Destination)
                .NotEmpty().WithMessage("Destination is required.")
                .When(x => x.UpdateTicketRequest.Destination is not null);

            RuleFor(x => x.UpdateTicketRequest.Description)
                .NotEmpty().WithMessage("description is required")
                .When(x => x.UpdateTicketRequest.Description is not null);

            RuleFor(x => x.UpdateTicketRequest.Price)
                .NotEmpty().WithMessage("Price is required.")
                .GreaterThan(0).WithMessage("Price should be greater then 0.");

            RuleFor(x => x.UpdateTicketRequest.Date)
                .NotEmpty().WithMessage("Date cant be empty")
                .When(x => x.UpdateTicketRequest.Date is not null);
        }
    }
}
