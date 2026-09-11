using BuildingBlocks.Infrastracture.Outbox;
using BuildingBlocks.Messaging.Events.CatalogEvents;
using BuildingBlocks.TestBase;
using Catalog.API.Tickets.UpdateTicket;
using Catalog.IntegrationTest.FakeData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Catalog.IntegrationTest.Features
{
    public sealed class UpdateTicketIntegrationEvent : IClassFixture<CatalogIntegrationFactory>, IAsyncLifetime
    {
        private readonly CatalogIntegrationFactory _factory;
        public UpdateTicketIntegrationEvent(CatalogIntegrationFactory factory)
        {
            _factory = factory;
        }

        public async Task DisposeAsync() => await _factory.ResetDatabaseAsync();
        public Task InitializeAsync() => Task.CompletedTask;

        [Fact]
        public async Task UpdateTicket_WithAdminRole_ShouldReturns204()
        {
            // Arrange
            var createRequest = FakeTicket.CreateFakeRequestDto();
            var updateRequest = new UpdateTicketRequestDTO(
                Origin: "Origin",
                Destination: "dest",
                Description: "desc",
                Date: DateTime.UtcNow,
                Price: 28000);

            using var createTicketResponse = await _factory.HttpClient
                .WithRoles("Admin")
                .PostAsJsonAsync("/tickets", createRequest);

            createTicketResponse.EnsureSuccessStatusCode();

            var createTicketResponseBody = await createTicketResponse.Content.ReadFromJsonAsync<Guid>();

            // Act
            using var putResponse = await _factory.HttpClient
                .WithRoles("Admin")
                .PutAsJsonAsync($"/tickets/{createTicketResponseBody}", updateRequest);

            // Assert
            putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateTicket_Should_Persist_OutboxMessage()
        {
            // Arrange
            var createRequest = FakeTicket.CreateFakeRequestDto();
            var updateRequest = new UpdateTicketRequestDTO(
                Origin: "Origin",
                Destination: "dest",
                Description: "desc",
                Date: DateTime.UtcNow,
                Price: 28000);

            using var createTicketResponse = await _factory.HttpClient
                .WithRoles("Admin")
                .PostAsJsonAsync("/tickets", createRequest);

            createTicketResponse.EnsureSuccessStatusCode();
            var createTicketResponseBody = await createTicketResponse.Content.ReadFromJsonAsync<Guid>();

            // Act
            using var putResponse = await _factory.HttpClient
                .WithRoles("Admin")
                .PutAsJsonAsync($"/tickets/{createTicketResponseBody}", updateRequest);

            var outboxMessage = await _factory.ExecuteDbContextAsync(async context =>
            {
                var message = await context.OutboxMessages
                    .Where(e => e.Type.Contains(nameof(TicketUpdatedIntegrationEvent)))
                    .FirstOrDefaultAsync();

                return message;
            });

            // Assert
            outboxMessage.Should().NotBeNull();

            outboxMessage.Type.Should().Contain(nameof(TicketUpdatedIntegrationEvent));
            outboxMessage.ProcessedOnUtc.Should().BeNull();
        }

        [Fact]
        public async Task OutboxProcessor_WhenUnprocessedMessagesExist_ShouldPublishToBroker()
        {
            // Arrange
            var createRequest = FakeTicket.CreateFakeRequestDto();
            var updateRequest = new UpdateTicketRequestDTO(
                Origin: "Origin",
                Destination: "dest",
                Description: "desc",
                Date: DateTime.UtcNow,
                Price: 28000);

            using var createTicketResponse = await _factory.HttpClient
                .WithRoles("Admin")
                .PostAsJsonAsync("/tickets", createRequest);

            createTicketResponse.EnsureSuccessStatusCode();
            var createTicketResponseBody = await createTicketResponse.Content.ReadFromJsonAsync<Guid>();

            using var putResponse = await _factory.HttpClient
                .WithRoles("Admin")
                .PutAsJsonAsync($"/tickets/{createTicketResponseBody}", updateRequest);

            // Act
            await _factory.ExecuteScopeAsync(async sp =>
            {
                var outboxProcessor = sp.GetRequiredService<IOutboxProcessor>();

                await outboxProcessor.ProcessMessageAsync();
            });

            var outboxMessage = await _factory.ExecuteDbContextAsync(async context =>
            {
                var message = await context.OutboxMessages
                    .Where(x => x.Type.Contains(nameof(TicketUpdatedIntegrationEvent)))
                    .FirstOrDefaultAsync();

                return message;
            });

            outboxMessage!.ProcessedOnUtc.Should().NotBeNull();
        }
    }
}
