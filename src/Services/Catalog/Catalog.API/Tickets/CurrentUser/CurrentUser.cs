using BuildingBlocks.Abstractions;
using System.Security.Claims;

namespace Catalog.API.Tickets.CurrentUser
{
    public class CurrentUser(IHttpContextAccessor httpContextAccessor)
        : ICurrentUser
    {
        private ClaimsPrincipal? _principal =>
            httpContextAccessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                var value = _principal!.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(value, out var userId) ? userId : null;
            }
        }

        public string? UserName => _principal!.FindFirst(ClaimTypes.Name)!.Value;
        public string? UserEmail => _principal!.FindFirst(ClaimTypes.Email)!.Value;
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
