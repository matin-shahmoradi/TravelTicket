using Catalog.API.Tickets.DeleteTicket;
using FluentAssertions;

namespace Catalog.Unit.test.Features.DeleteTicket
{
    public class DeleteTicketCommandValidatorTest
    {
        private readonly DeleteTicketCommandValidator _validator = new();

        [Fact]
        public void Validator_ShouldPass_WhenTicketIdIsValid()
        {
            // Arrange
            var ticketId = Guid.NewGuid();

            var command = new DeleteTicketCommand(ticketId);
            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void Validator_ShouldFail_WhenTicketIdIsInvalid(Guid id)
        {
            // Arrange
            var ticketId = id;

            var command = new DeleteTicketCommand(ticketId);
            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }
    }
}
