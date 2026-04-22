namespace BuildingBlocks.ResultLib
{
    public abstract class BaseResult
    {
        public bool IsSuccess { get; protected set; }
        public Error? Error { get; protected set; }
    }
}
