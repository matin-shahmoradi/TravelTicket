using Catalog.API.Tickets.UpdateTicket;
using FluentAssertions;

namespace Catalog.Unit.test.Features.UpdateTicket
{
    public sealed class UpdateTicketCommandValidatorTest
    {
        private readonly UpdateTicketCommandValidator _validator = new();
        private readonly Guid _id = new Guid("1C20722C-58F7-4523-8553-B1AAEB4DE02D");

        private UpdateTicketCommand CreateUpdateCommand(
            DateTime travelDate,
            string? origin = "TEHRAN",
            string? destination = "MARKAZI",
            string? description = "TEHRAN => MARKAZI(ARAK)",
            decimal? price = 3600)
        {
            var ticketRequestDto = new UpdateTicketRequestDTO(origin, destination, description, travelDate, price);
            return new UpdateTicketCommand(_id, ticketRequestDto);
        }

        [Fact]
        public void Validator_ShouldPass_WhenRequestIsValid()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var updateRequestDto = new UpdateTicketRequestDTO(
                "TEHRAN",
                "MARKAZI",
                "TEHRAN => MARKAZI(ARAK)"
                , new DateTime(2026, 10, 1, 10, 36, 0, DateTimeKind.Utc),
                3600);

            var command = new UpdateTicketCommand(ticketId, updateRequestDto);

            // Act
            var result = _validator.Validate(command);

            // Assert

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validator_ShouldFail_WhenTicketIdIsInvalid()
        {
            // Arrange
            var ticketId = Guid.Empty;
            var updateRequestDto = new UpdateTicketRequestDTO(
                "TEHRAN",
                "MARKAZI",
                "TEHRAN => MARKAZI(ARAK)"
                , new DateTime(2026, 10, 1, 10, 36, 0, DateTimeKind.Utc),
                3600);

            var command = new UpdateTicketCommand(ticketId, updateRequestDto);

            // Act
            var result = _validator.Validate(command);

            // Assert

            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        public void Validator_ShouldFail_WhenOriginIsInvalid(string origin)
        {
            // Arrange
            var command = CreateUpdateCommand(
                travelDate: new DateTime(2026, 10, 1, 10, 36, 0, DateTimeKind.Utc),
                origin: origin);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        public void Validator_ShouldFail_WhenDestinationIsInvalid(string destination)
        {
            // Arrange
            var command = CreateUpdateCommand(
                travelDate: new DateTime(2026, 10, 1, 10, 36, 0, DateTimeKind.Utc),
                destination: destination);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        public void Validator_ShouldFail_WhenDescriptionIsInvalid(string description)
        {
            // Arrange
            var command = CreateUpdateCommand(
                travelDate: new DateTime(2026, 10, 1, 10, 36, 0, DateTimeKind.Utc),
                description: description);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(-1)]
        public void Validator_ShouldFail_WhenPriceIsInvalid(decimal price)
        {
            // Arrange
            var command = CreateUpdateCommand(
                travelDate: new DateTime(2026, 10, 1, 10, 36, 0, DateTimeKind.Utc),
                price: price);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }
    }
}
