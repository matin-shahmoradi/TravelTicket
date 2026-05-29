using AuthService.Model.DTOs.UserDtos;
using BuildingBlocks;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace AuthService.Auth.GetUsersByEmail
{
    public record GetUserByEmailQuery(string Email) : IQuery<Result<UserResponseDto>>
    {
    }
    public class GetUserByEmailQueryValidator : AbstractValidator<GetUserByEmailQuery>
    {
        public GetUserByEmailQueryValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email can't be empty.");
        }
    }
}
