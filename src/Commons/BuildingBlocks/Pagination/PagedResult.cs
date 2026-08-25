using System.Text.Json.Serialization;

namespace BuildingBlocks.Pagination
{
    public sealed class PagedResult<T>
    {
        [JsonConstructor]
        public PagedResult(
            IReadOnlyList<T> items,
            int pageNumber,
            int pageSize,
            long totalCount)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
        public IReadOnlyList<T> Items { get; init; } = [];
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public long TotalCount { get; init; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;

        public static PagedResult<T> CreatePagedResult(
            IReadOnlyList<T> items,
            int pageNumber,
            int pageSize,
            long totalCount)
        {
            return new PagedResult<T>(
                items,
                pageNumber,
                pageSize,
                totalCount);
        }
    }
}
