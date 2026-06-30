using BuildingBlocks.Abstractions;
using BuildingBlocks.CustomExceptions;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Catalog.API.Tickets.CurrentUser
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
                    : throw new UnAuthorizedException("User Id can't be null");
            }
        }

        public string UserName =>
           _principal?.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
           ?? throw new UnAuthorizedException("Username Id can't be null");

        public string UserEmail =>
            _principal?.FindFirstValue(JwtRegisteredClaimNames.Email)
            ?? throw new UnAuthorizedException("Email User email");

        public string PhoneNumber =>
            _principal?.FindFirstValue(JwtRegisteredClaimNames.PhoneNumber)
            ?? throw new UnAuthorizedException("Phone number User phone number");

        public IReadOnlyCollection<string> Roles =>
            _principal?.FindAll("role").Select(c => c.Value).ToList().AsReadOnly()
            ?? new List<string>().AsReadOnly();

        public bool IsInRole(string role) => _principal?.IsInRole(role) ?? false;
    }
}
