using Basket.API.Basket.DeleteBasket;
using Basket.API.Common.Dtos;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.IntegrationTests.Features
{
    public class DeleteBasketIntegrationTest : BasketIntegrationTestBase
    {
        private readonly IntegrationTestHelper _testHelper;
        public DeleteBasketIntegrationTest(BasketIntegrationTestFactory factory) : base(factory)
        {
            _testHelper = new(_cache, _backetDbContext);
        }

        [Fact]
        public async Task DeleteBasket_WhenBasketExists_ShouldDeleteFromDatabaseAndCache()
        {
            // Arrange
            await _testHelper.SeedDb(TestUserId);

            var cachedBasket = new ShoppingCartDto
            {
                Id = Guid.NewGuid(),
                CustomerId = TestUserId,
                Items = new List<ShoppingCartItemDto>()
            };

            await _cache.SetStringAsync(
               key: TestUserId.ToString(),
               value: JsonSerializer.Serialize(cachedBasket));

            var command = new DeleteBasketCommand();

            // Act
            var result = await _factory.SendAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var deletedBasket = await _backetDbContext.ShoppingCarts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerId == TestUserId);

            deletedBasket.Should().BeNull();

            var cacheValue = await _cache.GetStringAsync(key: TestUserId.ToString());

            cacheValue.Should().BeNull();
        }

    }
}
