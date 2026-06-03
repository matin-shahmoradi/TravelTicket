using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Model.DTOs.LoginDtos;
using BuildingBlocks;
using BuildingBlocks.CQRS;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Auth.Login
{
    internal sealed class LoginCommandHandler(
        IUserManagerQueryService userQueryService,
        IUserRoleQueryService userRoleQueryService,
        IUserSignInManagerService userSignInManager,
        IConfiguration _configuration
        )
        : ICommandHandler<LoginCommand, Result<LoginResponse>>
    {
        public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await userQueryService.GetUserByEmailAsync(command.Request.Email, cancellationToken);
            if (user is null)
                return Result<LoginResponse>.Failure(Error.UnAuthorized(message: "invalid username or password"));

            var result = await userSignInManager.ValidateUserPassword(user, command.Request.Password);

            if (!result)
                return Result<LoginResponse>.Failure(Error.UnAuthorized(message: "invalid username or password"));

            var roles = await userRoleQueryService.GetUserRolesAsync(user.Id);
            var token = GenerateJwtToken(user, roles);

            return Result<LoginResponse>.Success(new LoginResponse(
                email: user.Email!,
                username: user.UserName!,
                token: token)
                );
        }
        private string GenerateJwtToken(ApplicationUser user, IReadOnlyList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Email,user.Email!),
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                  issuer: _configuration["Jwt:Issuer"],
                  audience: _configuration["Jwt:Audience"],
                  claims: claims,
                  expires: DateTime.UtcNow.AddHours(24),
                  signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
