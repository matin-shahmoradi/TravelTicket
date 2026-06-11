using AuthService.Model.DTOs.LoginDtos;
using BuildingBlocks;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace AuthService.Auth.Login
{
    public record LoginCommand(LoginRequest Request) : ICommand<Result<LoginResponse>>;

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Request.Email)
                .NotEmpty().WithMessage("Email field cant be empty")
                .EmailAddress().WithMessage("Invalid Email format!");

            RuleFor(x => x.Request.Password)
                .NotEmpty().WithMessage("Password field cant be empty.")
                .MinimumLength(8).WithMessage("Password cant be less than 8 character")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .Matches(@"[\!\?\*\.\@\#]+").WithMessage("Your password must contain at least one (!? *. @#).");
        }
    }
}
