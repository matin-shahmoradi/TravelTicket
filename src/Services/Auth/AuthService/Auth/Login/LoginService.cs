using AuthService.Model;
using AuthService.Model.DTOs.LoginDtos;
using BuildingBlocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Auth.Login
{
    // TODO : refactoring the Login service based on CQRS and MediateR
    public class LoginService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public LoginService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<Result<LoginResponse>> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<LoginResponse>.Failure(Error.UnAuthorized(message: "invalid username or password"));

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (!result.Succeeded)
                Result<string>.Failure(Error.UnAuthorized(message: "invalid username or password"));

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user , roles);

            return Result<LoginResponse>.Success(new LoginResponse(
                email: user.Email!,
                username: user.UserName!,
                token: token)
                );
        }

        private string GenerateJwtToken(ApplicationUser user , IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email!),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role,role)));

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
