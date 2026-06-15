using BuildingBlocks.Abstractions;

namespace Basket.API.Basket.DeleteBasket
{
    public record DeleteBasketCommand : ICommand<Result<string>>;
    internal sealed class DeleteBasketCommandHandler(
        IBasketRepository basketRepository,
        ICurrentUser currentUser)
        : ICommandHandler<DeleteBasketCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
        {
            var existedBasket = await basketRepository.GetBasket(currentUser.UserEmail!);
            if (existedBasket is not null)
            {
                await basketRepository.DeleteBasket(currentUser.UserEmail!);
                return Result<string>.Success($"Basket with Id: {existedBasket.Id} deleted successfully.");
            }

            return Result<string>.Failure(Error.NotFoundError(message: "Basket Not Found!"));
        }
    }
}
