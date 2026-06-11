using AuthService.Auth.ConfirmEmail;
using FluentAssertions;

namespace AuthService.UnitTests.Features.ConfirmEmail
{
    public class ConfirmEmailCommandValidatorTest
    {
        private readonly ConfirmEmailCommandValidator _validator = new();

        [Fact]
        public void Validate_ShouldPass_WhenUserIdAndTokenAreValid()
        {
            var command = new ConfirmEmailCommand
                (
                    UserId: Guid.NewGuid().ToString(),
                    EmailConfirmationToken: Guid.NewGuid().ToString()
                );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();

            result.Errors.Count.Should().Be(0);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ShouldFail_WhenUserIdIsEmpty(string userId)
        {
            var command = new ConfirmEmailCommand(
                UserId: userId,
                EmailConfirmationToken: Guid.NewGuid().ToString()
                );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().NotBe(0);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ShouldFail_WhenTokenIsEmpty(string emailConfirmationToken)
        {
            var command = new ConfirmEmailCommand(
                UserId: Guid.NewGuid().ToString(),
                EmailConfirmationToken: emailConfirmationToken
                );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().NotBe(0);
        }
    }

}
