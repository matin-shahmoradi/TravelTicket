using AuthService.Interfaces;
using AuthService.Model.DTOs.UserDtos;
using AutoMapper;
using BuildingBlocks;
using BuildingBlocks.CQRS;

namespace AuthService.Auth.GetUsersByEmail
{
    internal sealed class GetUsersByEmailQueryHandler(
        IUserManagerQueryService userQueryService,
        IMapper mapper
        )
        : IQueryHandler<GetUserByEmailQuery, Result<UserResponseDto>>
    {
        public async Task<Result<UserResponseDto>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
        {
            var getUser = await userQueryService.GetUserByEmailAsync(query.Email, cancellationToken);

            if (getUser == null)
                return Result<UserResponseDto>.Failure(
                    Error.NotFoundError(message: $"User with email {query.Email} is not exist"));

            var userResponseDto = mapper.Map<UserResponseDto>(getUser);

            return Result<UserResponseDto>.Success(userResponseDto);
        }
    }
}
