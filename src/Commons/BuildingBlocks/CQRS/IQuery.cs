using MediatR;

namespace BuildingBlocks.CQRS
{
    // This interface used for read operation
    public interface IQuery<out TResponse> : IRequest<TResponse> 
        where TResponse : notnull
    {
    }
}
