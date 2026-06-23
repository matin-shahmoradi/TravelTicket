using BuildingBlocks.Abstractions;
using System.Security.Claims;

namespace Basket.API.Services
{
    public class CurrentUser(IHttpContextAccessor httpContextAccessor)
        : ICurrentUser
    {
        private ClaimsPrincipal? _principal =>
            httpContextAccessor.HttpContext?.User;

        public Guid UserId
        {
            get
            {
                var value = _principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(value, out var userId) ? userId : throw new Exception("Null User id");
            }
        }

        public string? UserName => _principal?.FindFirst(ClaimTypes.Name)?.Value ?? throw new Exception("Null Username");
        public string? UserEmail => _principal?.FindFirst(ClaimTypes.Email)?.Value ?? throw new Exception("Null User email ");
        public bool IsAuthenticated => _principal?.Identity?.IsAuthenticated ?? false;
        public IReadOnlyCollection<string> Roles =>
            _principal?
                .FindAll(ClaimTypes.Role)
                .Select(x => x.Value)
                .ToArray() ?? [];

        public bool IsInRole(string role)
        {
            return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }
    }
}
