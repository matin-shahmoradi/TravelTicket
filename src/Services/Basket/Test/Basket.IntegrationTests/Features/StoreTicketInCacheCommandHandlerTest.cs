using Basket.API.Basket.StoreTicket;
using Basket.API.Model;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.IntegrationTests.Features
{
    public class StoreTicketInCacheCommandHandlerTest : BasketIntegrationTestBase
    {
        public StoreTicketInCacheCommandHandlerTest(BasketIntegrationTestFactory factory) : base(factory)
        {

        }

        [Fact]
        public async Task Handle_Should_StoreTicketInCache()
        {
            // Arrange
            var ticket = IntegrationTestHelper.BuildTicketReadModel(Guid.NewGuid(), 5);

            var command = new StoreTicketCommand(ticket);

            // Act

            var result = await _factory.SendAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var cacheValue = await _cache.GetStringAsync(ticket.TicketId.ToString());
            cacheValue.Should().NotBeNull();

            var deserilizedTicket = JsonSerializer.Deserialize<TicketReadModel>(cacheValue!);

            deserilizedTicket!.Origin.Should().Be(ticket.Origin);

        }
    }
}
