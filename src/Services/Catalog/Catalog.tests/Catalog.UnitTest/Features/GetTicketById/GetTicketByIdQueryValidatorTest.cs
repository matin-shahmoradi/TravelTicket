using Catalog.API.Tickets.GetTicketsById;
using FluentAssertions;

namespace Catalog.Unit.test.Features.GetTicketById
{
    public sealed class GetTicketByIdQueryValidatorTest
    {
        private readonly GetTicketByIdQueryValidator _validator = new();

        [Fact]
        public void Validator_ShouldPass_WhenRequestIsValid()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var command = new GetTicketByIdQuery(ticketId);

            // Act 
            var result = _validator.Validate(command);

            // Assert

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validator_ShouldFail_WhenRequestIsInvalid()
        {
            // Arrange
            var ticketId = Guid.Empty;
            var command = new GetTicketByIdQuery(ticketId);

            // Act 
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }
    }
}
