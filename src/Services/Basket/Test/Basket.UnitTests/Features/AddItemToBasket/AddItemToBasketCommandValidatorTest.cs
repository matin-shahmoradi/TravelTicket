using Basket.API.Basket.AddItemToBasket;
using Basket.API.Common.Dtos;
using FluentAssertions;

namespace Basket.UnitTests.Features.AddItemToBasket
{
    public class AddItemToBasketCommandValidatorTest
    {
        private readonly AddItemToBasketCommandValidator _validator = new();

        [Fact]
        public void Validator_ShouldPass_WhenRequestIsValid()
        {
            // Arrange
            var dto = new AddItemToBasketDto(Guid.NewGuid(), 5);
            var command = new AddItemToBasketCommand(dto);

            // Act
            var result = _validator.Validate(command);

            // Assert

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Validator_ShouldFail_WhenTicketIdIsEmpty()
        {
            // Arrange
            var dto = new AddItemToBasketDto(Guid.Empty, 1);

            var command = new AddItemToBasketCommand(dto);

            // Act
            var result = _validator.Validate(command);

            // Assert

            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();

            result.Errors.Select(x => x.ErrorMessage).Should().BeEquivalentTo("ticket id cant be empty.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Validator_ShouldFail_WhenQuantityIsInvalid(int quantity)
        {
            // Arrange
            var dto = new AddItemToBasketDto(Guid.NewGuid(), quantity);

            var command = new AddItemToBasketCommand(dto);

            // Act
            var result = _validator.Validate(command);

            // Assert

            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();

            result.Errors.Select(x => x.ErrorMessage).Should().BeEquivalentTo("Quantity should be greater than zero");
        }
    }
}
