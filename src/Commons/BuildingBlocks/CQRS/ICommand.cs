using MediatR;

namespace BuildingBlocks.CQRS
{
    // This is an empty ICommand that does not return any responses.
    public interface ICommand : ICommand<Unit>
    {
    }

    // ICommand interface for produce response.
    public interface ICommand<out TResponse>: IRequest<TResponse>
    {

    }
}
