namespace BuildingBlocks.Infrastracture.Outbox
{
    public interface IOutboxProcessor
    {
        Task ProcessMessageAsync(CancellationToken cancellationToken = default);
    }
}
