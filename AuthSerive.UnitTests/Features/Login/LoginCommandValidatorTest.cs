using AuthService.Auth.Login;
using AuthService.Model.DTOs.LoginDtos;
using FluentAssertions;

namespace AuthService.UnitTests.Features.Login
{
    public class LoginCommandValidatorTest
    {
        private readonly LoginCommandValidator _validator = new();
        [Fact]
        public void Validate_ShouldPass_WhenEmailAndPasswordAreValid()
        {
            // Arrange
            string email = "ValidEmail@gmail.com";
            string password = "ValidPassword12@";

            var request = new LoginRequest(email, password);
            var command = new LoginCommand(request);

            // Act
            var result = _validator.Validate(command);

            // Assert

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("@notvalid.com")]
        [InlineData("notValid@")]
        public void Validate_ShouldFail_When_EmailIsNotValid(string email)
        {
            // Arrange
            string password = "ValidPassword12@";

            var request = new LoginRequest(email, password);
            var command = new LoginCommand(request);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("1234567")]
        [InlineData("notValid@")]
        [InlineData("NOTVALID")]
        [InlineData("notvalid")]
        [InlineData("notValid1")]
        public void Validate_ShouldFail_When_PasswordIsNotValid(string password)
        {
            // Arrange
            string email = "matiu@gmail.com";

            var request = new LoginRequest(email, password);
            var command = new LoginCommand(request);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();
        }
    }
}
