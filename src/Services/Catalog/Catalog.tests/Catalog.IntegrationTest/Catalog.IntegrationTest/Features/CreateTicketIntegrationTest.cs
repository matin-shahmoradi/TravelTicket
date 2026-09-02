using BuildingBlocks.Infrastracture.Outbox;
using BuildingBlocks.Messaging.Events.CatalogEvents;
using BuildingBlocks.TestBase;
using Catalog.API.Data;
using Catalog.API.Domain.ValueObjects;
using Catalog.IntegrationTest.FakeData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Catalog.IntegrationTest.Features
{
    public class CreateTicketIntegrationTest : IClassFixture<CatalogIntegrationFactory>, IAsyncLifetime
    {
        private readonly CatalogIntegrationFactory _factory;
        public CreateTicketIntegrationTest(CatalogIntegrationFactory factory)
        {
            _factory = factory;
        }
        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync() => _factory.ResetDatabaseAsync();

        [Fact]
        public async Task CreateTicket_WithAdminRoke_Returns201()
        {
            // Arrange 
            var requestDto = FakeTicket.CreateFakeRequestDto();

            // Act
            using var response = await _factory.HttpClient
                .WithRoles("Admin")
                .PostAsJsonAsync("/tickets", requestDto);

            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task CreateTicket_WithAdminRole_PersistsToDatabase()
        {
            // Arrange
            var request = FakeTicket.CreateFakeRequestDto();

            // Act
            using var response = await _factory.HttpClient
                 .WithRoles("Admin")
                 .PostAsJsonAsync("/tickets", request);

            var ticketId = await response.Content.ReadFromJsonAsync<Guid>();

            var ticket = await _factory.ExecuteScopeAsync(async action =>
            {
                var context = action.GetRequiredService<CatalogDbContext>();

                return await context.Tickets.SingleOrDefaultAsync(x => x.Id == TicketId.New(ticketId));
            });


            // Arrange

            ticket!.Origin.Should().Be(request.Origin);
            ticket.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateTicket_WithoutAdminRole_Returns403()
        {
            // Arrange 
            var requestDto = FakeTicket.CreateFakeRequestDto();

            // Act
            using var response = await _factory.HttpClient
                .WithRoles("user")
                .PostAsJsonAsync("/tickets", requestDto);

            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task CreateTicket_WithoutToken_Returns401()
        {
            // Arrange 
            var requestDto = FakeTicket.CreateFakeRequestDto();

            // Act
            using var response = await _factory.HttpClient.PostAsJsonAsync("/tickets", requestDto);

            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateTicket_Should_Persist_OutboxMessage()
        {
            // Arrange
            var request = FakeTicket.CreateFakeRequestDto();
            // Act

            using var response = await _factory.HttpClient
                .WithRoles("Admin")
                .PostAsJsonAsync("/tickets", request);

            var responseBody = response.Content.ReadFromJsonAsync<Guid>();
            response.EnsureSuccessStatusCode();

            // Assert
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

            var outboxMessages = await context.OutboxMessages
                .AsNoTracking()
                .Where(x => x.Type.Contains(nameof(TicketCreatedIntegrationEvent)))
                .ToListAsync();


            var message = outboxMessages.LastOrDefault();
            var messageType = Type.GetType(message.Type);

            outboxMessages.Should().NotBeEmpty();

            message.ProcessedOnUtc.Should().BeNull();

            message.Error.Should().BeNull();

            var deseriliziedEvent = JsonSerializer.Deserialize<TicketCreatedIntegrationEvent>(message.Content);

            deseriliziedEvent.Should().NotBeNull();
            deseriliziedEvent.Origin.Should().Contain(request.Origin);
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

            await _factory.ExecuteScopeAsync(async sp =>
            {
                var processor = sp.GetRequiredService<IOutboxProcessor>();
                await processor.ProcessMessageAsync();
            });

            // Act
            var processedMessage = await _factory.ExecuteDbContextAsync(ctx =>
            {
                return ctx.OutboxMessages
                    .AsNoTracking()
                    .Where(x => x.Type.Contains(nameof(TicketCreatedIntegrationEvent)))
                    .FirstOrDefaultAsync();
            });

            // Assert
            processedMessage.Should().NotBeNull();
            processedMessage!.ProcessedOnUtc.Should().NotBeNull();
            processedMessage.Error.Should().BeNull();
        }
    }
}
