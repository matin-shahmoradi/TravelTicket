using BuildingBlocks.Abstractions;

namespace Basket.IntegrationTests.FakeData
{
    public class FakeCurrentUser : ICurrentUser
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } = "test-user";
        public string UserEmail { get; set; } = "test@test.com";
        public string PhoneNumber { get; set; } = "09000000000";
        public bool IsAuthenticated { get; set; } = true;
        public IReadOnlyCollection<string> Roles { get; set; } = new List<string> { "User" };
        public bool IsInRole(string role) => Roles.Contains(role);
    }
}
