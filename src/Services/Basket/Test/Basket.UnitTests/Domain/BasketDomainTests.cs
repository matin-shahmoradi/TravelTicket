using Basket.API.Events;
using Basket.API.Model;
using FluentAssertions;

namespace Basket.UnitTests.Domain
{
    public class BasketDomainTests
    {
        [Fact]
        public void Create_WithValidCustomerId_ShouldCreateShoppingCart()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            // Act
            var cart = ShoppingCart.Create(customerId);

            // Assert
            cart.Should().NotBeNull();
            cart.CustomerId.Should().Be(customerId);
            cart.Id.Should().NotBeNull();
            cart.Id.Value.Should().NotBe(Guid.Empty);
            cart.Items.Should().BeEmpty();
            cart.TotalPrice.Should().Be(0);
        }

        [Fact]
        public void AddItem_WhenItemDoesNotExist_ShouldAddNewItem()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var ticketId = Guid.NewGuid();
            var quantity = 2;
            decimal price = 100;

            // Act
            cart.AddItem(ticketId, quantity, price);

            // Assert
            cart.Items.Should().HaveCount(1);
            cart.Items[0].TicketId.Should().Be(ticketId);
            cart.Items[0].Quantity.Should().Be(quantity);
            cart.Items[0].Price.Should().Be(price);
            cart.TotalPrice.Should().Be(200); // 2 * 100
        }

        [Fact]
        public void AddItem_WhenItemAlreadyExists_ShouldIncreaseQuantity()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var ticketId = Guid.NewGuid();
            cart.AddItem(ticketId, 2, 100);

            // Act
            cart.AddItem(ticketId, 3, 100);

            // Assert
            cart.Items.Should().HaveCount(1);
            cart.Items[0].Quantity.Should().Be(5); // 2 + 3
            cart.TotalPrice.Should().Be(500); // 5 * 100
        }

        [Fact]
        public void AddItem_WithNegativeQuantity_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var ticketId = Guid.NewGuid();

            // Act
            var act = () => cart.AddItem(ticketId, -1, 100);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void AddItem_WithNegativePrice_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var ticketId = Guid.NewGuid();

            // Act
            var act = () => cart.AddItem(ticketId, 1, -50);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void AddItem_WithZeroPrice_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var ticketId = Guid.NewGuid();

            // Act
            var act = () => cart.AddItem(ticketId, 1, 0);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void AddItem_MultipleItems_ShouldCalculateTotalPriceCorrectly()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var ticket1 = Guid.NewGuid();
            var ticket2 = Guid.NewGuid();

            // Act
            cart.AddItem(ticket1, 2, 100); // 200
            cart.AddItem(ticket2, 3, 50);  // 150

            // Assert
            cart.Items.Should().HaveCount(2);
            cart.TotalPrice.Should().Be(350); // 200 + 150
        }

        [Fact]
        public void RemoveItem_WhenItemExists_ShouldRemoveItem()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var ticketId = Guid.NewGuid();
            cart.AddItem(ticketId, 2, 100);

            // Act
            cart.RemoveItem(ticketId);

            // Assert
            cart.Items.Should().BeEmpty();
            cart.TotalPrice.Should().Be(0);
        }

        [Fact]
        public void RemoveItem_WhenItemDoesNotExist_ShouldDoNothing()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var existingTicketId = Guid.NewGuid();
            var nonExistingTicketId = Guid.NewGuid();
            cart.AddItem(existingTicketId, 2, 100);

            // Act
            cart.RemoveItem(nonExistingTicketId);

            // Assert
            cart.Items.Should().HaveCount(1);
            cart.Items[0].TicketId.Should().Be(existingTicketId);
        }

        [Fact]
        public void RemoveItem_WhenMultipleItemsExist_ShouldRemoveOnlySpecifiedItem()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var ticket1 = Guid.NewGuid();
            var ticket2 = Guid.NewGuid();
            cart.AddItem(ticket1, 2, 100);
            cart.AddItem(ticket2, 3, 50);

            // Act
            cart.RemoveItem(ticket1);

            // Assert
            cart.Items.Should().HaveCount(1);
            cart.Items[0].TicketId.Should().Be(ticket2);
            cart.TotalPrice.Should().Be(150);
        }

        [Fact]
        public void CheckoutBasket_WhenCartHasItems_ShouldAddDomainEvent()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var ticket1 = Guid.NewGuid();
            var ticket2 = Guid.NewGuid();
            cart.AddItem(ticket1, 2, 100);
            cart.AddItem(ticket2, 1, 50);

            // Act
            cart.CheckoutBasket();

            // Assert
            cart.DomainEvents.Should().HaveCount(1);
            var domainEvent = cart.DomainEvents[0] as BasketCheckOutEvent;
            domainEvent.Should().NotBeNull();
            domainEvent!.Items.Should().HaveCount(2);

            domainEvent.Items[0].TicketId.Should().Be(ticket1);
            domainEvent.Items[0].Quantity.Should().Be(2);
            domainEvent.Items[0].Price.Should().Be(100);

            domainEvent.Items[1].TicketId.Should().Be(ticket2);
            domainEvent.Items[1].Quantity.Should().Be(1);
            domainEvent.Items[1].Price.Should().Be(50);
        }

        [Fact]
        public void CheckoutBasket_WhenCartIsEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());

            // Act
            var act = () => cart.CheckoutBasket();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Cart is empty.");
        }

        [Fact]
        public void CheckoutBasket_WhenCalledMultipleTimes_ShouldAddMultipleDomainEvents()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());
            cart.AddItem(Guid.NewGuid(), 1, 100);

            // Act
            cart.CheckoutBasket();
            cart.CheckoutBasket();

            // Assert
            cart.DomainEvents.Should().HaveCount(2);
            cart.DomainEvents.Should().AllBeOfType<BasketCheckOutEvent>();
        }

        [Fact]
        public void TotalPrice_WhenCartIsEmpty_ShouldBeZero()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());

            // Assert
            cart.TotalPrice.Should().Be(0);
        }

        [Fact]
        public void TotalPrice_AfterRemovingAllItems_ShouldBeZero()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var ticketId = Guid.NewGuid();
            cart.AddItem(ticketId, 2, 100);

            // Act
            cart.RemoveItem(ticketId);

            // Assert
            cart.TotalPrice.Should().Be(0);
        }

        [Fact]
        public void Items_ShouldBeReadOnly()
        {
            // Arrange
            var cart = ShoppingCart.Create(Guid.NewGuid());

            // Assert
            cart.Items.Should().BeAssignableTo<IReadOnlyList<ShoppingCartItem>>();
        }
    }
}
