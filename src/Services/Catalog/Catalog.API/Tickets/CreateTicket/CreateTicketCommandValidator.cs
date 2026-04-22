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
                .NotEmpty().WithMessage("Price is required.")
                .GreaterThan(0).WithMessage("Price should be greater then 0.");

            RuleFor(x => x.CreateTicketRequest.Date)
                .NotEmpty().WithMessage("Date cant be empty")
                .NotNull().WithMessage("Date is required.");

            RuleFor(x => x.CreateTicketRequest.TravlerName)
                .NotEmpty().WithMessage("TravlerName is required.")
                .NotNull().WithMessage("TravlerName is required.");

            RuleFor(x => x.CreateTicketRequest.TravlerNumber)
                .NotEmpty().WithMessage("TravlerNumber is required.")
                .NotNull().WithMessage("TravlerNumber is required.");
        }
    }
}
