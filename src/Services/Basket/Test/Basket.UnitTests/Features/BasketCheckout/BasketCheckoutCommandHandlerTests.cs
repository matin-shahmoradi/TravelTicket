using Basket.API.basket.BasketCheckout;
using Basket.API.basket.Checkout;
using Basket.API.Data.Repository;
using Basket.API.Model;
using BuildingBlocks.Abstractions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Basket.UnitTests.Features.BasketCheckout
{
    public class BasketCheckoutCommandHandlerTests
    {
        private readonly Mock<ICurrentUser> _currentUserMock = new();
        private readonly Mock<IBasketRepository> _basketRepositotyMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly BasketCheckOutCommandHandler _handler;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        public BasketCheckoutCommandHandlerTests()
        {
            _handler = new BasketCheckOutCommandHandler(
                _currentUserMock.Object,
                _basketRepositotyMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WhenBasketExist_ShouldCheckoutBasket()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cart = TestHelper.CreateUserBasketWithItems(userId);

            _currentUserMock.Setup(x => x.UserId).Returns(userId);

            _basketRepositotyMock.Setup(x => x.GetBasket(
                customerId: userId,
                tracking: QueryTrackingBehavior.TrackAll,
                cancellation: It.IsAny<CancellationToken>()
                ))
                    .ReturnsAsync(cart);

            var command = new BasketCheckOutCommand();

            // Act

            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();

            result.Error.Should().BeNull();

            _basketRepositotyMock.Verify(
                x => x.GetBasket(
                    customerId: userId,
                    tracking: QueryTrackingBehavior.TrackAll,
                    cancellation: It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenBasketNotFound_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _currentUserMock.Setup(x => x.UserId).Returns(userId);

            _basketRepositotyMock.Setup(x => x.GetBasket(
                customerId: userId,
                tracking: QueryTrackingBehavior.TrackAll,
                cancellation: It.IsAny<CancellationToken>()))
                .ReturnsAsync((ShoppingCart?)null);

            var command = new BasketCheckOutCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Value.Message.Should().Be("basket Not Found!");

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
