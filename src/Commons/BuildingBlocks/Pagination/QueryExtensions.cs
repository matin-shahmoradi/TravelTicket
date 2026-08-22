using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Pagination
{
    public static class QueryExtensions
    {
        /// <summary>
        ///     Creates a paged result from the given query.
        /// </summary>
        /// <typeparam name="TSource"> The type of the source data in the query. </typeparam>
        /// <typeparam name="TDestination"> he type of the items returned in the paged result. </typeparam>
        /// <param name="query"> The source query to paginate </param>
        /// <param name="pageRequest"> Contains the page number and page size. </param>
        /// <param name="projection"> Maps the source query to the destination type. </param>
        /// <returns> A paged result containing the requested items and pagination information. </returns>
        public static async Task<PagedResult<TDestination>> ToPagedResultAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            PageRequest pageRequest,
            Func<IQueryable<TSource>, IQueryable<TDestination>> projection,
            CancellationToken cancellationToken)
        {
            var pageNumber = pageRequest.PageNumber;
            var pageSize = pageRequest.PageSize;

            var totalCount = await query.LongCountAsync(cancellationToken);

            var items = await projection(query)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return PagedResult<TDestination>.CreatePagedResult(
                items,
                pageNumber,
                pageSize,
                totalCount);
        }
    }
}
