namespace Catalog.API.CatalogExtensions
{
    public static class CatalogExtension
    {
        public static IResult ToHttpResult<T>(this Result<T> result)
        {
            return result.Error!.Value.ErrorType switch
            {
                ErrorType.VALIDATION_ERROR => Results.BadRequest(result.Error),
                ErrorType.NOT_FOUND => Results.NotFound(result.Error),
                _ => Results.Problem(result.Error.Value.Message)
            };
        }
    }
}
