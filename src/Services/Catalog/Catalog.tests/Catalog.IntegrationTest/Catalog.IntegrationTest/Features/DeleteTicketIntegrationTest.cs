using BuildingBlocks.Infrastracture.Outbox;
using BuildingBlocks.Messaging.Events.CatalogEvents;
using BuildingBlocks.TestBase;
using Catalog.API.Tickets.CreateTicket;
using Catalog.IntegrationTest.FakeData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Catalog.IntegrationTest.Features
{
    public class DeleteTicketIntegrationTest : IClassFixture<CatalogIntegrationFactory>, IAsyncLifetime
    {
        private readonly CatalogIntegrationFactory _factory;
        public DeleteTicketIntegrationTest(CatalogIntegrationFactory factory)
        {
            _factory = factory;
        }
        public Task InitializeAsync() => Task.CompletedTask;
        public async Task DisposeAsync()
        {
            await _factory.ResetDatabaseAsync();
        }

        [Fact]
        public async Task DeleteTicket_WithAdminRole_ShouldDeleteTicket_And_Returns204()
        {
            // Arrange
            var request = new CreateTicketRequestDTO(
                Origin: "tehran",
                Destination: "dest",
                Description: "desc",
                Date: DateTime.UtcNow,
                Price: 2000);

            using var response = await _factory.HttpClient
                .WithRoles("Admin")
                .PostAsJsonAsync("/tickets", request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<Guid>();

            // Act

            using var deleteResponse = await _factory.HttpClient
                .WithRoles("Admin")
                .DeleteAsync($"tickets/{responseBody}");

            response.EnsureSuccessStatusCode();

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteTicket_WithNoRole_ShouldReturn401()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            using var response = await _factory.HttpClient
                .DeleteAsync($"tickets/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        [Fact]
        public async Task DeleteTicket_WithNoAdminRole_ShouldReturn403()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            using var response = await _factory.HttpClient
                .WithRoles("User")
                .DeleteAsync($"tickets/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task DeleteTicket_Should_Persist_OutboxMessage()
        {
            // Arrange
            var request = FakeTicket.CreateFakeRequestDto();
            using var response = await _factory.HttpClient
                .WithRoles("Admin")
                .PostAsJsonAsync("/tickets", request);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<Guid>();

            // Act
            using var deleteResponse = await _factory.HttpClient
                .WithRoles("Admin")
                .DeleteAsync($"tickets/{responseBody}");

            response.EnsureSuccessStatusCode();


            var outboxMessage = await _factory.ExecuteDbContextAsync(async x =>
            {
                var message = await x.OutboxMessages
                    .Where(m => m.Type.Contains(nameof(TicketDeletedIntegrationEvent)))
                    .FirstOrDefaultAsync();

                return message;
            });

            // Assert

            outboxMessage.Should().NotBeNull();
            outboxMessage.ProcessedOnUtc.Should().BeNull();
            outboxMessage.Error.Should().BeNull();
        }

        [Fact]
        public async Task OutboxProcessor_WhenUnprocessedMessagesExist_ShouldPublishToBroker()
        {
            // Arrange
            var request = FakeTicket.CreateFakeRequestDto();
            using var response = await _factory.HttpClient
                .WithRoles("Admin")
                .PostAsJsonAsync("/tickets", request);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<Guid>();

            // Act
            using var deleteResponse = await _factory.HttpClient
                .WithRoles("Admin")
                .DeleteAsync($"tickets/{responseBody}");

            await _factory.ExecuteScopeAsync(async sp =>
            {
                var processor = sp.GetRequiredService<IOutboxProcessor>();
                await processor.ProcessMessageAsync();
            });
            var processedMessage = await _factory.ExecuteDbContextAsync(ctx =>
            {
                return ctx.OutboxMessages
                    .AsNoTracking()
                    .Where(x => x.Type.Contains(nameof(TicketDeletedIntegrationEvent)))
                    .FirstOrDefaultAsync();
            });

            // Assert
            processedMessage.Should().NotBeNull();
            processedMessage.ProcessedOnUtc.Should().NotBeNull();
            processedMessage.Error.Should().BeNull();
        }
    }
}
