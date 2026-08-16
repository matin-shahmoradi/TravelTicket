using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BuildingBlocks.TestBase
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "TestScheme";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Test-Roles", out var rolesHeader))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, Request.Headers["Test-UserId"].FirstOrDefault() ?? Guid.NewGuid().ToString()),
                    new(ClaimTypes.Name, Request.Headers["Test-UserName"].FirstOrDefault() ?? "test-user")
                };

            var roles = rolesHeader.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
    public static class HttpClientAuthExtensions
    {
        public static HttpClient WithRoles(this HttpClient client, params string[] roles)
        {
            client.DefaultRequestHeaders.Add("Test-Roles", string.Join(",", roles));
            return client;
        }

        public static HttpClient WithUser(this HttpClient client, string userId, params string[] roles)
        {
            client.DefaultRequestHeaders.Add("Test-UserId", userId);
            client.DefaultRequestHeaders.Add("Test-Roles", string.Join(",", roles));
            return client;
        }
    }
}
