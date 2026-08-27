using BuildingBlocks;
using BuildingBlocks.Pagination;
using Catalog.API.Data;
using Catalog.API.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Catalog.IntegrationTest.Features
{
    public sealed class GetTicketsIntegrationTest : IClassFixture<CatalogIntegrationFactory>
    {
        private readonly CatalogIntegrationFactory _factory;
        public GetTicketsIntegrationTest(CatalogIntegrationFactory Factory)
        {
            _factory = Factory;
        }

        [Fact]
        public async Task GetTickets_WithValidRequest_ShouldReturnTicket()
        {
            // Arrange
            const int PageNumber = 1;
            const int PageSize = 10;

            string requestUri = $"/tickets?pageNumber={PageNumber}&pageSize={PageSize}";
            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(requestUri);

            var result = await response.Content.ReadFromJsonAsync<Result<PagedResult<TicketDto>>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            result!.IsSuccess.Should().BeTrue();
            result.Value!.Should().NotBeNull();
            result.Error.Should().BeNull();
        }

        [Fact]
        public async Task GetTicket_WhenTicketsAreNull_ShouldReturn404()
        {
            // Arrange 
            const int PageNumber = 1;
            const int PageSize = 10;

            await _factory.ExecuteScopeAsync(async exe =>
            {
                var dbContext = exe.GetRequiredService<CatalogDbContext>();
                await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.MigrateAsync();
            });


            string requestUri = $"/tickets?pageNumber={PageNumber}&pageSize={PageSize}";
            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(requestUri);

            var result = await response.Content.ReadFromJsonAsync<Result<PagedResult<TicketDto>>>();


            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            result!.IsSuccess.Should().BeFalse();

            result.Value.Should().BeNull();
        }
    }
}