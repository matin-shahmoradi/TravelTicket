using Basket.API.Basket.AddItemToBasket;
using Basket.API.Common.Dtos;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text.Json;

namespace Basket.IntegrationTests.Features
{
    public class AddItemToBasketIntegrationTest : BasketIntegrationTestBase
    {
        public AddItemToBasketIntegrationTest(BasketIntegrationTestFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Handle_WhenTicketExistsInCache_ShouldNotCallGrpc()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var cachedTicket = IntegrationTestHelper.BuildTicketReadModel(ticketId, price: 150000);

            await _cache.SetStringAsync(
                key: ticketId.ToString(),
                value: JsonSerializer.Serialize(cachedTicket));

            var command = new AddItemToBasketCommand(
                new AddItemToBasketDto(ticketId, Quantity: 2));

            // Act
            var result = await _factory.SendAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.CustomerId.Should().Be(TestUserId);
            result.Value.Items.Should().HaveCount(1);
            result.Value.Items[0].TicketId.Should().Be(ticketId);
            result.Value.Items[0].Quantity.Should().Be(2);
            result.Value.Items[0].Price.Should().Be(150000);
            result.Value.TotalPrice.Should().Be(300000);

            _factory.CatalogGrpcClientMock
                .Verify(x => x.GetTicketByIdAsync(It.IsAny<string>()), Times.Never);

            var savedCart = await _backetDbContext.ShoppingCarts
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.CustomerId == TestUserId);

            savedCart.Should().NotBeNull();
            savedCart!.Items.Should().HaveCount(1);
        }

    }
}
