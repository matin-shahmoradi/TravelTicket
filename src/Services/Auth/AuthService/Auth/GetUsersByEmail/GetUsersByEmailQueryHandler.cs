using AuthService.Interfaces;
using AuthService.Model.DTOs.UserDtos;
using BuildingBlocks;
using BuildingBlocks.CQRS;

namespace AuthService.Auth.GetUsersByEmail
{
    public class GetUsersByEmailQueryHandler(IUserQueryService userQueryService)
        : IQueryHandler<GetUserByEmailQuery, Result<UserResponseDto>>
    {
        public async Task<Result<UserResponseDto>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
        {
            var getUser = await userQueryService.GetUsersByEmailAsync(query.Email, cancellationToken);

            if (getUser == null)
                return Result<UserResponseDto>.Failure(
                    Error.NotFoundError(message:$"User with email {query.Email} is not exist"));

            return Result<UserResponseDto>.Success(getUser);
        }
    }
}
