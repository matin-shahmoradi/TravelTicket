using BuildingBlocks.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Basket.API.Services
{
    public class CurrentUser(IHttpContextAccessor httpContextAccessor)
        : ICurrentUser
    {
        private ClaimsPrincipal? _principal =>
            httpContextAccessor.HttpContext?.User;
        public bool IsAuthenticated => _principal?.Identity?.IsAuthenticated ?? false;

        public Guid UserId
        {
            get
            {
                var value = _principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(value, out var userId)
                    ? userId
                    : throw new Exception("Null User id");
            }
        }

        public string UserName =>
           _principal?.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
           ?? throw new Exception("Null Username");

        public string UserEmail =>
            _principal?.FindFirstValue(ClaimTypes.Email)
            ?? throw new Exception("Null User email");

        public string PhoneNumber =>
            _principal?.FindFirstValue(ClaimTypes.MobilePhone)
            ?? throw new Exception("Null User phone number");

        public IReadOnlyCollection<string> Roles =>
            _principal?.FindAll("role").Select(c => c.Value).ToList().AsReadOnly()
            ?? new List<string>().AsReadOnly();

        public bool IsInRole(string role) => _principal?.IsInRole(role) ?? false;
    }
}
