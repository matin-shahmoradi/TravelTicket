using AuthService.Model.DTOs.RegisterDtos;
using BuildingBlocks;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace AuthService.Auth.Register
{
    public record UserRegisterCommand(RegisterRequestDto Request) : ICommand<Result<RegisterResponseDto>>;
    public class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
    {
        public UserRegisterCommandValidator()
        {
            RuleFor(c => c.Request.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .NotEmpty().WithMessage("Email field can't be empty.");


            RuleFor(c => c.Request.Firstname)
                .NotEmpty().WithMessage("Firstname field can't be empty.");

            RuleFor(c => c.Request.Lastname)
                .NotEmpty().WithMessage("Lastname field can't be empty.");

            RuleFor(c => c.Request.Password)
                .NotEmpty().WithMessage("Password field can't be empty.")
                .MinimumLength(8).WithMessage("Password cant be less than 8 character")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .Matches(@"[\!\?\*\.\@\#]+").WithMessage("Your password must contain at least one (!? *. @#)."); ;
        }
    }
}
