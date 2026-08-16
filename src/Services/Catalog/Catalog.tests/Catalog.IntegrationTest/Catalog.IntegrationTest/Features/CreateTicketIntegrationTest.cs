using BuildingBlocks.TestBase;
using Catalog.API.Data;
using Catalog.API.Domain.ValueObjects;
using Catalog.IntegrationTest.FakeData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

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

            using var response = await _factory.HttpClient
                 .WithRoles("Admin")
                 .PostAsJsonAsync("/tickets", request);

            var ticketId = await response.Content.ReadFromJsonAsync<Guid>();

            // Act

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
    }
}
