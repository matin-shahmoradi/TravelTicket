using Basket.API.basket.BasketCheckout;
using BuildingBlocks.Messaging.Events.BasketEvents;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Basket.IntegrationTests.Features
{
    public class BasketCheckoutIntegerationTest : BasketIntegrationTestBase
    {
        private readonly IntegrationTestHelper testHelper;
        public BasketCheckoutIntegerationTest(BasketIntegrationTestFactory factory) : base(factory)
        {
            testHelper = new(_cache, _backetDbContext);
        }

        [Fact]
        public async Task BasketCheckout_Persist_OutboxMessage()
        {
            // Arrange

            await testHelper.SeedDb(TestUserId);

            var command = new BasketCheckOutCommand();

            // Act
            var result = await _factory.SendAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var outboxMessages =
                await _backetDbContext.outboxMessages
                .AsNoTracking()
                .Where(x => x.Type.Contains(nameof(BasketCheckOutIntegrationEvent)))
                .FirstOrDefaultAsync();

            outboxMessages.Should().NotBeNull();

            outboxMessages.ProcessedOnUtc.Should().BeNull();
        }

        [Fact]
        public async Task OutboxProcessor_WhenUnprocessedMessageExist_ShouldPublishMessage_ToBroker()
        {
            await testHelper.SeedDb(TestUserId);
            var command = new BasketCheckOutCommand();

            // Act
            var result = await _factory.SendAsync(command);

            await _outboxProcessor.ProcessMessageAsync();
            // Assert
            result.IsSuccess.Should().BeTrue();

            var outboxMessages =
                await _backetDbContext.outboxMessages
                .AsNoTracking()
                .Where(x => x.Type.Contains(nameof(BasketCheckOutIntegrationEvent)))
                .FirstOrDefaultAsync();

            outboxMessages.ProcessedOnUtc.Should().NotBeNull();
        }
    }
}
