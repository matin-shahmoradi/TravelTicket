using AuthService.Auth.Register;
using AuthService.Model.DTOs.RegisterDtos;
using FluentAssertions;

namespace AuthService.UnitTests.Features.Register
{
    public class UserRegisterCommandValidatorTest
    {
        private readonly UserRegisterCommandValidator _validator = new();

        [Fact]
        public void Validate_ShouldPass_WhenRequestIsValid()
        {
            // Arrange
            var registerRequestDto = new RegisterRequestDto
            (
                Email: "ImValidEmail@gmail.com",
                Password: "Validpassword12!",
                Firstname: "ValidFirstName",
                Lastname: "ValidLastName"
            );

            var command = new UserRegisterCommand(registerRequestDto);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("@notValid.com")]
        [InlineData("notValid@")]
        public void Validate_ShouldFail_WhenEmailIsNotValid(string email)
        {
            var registerRequestDto = new RegisterRequestDto
            (
                Email: email,
                Password: "Validpassword12!",
                Firstname: "ValidFirstName",
                Lastname: "ValidLastName"
            );

            var command = new UserRegisterCommand(registerRequestDto);

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
        [InlineData("notvalidpass")]
        [InlineData("Notvalidpass")]
        [InlineData("notvalidpass1")]
        [InlineData("notvalidpass@")]
        [InlineData("Notvalidpass@")]
        public void Validate_ShouldFail_WhenPasswordIsNotValid(string password)
        {
            var registerRequestDto = new RegisterRequestDto
            (
                Email: "ValidEmail@gmail.com",
                Password: password,
                Firstname: "ValidFirstName",
                Lastname: "ValidLastName"
            );

            var command = new UserRegisterCommand(registerRequestDto);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Validate_ShouldFail_WhenFirstnameIsEmpty()
        {
            var registerRequestDto = new RegisterRequestDto
           (
               Email: "ValidEmail@gmail.com",
               Password: "Validpassword12!",
               Firstname: " ",
               Lastname: "ValidLastName"
           );

            var command = new UserRegisterCommand(registerRequestDto);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Validate_ShouldFail_WhenLastnameIsEmpty()
        {
            var registerRequestDto = new RegisterRequestDto
           (
               Email: "ValidEmail@gmail.com",
               Password: "Validpassword12!",
               Firstname: "ValidFirstName",
               Lastname: " "
           );

            var command = new UserRegisterCommand(registerRequestDto);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();
        }
    }
}

