using BuildingBlocks;
using BuildingBlocks.Abstractions;
using Catalog.API.Domain.ValueObjects;
using Catalog.API.Models;
using Catalog.API.Repository;
using Catalog.API.Tickets.DeleteTicket;
using FluentAssertions;
using Moq;

namespace Catalog.Unit.test.Features.DeleteTicket
{
    public class DeleteTicketCommandHandlerTest
    {
        private readonly Mock<ITicketQueryRepository> _mockTicketQueryRepository;
        private readonly Mock<ITicketCommandRepository> _mockTicketCommandRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly DeleteTicketCommandHandler _handler;
        private readonly CancellationToken _cancellationToken;

        public DeleteTicketCommandHandlerTest()
        {
            _mockTicketQueryRepository = new Mock<ITicketQueryRepository>();
            _mockTicketCommandRepository = new Mock<ITicketCommandRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new DeleteTicketCommandHandler(
                _mockTicketQueryRepository.Object,
                _mockTicketCommandRepository.Object,
                _mockUnitOfWork.Object);

            _cancellationToken = CancellationToken.None;
        }

        [Fact]
        public async Task Handle_WithValidRequest_ShouldDeleteTicket()
        {
            // Arrange
            var ticketId = Guid.NewGuid();

            var ticket = Ticket.Create(
                id: TicketId.New(ticketId),
                origin: "Kerman",
                destination: "Tabriz",
                description: "Description!",
                travelDate: new DateTime(2026, 10, 15, 10, 15, 30, DateTimeKind.Utc),
                price: 2000);

            var command = new DeleteTicketCommand(ticketId);

            _mockTicketQueryRepository.Setup(x => x.GetTicketById(ticketId, _cancellationToken))
                .ReturnsAsync(ticket);

            _mockTicketCommandRepository.Setup(x => x.DeleteTicket(ticket));

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(_cancellationToken))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeNull();
            result.Value.Should().BeTrue();

            _mockTicketQueryRepository.Verify(
                x => x.GetTicketById(ticketId, _cancellationToken),
                times: Times.Once);

            _mockTicketCommandRepository.Verify(
                x => x.DeleteTicket(It.IsAny<Ticket>()),
                times: Times.Once);

            _mockUnitOfWork.Verify(
                x => x.SaveChangesAsync(_cancellationToken),
                times: Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldFail_WhenTicketNotFound()
        {
            // Arrange
            var ticketId = Guid.NewGuid();

            var command = new DeleteTicketCommand(ticketId);

            _mockTicketQueryRepository.Setup(x => x.GetTicketById(ticketId, _cancellationToken))
                .ReturnsAsync((Ticket?)null);
            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Should().Be(Error.NotFoundError("Ticket Not Found!"));

            _mockTicketQueryRepository.Verify(
                x => x.GetTicketById(ticketId, _cancellationToken),
                times: Times.Once);

            _mockTicketCommandRepository.Verify(
                x => x.DeleteTicket(It.IsAny<Ticket>()),
                times: Times.Never);

            _mockUnitOfWork.Verify(
                x => x.SaveChangesAsync(_cancellationToken),
                times: Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenDeleteTicketThrowException()
        {
            // Arrange
            var ticketId = Guid.NewGuid();

            var ticket = Ticket.Create(
                id: TicketId.New(ticketId),
                origin: "Kerman",
                destination: "Tabriz",
                description: "Description!",
                travelDate: new DateTime(2026, 10, 15, 10, 15, 30, DateTimeKind.Utc),
                price: 2000);

            var command = new DeleteTicketCommand(ticketId);

            _mockTicketQueryRepository.Setup(x => x.GetTicketById(ticketId, _cancellationToken))
                .ReturnsAsync(ticket);

            _mockTicketCommandRepository.Setup(x => x.DeleteTicket(ticket))
                .Throws(new Exception("ef Delete exception"));


            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Should().Be(Error.Internal_Server(message: "ef Delete exception"));

            _mockTicketQueryRepository.Verify(
                x => x.GetTicketById(ticketId, _cancellationToken),
                times: Times.Once);

            _mockTicketCommandRepository.Verify(
                x => x.DeleteTicket(It.IsAny<Ticket>()),
                times: Times.Once);

            _mockUnitOfWork.Verify(
                x => x.SaveChangesAsync(_cancellationToken),
                times: Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldFail_WhenSaveChangesAsyncThrowException()
        {
            // Arrange
            var ticketId = Guid.NewGuid();

            var ticket = Ticket.Create(
                id: TicketId.New(ticketId),
                origin: "Kerman",
                destination: "Tabriz",
                description: "Description!",
                travelDate: new DateTime(2026, 10, 15, 10, 15, 30, DateTimeKind.Utc),
                price: 2000);

            var command = new DeleteTicketCommand(ticketId);

            _mockTicketQueryRepository.Setup(x => x.GetTicketById(ticketId, _cancellationToken))
                .ReturnsAsync(ticket);

            _mockTicketCommandRepository.Setup(x => x.DeleteTicket(ticket));

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(_cancellationToken))
                .Throws(new Exception("Database exception"));

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Should().Be(Error.Internal_Server(message: "Database exception"));

            _mockTicketQueryRepository.Verify(
                x => x.GetTicketById(ticketId, _cancellationToken),
                times: Times.Once);

            _mockTicketCommandRepository.Verify(
                x => x.DeleteTicket(It.IsAny<Ticket>()),
                times: Times.Once);

            _mockUnitOfWork.Verify(
                x => x.SaveChangesAsync(_cancellationToken),
                times: Times.Once);
        }
    }
}
