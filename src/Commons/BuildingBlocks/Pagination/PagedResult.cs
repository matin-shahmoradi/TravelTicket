namespace BuildingBlocks.Pagination
{
    public class PagedResult<TEntity>
        (int PageIndex, int PageSize , long Count , IEnumerable<TEntity> data)
        where TEntity : class
    {
        public int PageIndex { get; } = PageIndex;
        public int PageSize { get; } = PageSize;
        public long Count { get; } = Count;
        public IEnumerable<TEntity> Data { get; } = data;
    }
}
