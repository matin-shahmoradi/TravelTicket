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
                .NotEmpty().WithMessage("Password field can't be empty.");
        }
    }
}
