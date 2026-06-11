using AuthService.Auth.GetUsersByEmail;
using FluentAssertions;

namespace AuthSerive.UnitTests.Features.Users.GetUserByEmailTests
{
    public class GetUserByEmailQueryValidatorTest
    {
        private readonly GetUserByEmailQueryValidator _validator = new();
        [Fact]
        public void Validate_ShouldPass_WhenEmailIsValid()
        {
            // Arrange
            var query = new GetUserByEmailQuery("example@gmail.com");

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("invalid-email")]
        [InlineData("user@")]
        [InlineData("@invalidEmail")]
        public void Validate_ShouldFail_WhenEmailIsInvalid(string email)
        {
            // Arrange 
            var query = new GetUserByEmailQuery(email);

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }
    }
}
