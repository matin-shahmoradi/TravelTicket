namespace BuildingBlocks.Abstractions
{
    public interface ITransactionExecutor
    {
        Task<TResult> ExecuteAsync<TResult>(
            Func<CancellationToken, Task<TResult>> action,
            CancellationToken cancellationToken = default);
    }
}
