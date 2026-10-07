using Basket.API.Basket.DeleteBasket;
using Basket.API.Data.Repository;
using Basket.API.Model;
using BuildingBlocks.Abstractions;
using FluentAssertions;
using Moq;

namespace Basket.UnitTests.Features.DeleteBasket
{
    public class DeleteBasketCommandHandlerTests
    {
        private readonly Mock<IBasketRepository> _basketRepositoryMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();
        private readonly DeleteBasketCommandHandler _handler;

        public DeleteBasketCommandHandlerTests()
        {
            _handler = new DeleteBasketCommandHandler(
                _basketRepositoryMock.Object,
                _currentUserMock.Object);
        }

        [Fact]
        public async Task Handle_WhenBasketExists_ShouldDeleteBasketSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cart = TestHelper.CreateUserBasketWithItems(userId);
            var command = new DeleteBasketCommand();

            _currentUserMock.Setup(x => x.UserId).Returns(userId);

            _basketRepositoryMock
                .Setup(x => x.GetBasket(userId))
                .ReturnsAsync(cart);

            _basketRepositoryMock
                .Setup(x => x.DeleteBasket(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Contain(cart.Id.ToString());
            result.Value.Should().Contain("deleted successfully");
            result.Error.Should().BeNull();

            _basketRepositoryMock.Verify(
                x => x.GetBasket(userId),
                Times.Once);

            _basketRepositoryMock.Verify(
                x => x.DeleteBasket(userId),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenBasketNotFound_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteBasketCommand();

            _currentUserMock.Setup(x => x.UserId).Returns(userId);

            _basketRepositoryMock
                .Setup(x => x.GetBasket(userId))
                .ReturnsAsync((ShoppingCart?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Value.Message.Should().Be("Basket Not Found!");

            _basketRepositoryMock.Verify(
                x => x.GetBasket(userId),
                Times.Once);

            _basketRepositoryMock.Verify(
                x => x.DeleteBasket(It.IsAny<Guid>()),
                Times.Never);
        }
    }
}
