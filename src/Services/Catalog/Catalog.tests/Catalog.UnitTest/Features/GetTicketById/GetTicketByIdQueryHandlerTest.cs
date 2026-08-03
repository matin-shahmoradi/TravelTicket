using BuildingBlocks;
using Catalog.API.Domain.ValueObjects;
using Catalog.API.Models;
using Catalog.API.Repository;
using Catalog.API.Tickets.GetTicketsById;
using FluentAssertions;
using Moq;

namespace Catalog.Unit.test.Features.GetTicketById
{
    public sealed class GetTicketByIdQueryHandlerTest
    {
        private readonly Mock<ITicketQueryRepository> _mockTicketQueryRepository;
        private readonly GetTicketByIdQueryHandler _handler;

        private readonly CancellationToken _cancellationToken;
        public GetTicketByIdQueryHandlerTest()
        {
            _mockTicketQueryRepository = new Mock<ITicketQueryRepository>();

            _handler = new GetTicketByIdQueryHandler(
                queryRepository: _mockTicketQueryRepository.Object);
            _cancellationToken = CancellationToken.None;
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenRequestIsValid()
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

            var query = new GetTicketByIdQuery(ticketId);

            _mockTicketQueryRepository.Setup(
                x => x.GetTicketByIdWithNoTracking(ticketId, _cancellationToken))
                .ReturnsAsync(ticket);

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeTrue();

            result.Value!.Id.Should().Be(TicketId.New(ticketId));
            result.Value!.Origin.Should().Be(ticket.Origin);

            result.Error.Should().BeNull();

            _mockTicketQueryRepository.Verify(x => x.GetTicketByIdWithNoTracking(ticketId, _cancellationToken),
                times: Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTicketNotFound()
        {
            // Arrange
            var ticketId = new Guid("0FF9725F-8DEF-4BE0-8E25-4776B89820A4");

            var query = new GetTicketByIdQuery(ticketId);

            _mockTicketQueryRepository.Setup(
                x => x.GetTicketByIdWithNoTracking(ticketId, _cancellationToken))
                .ReturnsAsync((Ticket?)null);

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeFalse();

            result.Error.Should().Be(Error.NotFoundError(message: $"Ticket {query.Id} Not Found!"));

            _mockTicketQueryRepository.Verify(x => x.GetTicketByIdWithNoTracking(ticketId, _cancellationToken),
                times: Times.Once);
        }
    }
}
