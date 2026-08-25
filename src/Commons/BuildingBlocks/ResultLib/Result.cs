using System.Text.Json.Serialization;

namespace BuildingBlocks
{
    /// <summary>
    ///     implementaion of Result pattern.
    /// </summary>
    /// <typeparam name="T"> T for any type. </typeparam>
    public class Result<T> : BaseResult
    {
        // This constructor is used by System.Text.Json.
        [JsonConstructor]
        public Result(bool isSuccess, T? value, Error? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }
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
