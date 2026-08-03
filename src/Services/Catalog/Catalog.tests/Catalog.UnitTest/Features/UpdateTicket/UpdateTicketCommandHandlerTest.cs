using BuildingBlocks;
using BuildingBlocks.Abstractions;
using Catalog.API.Domain.ValueObjects;
using Catalog.API.Models;
using Catalog.API.Repository;
using Catalog.API.Tickets.UpdateTicket;
using FluentAssertions;
using Moq;

namespace Catalog.Unit.test.Features.UpdateTicket
{
    public sealed class UpdateTicketCommandHandlerTest
    {
        private readonly Mock<ITicketQueryRepository> _mockTicketQueryRepository;
        private readonly Mock<ITicketCommandRepository> _mockTicketCommandRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UpdateTicketCommandHandler _handler;

        private readonly CancellationToken _cancellationToken;
        public UpdateTicketCommandHandlerTest()
        {
            _mockTicketQueryRepository = new Mock<ITicketQueryRepository>();
            _mockTicketCommandRepository = new Mock<ITicketCommandRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new UpdateTicketCommandHandler(
                queryRepository: _mockTicketQueryRepository.Object,
                commandRepository: _mockTicketCommandRepository.Object,
                unitOfWork: _mockUnitOfWork.Object);
        }
        [Fact]
        public async Task Handle_WithValidRequest_ShouldUpdateTicket()
        {
            // Arrange
            var ticketId = new Guid("0FF9725F-8DEF-4BE0-8E25-4776B89820A4");

            var ticket = Ticket.Create(
                id: TicketId.New(ticketId),
                origin: "Kerman",
                destination: "Tabriz",
                description: "Description!",
                travelDate: new DateTime(2026, 10, 15, 10, 15, 30, DateTimeKind.Utc),
                price: 2000);

            var requestDto = new UpdateTicketRequestDTO(
                Origin: "Kerman",
                Destination: "Tabriz",
                Description: null,
                Date: null,
                Price: 5000
                );

            var command = new UpdateTicketCommand(ticketId, requestDto);

            ticket.ChangeOrigin(requestDto.Origin!);
            ticket.ChangeDestination(requestDto.Destination!);
            ticket.ChangePrice(requestDto.Price!.Value);

            Ticket? updatedTicket = null;

            _mockTicketQueryRepository
                .Setup(x => x.GetTicketById(ticketId, _cancellationToken))
                .ReturnsAsync(ticket);

            _mockTicketCommandRepository.Setup(x => x.UpdateTicket(ticket))
                .Callback<Ticket>((passedTicket) => updatedTicket = passedTicket);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(_cancellationToken))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Error.Should().BeNull();

            updatedTicket!.Origin.Should().Be("Kerman");
            updatedTicket.Destination.Should().Be("Tabriz");
            updatedTicket.Price.Should().Be(5000);

            _mockTicketQueryRepository.Verify
                (x => x.GetTicketById(ticketId, _cancellationToken), Times.Once);

            _mockTicketCommandRepository.Verify(
                x => x.UpdateTicket(It.IsAny<Ticket>()),
                times: Times.Once);

            _mockUnitOfWork.Verify(
                x => x.SaveChangesAsync(_cancellationToken),
                times: Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTicketIsNotFound()
        {
            // Arrange
            var ticketId = new Guid("0FF9725F-8DEF-4BE0-8E25-4776B89820A4");

            var requestDto = new UpdateTicketRequestDTO(
               Origin: "Kerman",
               Destination: "Tabriz",
               Description: null,
               Date: null,
               Price: 5000
               );

            _mockTicketQueryRepository
                .Setup(x => x.GetTicketById(ticketId, _cancellationToken))
                .ReturnsAsync((Ticket?)null);

            var command = new UpdateTicketCommand(ticketId, requestDto);

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Should().Be(Error.NotFoundError(message: "Ticket Not Found!"));

            _mockTicketQueryRepository.Verify(
                x => x.GetTicketById(ticketId, _cancellationToken),
               times: Times.Once);

            _mockTicketCommandRepository.Verify(
                x => x.UpdateTicket(It.IsAny<Ticket>()),
                times: Times.Never);

            _mockUnitOfWork.Verify(
                x => x.SaveChangesAsync(_cancellationToken),
                times: Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldRetrunFailure_WhenUpdateThrowException()
        {
            // Arrange
            var ticketId = new Guid("0FF9725F-8DEF-4BE0-8E25-4776B89820A4");

            var ticket = Ticket.Create(
                id: TicketId.New(ticketId),
                origin: "Kerman",
                destination: "Tabriz",
                description: "Description!",
                travelDate: new DateTime(2026, 10, 15, 10, 15, 30, DateTimeKind.Utc),
                price: 2000);

            var requestDto = new UpdateTicketRequestDTO(
                Origin: "Kerman",
                Destination: "Tabriz",
                Description: null,
                Date: null,
                Price: 5000
                );

            var command = new UpdateTicketCommand(ticketId, requestDto);

            _mockTicketQueryRepository.Setup(x => x.GetTicketById(ticketId, _cancellationToken))
                .ReturnsAsync(ticket);

            _mockTicketCommandRepository.Setup(x => x.UpdateTicket(It.IsAny<Ticket>()))
                .Throws(new Exception("Update failed"));

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Should().Be(Error.Internal_Server(message: "Update failed"));

            _mockTicketQueryRepository.Verify(
                x => x.GetTicketById(ticketId, _cancellationToken),
                times: Times.Once);

            _mockTicketCommandRepository.Verify(
                x => x.UpdateTicket(It.IsAny<Ticket>()),
                times: Times.Once);

            _mockUnitOfWork.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                times: Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldRetrunFailure_WhenSaveChangesAsyncThrowException()
        {
            // Arrange
            var ticketId = new Guid("0FF9725F-8DEF-4BE0-8E25-4776B89820A4");

            var ticket = Ticket.Create(
                id: TicketId.New(ticketId),
                origin: "Kerman",
                destination: "Tabriz",
                description: "Description!",
                travelDate: new DateTime(2026, 10, 15, 10, 15, 30, DateTimeKind.Utc),
                price: 2000);

            var requestDto = new UpdateTicketRequestDTO(
                Origin: "Kerman",
                Destination: "Tabriz",
                Description: null,
                Date: null,
                Price: 5000
                );

            var command = new UpdateTicketCommand(ticketId, requestDto);

            _mockTicketQueryRepository.Setup(x => x.GetTicketById(ticketId, _cancellationToken))
                .ReturnsAsync(ticket);

            _mockTicketCommandRepository.Setup(x => x.UpdateTicket(It.IsAny<Ticket>()));

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(_cancellationToken))
                .Throws(new Exception("Save changes failed"));

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Should().Be(Error.Internal_Server(message: "Save changes failed"));

            _mockTicketQueryRepository.Verify(
                x => x.GetTicketById(ticketId, _cancellationToken),
                times: Times.Once);

            _mockTicketCommandRepository.Verify(
                x => x.UpdateTicket(It.IsAny<Ticket>()),
                times: Times.Once);

            _mockUnitOfWork.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                times: Times.Once);
        }
    }
}
