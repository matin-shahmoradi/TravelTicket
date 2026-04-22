
namespace Catalog.API.Common
{
    public static class ResultExtensions
    {
        public static T Match<T>(
            this Result<T> result,
            Func<T> onSuccess,
            Func<Error?, T> onFailure)
        {
            return result.IsSuccess ? onSuccess() : onFailure(result.Error!);
        }

        public static T Match<T, TValue>(
            this Result<TValue> result,
            Func<TValue, T> onSuccess,
            Func<Error?, T> onFailure)
        {
            return result.IsSuccess ? onSuccess(result.Value!) : onFailure(result.Error!);
        }
    }
}
