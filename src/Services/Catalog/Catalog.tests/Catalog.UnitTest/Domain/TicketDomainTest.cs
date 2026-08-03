using Catalog.API.Domain.Exception;
using Catalog.API.Domain.ValueObjects;
using Catalog.API.Events;
using Catalog.API.Models;
using FluentAssertions;

namespace Catalog.Unit.test.Domain
{
    public class TicketDomainTest
    {
        [Fact]
        public void Create_ShouldCreateTicket_WithProvidedValues()
        {
            // Arrange
            var id = TicketId.New(Guid.NewGuid());
            var origin = "origin";
            var destination = "dest";
            var description = "Description";
            var travelDate = new DateTime(2026, 8, 10, 12, 30, 0, DateTimeKind.Utc);
            decimal price = 25000;

            var ticket = Ticket.Create(id, origin, destination, description, travelDate, price);

            // Assert
            ticket.Id.Should().Be(id);
            ticket.Origin.Should().Be(origin);
            ticket.Destination.Should().Be(destination);
            ticket.Description.Should().Be(description);
            ticket.TravelDate.Should().Be(travelDate);
            ticket.Price.Should().Be(price);
        }

        [Fact]
        public void Create_ShouldAdd_TicketCreatedEvent()
        {
            // Arrange
            var id = TicketId.New(Guid.NewGuid());

            // Act
            var ticket = Ticket.Create(id, "origin", "dest", "desc", new DateTime(2026, 8, 10, 12, 30, 0, DateTimeKind.Utc), 250);

            // Assert

            ticket.DomainEvents.Should().ContainSingle();

            ticket.DomainEvents.Single().Should().BeOfType<TicketCreatedEvent>();

        }

        [Fact]
        public void ClearDomainEvent_ShouldReturnEvent_AndClearDomainCollection()
        {
            // Arrange
            var ticket = CreateTicket();

            ticket.DomainEvents.Should().ContainSingle();

            // Act
            var clearedEvents = ticket.ClearDomainEvent();

            clearedEvents.Should().ContainSingle();
            clearedEvents[0].Should().BeOfType<TicketCreatedEvent>();

            ticket.DomainEvents.Should().BeEmpty();
        }
        [Fact]
        public void Update_ShouldUpdateAllProperties()
        {
            // Arrange
            var ticket = CreateTicket();
            ticket.ClearDomainEvent();

            var travelDate = new DateTime(2026, 8, 10, 12, 30, 0, DateTimeKind.Utc);

            // Act
            ticket.Update("Aligodarz", "dubai", "Aligodarz => dubai", travelDate, 1500);

            // Assert

            ticket.Origin.Should().Be("Aligodarz");
            ticket.Destination.Should().Be("dubai");
            ticket.Description.Should().Be("Aligodarz => dubai");
            ticket.TravelDate.Should().Be(travelDate);
            ticket.Price.Should().Be(1500);

            ticket.DomainEvents.Should().ContainSingle();
            ticket.DomainEvents.Single().Should().BeOfType<TicketUpdatedEvent>();
        }

        [Fact]
        public void Update_ShouldUpdateOnlyPrice()
        {
            var ticket = CreateTicket();
            ticket.ClearDomainEvent();

            var travelDate = new DateTime(2026, 8, 10, 12, 30, 0, DateTimeKind.Utc);
            ticket.ChangePrice(2000);

            ticket.Price.Should().Be(2000);
            ticket.DomainEvents.Should().ContainSingle();
            ticket.DomainEvents.Single().Should().BeOfType<TicketPriceChangedEvent>();
        }

        [Fact]
        public void ChangeOrigin_Should_UpdateOrigin()
        {
            // Arrange
            var ticket = CreateTicket();
            ticket.ClearDomainEvent();

            // Act
            ticket.ChangeOrigin("Alborz");

            // Assert

            ticket.Origin.Should().Be("Alborz");

            ticket.DomainEvents.Should().ContainSingle();
            ticket.DomainEvents.Single().Should().BeOfType<TicketUpdatedEvent>();
        }

