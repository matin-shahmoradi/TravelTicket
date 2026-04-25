using FluentValidation;

namespace Catalog.API.Tickets.UpdateTicket
{
    public class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
    {
        public UpdateTicketCommandValidator()
        {
            RuleFor(x => x.UpdateTicketRequest.Origin)
                .NotEmpty().WithMessage("Origin is required.")
                .NotNull().WithMessage("Origin is required.");

            RuleFor(x => x.UpdateTicketRequest.Destination)
                .NotEmpty().WithMessage("Destination is required.")
                .NotNull().WithMessage("Destination is required.");

            RuleFor(x => x.UpdateTicketRequest.Price)
                .NotEmpty().WithMessage("Price is required.")
                .GreaterThan(0).WithMessage("Price should be greater then 0.");

            RuleFor(x => x.UpdateTicketRequest.Date)
                .NotEmpty().WithMessage("Date cant be empty")
                .NotNull().WithMessage("Date is required.");

        }
    }
}
