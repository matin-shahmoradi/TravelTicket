namespace BuildingBlocks.Pagination
{
    public class PageRequest
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
        public int PageNumber { get; init; } = DefaultPageNumber;
        public int PageSize { get; init; } = DefaultPageSize;
        public string? SortBy { get; set; }
        public string? Search { get; set; }
    }
}
