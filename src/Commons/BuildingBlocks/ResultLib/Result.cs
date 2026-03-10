using BuildingBlocks.ResultLib;

namespace BuildingBlocks
{
    /// <summary>
    ///     implementaion of Result pattern.
    /// </summary>
    /// <typeparam name="T"> T for any type. </typeparam>
    public class Result<T> : BaseResult
    {
        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
        }
        private Result(Error error)
        {
            IsSuccess = false;
            Error = error;
        }
        public T? Value { get; init; }

        public static implicit operator Result<T>(T value) => new(value);

        public static Result<T> Success(T value) => new Result<T>(value);
        public static Result<T> Failure(Error error) => new Result<T>(error);
    }
}
