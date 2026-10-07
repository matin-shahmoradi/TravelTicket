using Basket.API.Basket.AddItemToBasket;
using Basket.API.Common.Dtos;
using Basket.API.Data.Repositories;
using Basket.API.Data.Repository;
using Basket.API.Grpc;
using Basket.API.Model;
using BuildingBlocks.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace Basket.UnitTests.Features.AddItemToBasket
{
    public class AddItemToBasketCommandHandlerTests
    {
        private readonly Mock<IBasketRepository> _basketRepositoryMock;
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly Mock<ICacheTicketRepository> _cacheTicketRepositoryMock;
        private readonly Mock<ICurrentUser> _currentUserMock;
        private readonly Mock<ICatalogGrpcClient> _grpcClientMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<AddItemToBasketCommandHandler>> _loggerMock;
        private readonly AddItemToBasketCommandHandler _handler;
        private readonly CancellationToken cancellationToken = new();
        public AddItemToBasketCommandHandlerTests()
        {
            _basketRepositoryMock = new Mock<IBasketRepository>();
            _distributedCacheMock = new Mock<IDistributedCache>();
            _cacheTicketRepositoryMock = new Mock<ICacheTicketRepository>();
            _currentUserMock = new Mock<ICurrentUser>();
            _grpcClientMock = new Mock<ICatalogGrpcClient>();
            _loggerMock = new Mock<ILogger<AddItemToBasketCommandHandler>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new AddItemToBasketCommandHandler(
                _basketRepositoryMock.Object,
                _distributedCacheMock.Object,
                _cacheTicketRepositoryMock.Object,
                _currentUserMock.Object,
                _grpcClientMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenUserBasketIsNull_ShouldCreateNewBasket_AndAddItemToBasket()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            var ticketSample = TestHelper.CreateTicketSample(ticketId);
            var addItemToBasketDto = new AddItemToBasketDto(ticketId, 1);

            _currentUserMock.SetupGet(x => x.UserId).Returns(userId);

            _basketRepositoryMock.Setup(x => x.GetBasket(userId))
                .ReturnsAsync((ShoppingCart?)null);

            _cacheTicketRepositoryMock.Setup(x => x.ReadTicketFromCacheAsync(ticketId.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JsonSerializer.Serialize(ticketSample));

            var command = new AddItemToBasketCommand(addItemToBasketDto);

            // Act

            var result = await _handler.Handle(command, cancellationToken);

            // Assert

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            _basketRepositoryMock.Verify(
                x => x.StoreBasket(
                   basket: It.IsAny<ShoppingCart>(),
                   cancellation: It.IsAny<CancellationToken>()),
               times: Times.Once());

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
               times: Times.Once);

            _grpcClientMock.Verify(
                x => x.GetTicketByIdAsync(ticketId.ToString()),
               times: Times.Never());

            _distributedCacheMock.Verify(
                x => x.RemoveAsync(userId.ToString(), It.IsAny<CancellationToken>()),
                times: Times.Once);

            _distributedCacheMock.Verify(
                x => x.SetAsync(
                    key: userId.ToString(),
                    value: It.IsAny<byte[]>(),
                    options: It.IsAny<DistributedCacheEntryOptions>(),
                    token: It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task Handle_WhenTicketCacheMisses_ShouldFetchFromGrpc_SaveToCache_AndCommit()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            var cart = ShoppingCart.Create(userId);
            var ticketSample = TestHelper.CreateTicketSample(ticketId);
            var addItemToBasketDto = new AddItemToBasketDto(ticketId, 1);

            _currentUserMock.SetupGet(x => x.UserId).Returns(userId);

            _basketRepositoryMock.Setup(
                x => x.GetBasket(userId)).ReturnsAsync(cart);

            _cacheTicketRepositoryMock.Setup(
                x => x.ReadTicketFromCacheAsync(userId.ToString(), It.IsAny<CancellationToken>()))!
                .ReturnsAsync((string?)null);

            _grpcClientMock.Setup(x => x.GetTicketByIdAsync(ticketId.ToString()))
                .ReturnsAsync(ticketSample);

            var command = new AddItemToBasketCommand(addItemToBasketDto);

            // Act

            var result = await _handler.Handle(command, cancellationToken);

            // Assert

            result.IsSuccess.Should().BeTrue();

            result.Value.Should().NotBeNull();

            _cacheTicketRepositoryMock.Verify(x => x.StoreTicketInCacheAsync(ticketSample, It.IsAny<CancellationToken>()), Times.Once());
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

            _grpcClientMock.Verify(x => x.GetTicketByIdAsync(ticketId.ToString()), times: Times.Once());

            _basketRepositoryMock.Verify(x => x.StoreBasket(It.IsAny<ShoppingCart>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_WhenTicketNotFoundInCacheNorCatalog_ShouldReturnNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            var cart = ShoppingCart.Create(userId);
            var dto = new AddItemToBasketDto(ticketId, 2);

            _currentUserMock.SetupGet(x => x.UserId).Returns(userId);
            _basketRepositoryMock.Setup(x => x.GetBasket(userId)).ReturnsAsync(ShoppingCart.Create(userId));

            _cacheTicketRepositoryMock
                .Setup(x => x.ReadTicketFromCacheAsync(ticketId.ToString(), It.IsAny<CancellationToken>()))!
                .ReturnsAsync((string?)null);

            _grpcClientMock
                .Setup(x => x.GetTicketByIdAsync(ticketId.ToString()))
                .ReturnsAsync((TicketReadModel?)null);

            var command = new AddItemToBasketCommand(dto);
            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _cacheTicketRepositoryMock.Verify(x => x.StoreTicketInCacheAsync(It.IsAny<TicketReadModel>(), It.IsAny<CancellationToken>()), Times.Never);
            _distributedCacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _distributedCacheMock.Verify(x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
