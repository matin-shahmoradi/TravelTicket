using BuildingBlocks;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace AuthService.Auth.ConfirmEmail
{
    public record ConfirmEmailCommand(string UserId, string EmailConfirmationToken) : ICommand<Result<string>>;

    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User id can't be empty");

            RuleFor(x => x.EmailConfirmationToken)
                .NotEmpty().WithMessage("token can't be empty");
        }
    }
}
