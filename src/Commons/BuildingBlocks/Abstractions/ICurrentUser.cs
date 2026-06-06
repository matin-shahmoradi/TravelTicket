namespace BuildingBlocks.Abstractions
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }

        Guid? UserId { get; }

        string? UserName { get; }

        IReadOnlyCollection<string> Roles { get; }

        bool IsInRole(string role);
    }
}
