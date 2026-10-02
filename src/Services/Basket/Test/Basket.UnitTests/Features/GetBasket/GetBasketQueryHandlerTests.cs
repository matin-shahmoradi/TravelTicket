using Basket.API.Basket.GetBasket;
using Basket.API.Data.Repository;
using Basket.API.Model;
using BuildingBlocks;
using BuildingBlocks.Abstractions;
using FluentAssertions;
using Moq;

namespace Basket.UnitTests.Features.GetBasket
{
    public class GetBasketQueryHandlerTests
    {
        private readonly Mock<ICurrentUser> _currentUserMock = new();
        private readonly Mock<IBasketRepository> _basketRepositoryMock = new();
        private readonly GetBasketQueryHandler _handler;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        public GetBasketQueryHandlerTests()
        {
            _handler = new GetBasketQueryHandler(
                _basketRepositoryMock.Object,
                _currentUserMock.Object);
        }

        [Fact]
        public async Task Handle_WhenBasketExist_ShouldReturnUserBasket()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cart = TestHelper.CreateUserBasketWithItems(userId);

            _currentUserMock.Setup(x => x.UserId).Returns(userId);

            _basketRepositoryMock.Setup(x => x.GetBasket(userId))
                .ReturnsAsync(cart);

            var query = new GetBasketQuery();

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeNull();

            _basketRepositoryMock.Verify(x => x.GetBasket(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenBasketIsNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _currentUserMock.Setup(x => x.UserId).Returns(userId);

            _basketRepositoryMock.Setup(x => x.GetBasket(userId)).ReturnsAsync((ShoppingCart?)null);

            var query = new GetBasketQuery();

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(Error.NotFoundError("Basket not found!"));

            _currentUserMock.Verify(x => x.UserId, Times.Once);
            _basketRepositoryMock.Verify(x => x.GetBasket(userId), Times.Once);
        }
    }
}
