namespace Catalog.API.Tickets.CreateTicket
{
    public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketCommandValidator()
        {
            RuleFor(x => x.CreateTicketRequest.Origin)
                .NotEmpty().WithMessage("Origin is required.")
                .NotNull().WithMessage("Origin is required.");

            RuleFor(x => x.CreateTicketRequest.Destination)
                .NotEmpty().WithMessage("Destination is required.")
                .NotNull().WithMessage("Destination is required.");

            RuleFor(x => x.CreateTicketRequest.Price)
                .GreaterThan(0).WithMessage("Price should be greater then 0.");

            RuleFor(x => x.CreateTicketRequest.Date)
                .NotEmpty().WithMessage("Date cant be empty")
                .NotNull().WithMessage("Date is required.");

            RuleFor(x => x.CreateTicketRequest.Description)
                .NotEmpty().WithMessage("Description is required")
                .NotNull().WithMessage("Description is required");
        }
    }
}