        [Fact]
        public void ChangeOrigin_ShouldNotAddEvent_WhenOriginIsSame()
        {
            // Arrange
            var ticket = CreateTicket();
            ticket.ClearDomainEvent();

            // Act
            ticket.ChangeOrigin("Tehran");

            // Assert
            ticket.DomainEvents.Should().BeEmpty();
            ticket.DomainEvents.Should().NotContain(x => x is TicketUpdatedEvent);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ChangeOrigin_WhenOriginIsInvalid_ShouldThrowDomainException(string newOrigin)
        {
            // Arrange
            var ticket = CreateTicket();

            // Act
            var action = () => ticket.ChangeOrigin(newOrigin);

            // Assert
            action.Should().Throw<DomainException>().WithMessage("new origin can't be empty");
        }

        [Fact]
        public void ChangeDestination_Should_UpdateDestination()
        {
            // Arrange
            var ticket = CreateTicket();
            ticket.ClearDomainEvent();

            // Act
            ticket.ChangeDestination("Hormozgan");

            // Assert

            ticket.Destination.Should().Be("Hormozgan");

            ticket.DomainEvents.Should().ContainSingle();
            ticket.DomainEvents.Single().Should().BeOfType<TicketUpdatedEvent>();
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ChangeDestination_WhenDestinationIsInvalid_ShouldThrowDomainException(string newDestination)
        {
            // Arrange
            var ticket = CreateTicket();

            // Act
            var action = () => ticket.ChangeDestination(newDestination);

            // Assert
            action.Should().Throw<DomainException>().WithMessage("new destination can't be empty");
        }
        [Fact]
        public void ChangeDestination_ShouldNotAddEvent_WhenDestinationIsSame()
        {
            // Arrange
            var ticket = CreateTicket();
            ticket.ClearDomainEvent();

            // Act
            ticket.ChangeDestination("Shiraz");

            // Assert
            ticket.DomainEvents.Should().BeEmpty();
            ticket.DomainEvents.Should().NotBeOfType<TicketUpdatedEvent>();
        }
        [Fact]
        public void ChangeDescription_Should_UpdateOnlyDescription()
        {
            // Arrange
            var ticket = CreateTicket();
            ticket.ClearDomainEvent();

            // Act
            ticket.ChangeDestination("Hormozgan");

            // Assert
            ticket.Origin.Should().Be("Tehran");
            ticket.Description.Should().Be("VIP bus ticket");
            ticket.Price.Should().Be(1500);
            ticket.TravelDate.Should().Be(new DateTime(2026, 8, 10, 12, 30, 0, DateTimeKind.Utc));

            ticket.Destination.Should().Be("Hormozgan");

            ticket.DomainEvents.Should().ContainSingle();
            ticket.DomainEvents.Single().Should().BeOfType<TicketUpdatedEvent>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ChangeDescription_WhenDescriptionIsInvalid_ShouldThrowDomainException(string newDescription)
        {
            // Arrange
            var ticket = CreateTicket();

            // Act
            var action = () => ticket.ChangeDescription(newDescription);

            // Assert
            action.Should().Throw<DomainException>().WithMessage("new description can't be empty");
        }

        [Fact]
        public void ChangeDescription_ShouldNotAddEvent_WhenDescriptionIsSame()
        {
            // Arrange
            var ticket = CreateTicket();
            ticket.ClearDomainEvent();

            // Act
            ticket.ChangeDescription("VIP bus ticket");

            // Assert
            ticket.DomainEvents.Should().BeEmpty();
            ticket.DomainEvents.Should().NotBeOfType<TicketUpdatedEvent>();
        }

        [Fact]
        public void ChangeTravelDate_Should_UpdateOnlyTravelDate()
        {
            // Arrange
            var ticket = CreateTicket();
            ticket.ClearDomainEvent();

            // Act
            ticket.ChangeTravelDate(new DateTime(2026, 7, 9, 12, 45, 0, DateTimeKind.Utc));

            // Assert
            ticket.Origin.Should().Be("Tehran");
            ticket.Destination.Should().Be("Shiraz");
            ticket.Description.Should().Be("VIP bus ticket");
            ticket.Price.Should().Be(1500);

            ticket.TravelDate.Should().Be(new DateTime(2026, 7, 9, 12, 45, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void ChangeTravelDate_ShouldNotAddEvent_WhenDestinationIsSame()
        {
            // Arrange
            var ticket = CreateTicket();
            ticket.ClearDomainEvent();

            // Act
            ticket.ChangeTravelDate(new DateTime(2026, 8, 10, 12, 30, 0, DateTimeKind.Utc));

            // Assert
            ticket.DomainEvents.Should().BeEmpty();
        }

        private static Ticket CreateTicket(decimal price = 1500)
        {
            return Ticket.Create(
                TicketId.New(Guid.NewGuid()),
                "Tehran",
                "Shiraz",
                "VIP bus ticket",
                new DateTime(2026, 8, 10, 12, 30, 0, DateTimeKind.Utc),
                price);
        }
    }
}
