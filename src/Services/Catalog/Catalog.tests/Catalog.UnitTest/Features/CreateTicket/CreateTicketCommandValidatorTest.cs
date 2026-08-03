using Catalog.API.Tickets.CreateTicket;
using FluentAssertions;

namespace Catalog.Unit.test.Features.CreateTicket
{
    public sealed class CreateTicketCommandValidatorTest
    {
        private readonly CreateTicketCommandValidator _validator = new();

        [Fact]
        public void Validator_ShouldPass_WhenRequsetIsValid()
        {
            // Arrange
            var createRequestDto = new CreateTicketRequestDTO("Mashhad", "Tabriz", "description", DateTime.UtcNow, 25000);
            var command = new CreateTicketCommand(createRequestDto);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validator_ShouldFailed_When_Origin_IsNotValid(string? origin)
        {
            // Arrange
            var createRequestDto = new CreateTicketRequestDTO(origin!, "Tabriz", "description", DateTime.UtcNow, 25000);
            var command = new CreateTicketCommand(createRequestDto);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validator_ShouldFailed_When_Destination_IsNotValid(string? destination)
        {
            // Arrange
            var createRequestDto = new CreateTicketRequestDTO("Mashhad", destination!, "description", DateTime.UtcNow, 25000);
            var command = new CreateTicketCommand(createRequestDto);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validator_ShouldFailed_When_Description_IsNotValid(string description)
        {
            // Arrange
            var createRequestDto = new CreateTicketRequestDTO("Mashhad", "Tabriz", description, DateTime.UtcNow, 25000);
            var command = new CreateTicketCommand(createRequestDto);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(null)]
        public void Validator_ShouldFailed_When_DateTime_IsNotValid(DateTime dateTime)
        {
            // Arrange
            var createRequestDto = new CreateTicketRequestDTO("Mashhad", "Tabriz", "description", dateTime, 25000);
            var command = new CreateTicketCommand(createRequestDto);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(0)]
        public void Validator_ShouldFailed_When_Price_IsNotValid(decimal price)
        {
            // Arrange
            var createRequestDto = new CreateTicketRequestDTO("Mashhad", "Tabriz", "description", DateTime.UtcNow, price);
            var command = new CreateTicketCommand(createRequestDto);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();
        }
    }
}
