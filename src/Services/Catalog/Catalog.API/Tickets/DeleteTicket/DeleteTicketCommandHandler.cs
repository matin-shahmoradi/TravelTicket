using BuildingBlocks.Abstractions;
using Catalog.API.Repository;

namespace Catalog.API.Tickets.DeleteTicket
{
    public record DeleteTicketCommand(Guid Id) : ICommand<Result<bool>>;
    public sealed class DeleteTicketCommandValidator : AbstractValidator<DeleteTicketCommand>
    {
        public DeleteTicketCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Ticket id is required")
                .NotEqual(Guid.Empty).WithMessage("Invalid guid");
        }
    }
    internal sealed class DeleteTicketCommandHandler(
        ITicketQueryRepository queryRepository,
        ITicketCommandRepository commandRepository,
        IUnitOfWork uow)
        : ICommandHandler<DeleteTicketCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteTicketCommand command, CancellationToken cancellationToken)
        {
            var existTicket = await queryRepository.GetTicketById(command.Id);
            if (existTicket is null)
            {
                return Result<bool>.Failure(Error.NotFoundError(message: "Ticket Not Found!"));
            }
            try
            {
                existTicket.RemoveTicket(command.Id);
                commandRepository.DeleteTicket(existTicket);
                await uow.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(Error.Internal_Server(message: ex.Message));
            }

        }
    }
}
