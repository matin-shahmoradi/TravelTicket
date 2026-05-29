using AuthService.Interfaces;
using AuthService.Model.DTOs.UserDtos;
using BuildingBlocks;
using BuildingBlocks.CQRS;

namespace AuthService.Auth.GetUsers
{
    public record GetUsersQuery : IQuery<Result<IEnumerable<UserResponseDto>>>;
    public class GetUsersQueryHandler(IUserQueryService userQueryService) 
        : IQueryHandler<GetUsersQuery, Result<IEnumerable<UserResponseDto>>>
    {
        public async Task<Result<IEnumerable<UserResponseDto>>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
        {
            var users = await userQueryService.GetUsersAsync(cancellationToken);
            if (users is null)
                return Result<IEnumerable<UserResponseDto>>.Failure(Error.NotFoundError(message:"Users not Found!"));

            return Result<IEnumerable<UserResponseDto>>.Success(users);
        }
    }
}
