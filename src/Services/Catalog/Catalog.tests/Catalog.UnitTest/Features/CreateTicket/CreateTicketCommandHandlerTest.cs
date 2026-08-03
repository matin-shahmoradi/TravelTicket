using BuildingBlocks;
using BuildingBlocks.Abstractions;
using Catalog.API.Models;
using Catalog.API.Repository;
using Catalog.API.Tickets.CreateTicket;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.Unit.test.Features.CreateTicket
{
    public sealed class CreateTicketCommandHandlerTest
    {
        private readonly Mock<ITicketCommandRepository> _mockTicketRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<CreateTicketCommandHandler>> _mockLogger;
        private readonly CreateTicketCommandHandler _handler;
        private readonly CancellationToken _cancellationToken;
        public CreateTicketCommandHandlerTest()
        {
            _mockTicketRepository = new Mock<ITicketCommandRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<CreateTicketCommandHandler>>();
            _handler = new CreateTicketCommandHandler(
                repository: _mockTicketRepository.Object,
                unitOfWork: _mockUnitOfWork.Object,
                logger: _mockLogger.Object);
            _cancellationToken = CancellationToken.None;
        }
        private CreateTicketRequestDTO CreateTicketRequestDTO =
            new CreateTicketRequestDTO("Origin", "dest", "description", DateTime.UtcNow, 250000);

        [Fact]
        public async Task Handle_WithValidRequest_ShouldAddTicketAndCommit()
        {
            // Arrange 
            var command = new CreateTicketCommand(CreateTicketRequestDTO);
            Ticket? capturedTicket = null;


            _mockTicketRepository.Setup(repo => repo.AddTicketAsync(
                It.IsAny<Ticket>(), _cancellationToken))
                .Callback<Ticket, CancellationToken>((ticket, _) => capturedTicket = ticket)
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(_cancellationToken))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, _cancellationToken);


            // Assert

            capturedTicket.Should().NotBeNull();

            CreateTicketRequestDTO.Origin.Should().Be(capturedTicket.Origin);
            CreateTicketRequestDTO.Destination.Should().Be(capturedTicket.Destination);
            CreateTicketRequestDTO.Description.Should().Be(capturedTicket.Description);
            CreateTicketRequestDTO.Date.Should().Be(capturedTicket.TravelDate);
            CreateTicketRequestDTO.Price.Should().Be(capturedTicket.Price);

            result.IsSuccess.Should().BeTrue();

            result.Value.Should().Be(capturedTicket.Id.Value);

            result.Error.Should().BeNull();

            _mockTicketRepository.Verify(repo => repo.AddTicketAsync(
               ticket: It.IsAny<Ticket>(),
               cancellationToken: _cancellationToken),
               Times.Once);

            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(_cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrows_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateTicketCommand(CreateTicketRequestDTO);

            _mockTicketRepository.Setup(repo => repo.AddTicketAsync(It.IsAny<Ticket>(), _cancellationToken))
                .ThrowsAsync(new Exception("db error"));

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert 
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(Error.Internal_Server(message: "db error"));

            _mockTicketRepository.Verify(
                repo => repo.AddTicketAsync(
                    ticket: It.IsAny<Ticket>(), _cancellationToken),
                times: Times.Once());

            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Never);
        }
    }
}
