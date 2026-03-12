namespace Basket.API.Basket.DeleteBasket
{
    public record DeleteBasketCommand(string TravlerNumber) : ICommand<Result<string>>;
    internal sealed class DeleteBasketCommandHandler(IBasketRepository basketRepository)
        : ICommandHandler<DeleteBasketCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
        {
            var existedBasket = await basketRepository.GetBasket(command.TravlerNumber);
            if (existedBasket is not null)
            {
                await basketRepository.DeleteBasket(command.TravlerNumber);
                return Result<string>.Success($"Basket with Id: {existedBasket.Id} deleted successfully.");
            }

            return Result<string>.Failure(Error.NotFoundError(message: "Basket Not Found!"));

        }
    }
}
