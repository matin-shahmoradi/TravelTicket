using AuthService.Interfaces;
using AuthService.Model.DTOs.UserDtos;
using AutoMapper;
using BuildingBlocks;
using BuildingBlocks.CQRS;

namespace AuthService.Auth.GetUsers
{
    public record GetUsersQuery : IQuery<Result<IEnumerable<UserResponseDto>>>;
    internal sealed class GetUsersQueryHandler(IUserManagerQueryService userQueryService, IMapper mapper)
        : IQueryHandler<GetUsersQuery, Result<IEnumerable<UserResponseDto>>>
    {
        public async Task<Result<IEnumerable<UserResponseDto>>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
        {
            var users = await userQueryService.GetUsersAsync(cancellationToken);
            if (users is null)
                return Result<IEnumerable<UserResponseDto>>.Failure(Error.NotFoundError(message: "Users not Found!"));

            var usersDto = mapper.Map<List<UserResponseDto>>(users);
            return Result<IEnumerable<UserResponseDto>>.Success(usersDto);
        }
    }
}
