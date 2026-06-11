using AuthService.Auth.ConfirmEmail;
using AuthService.Interfaces;
using AuthService.Model;
using BuildingBlocks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using System.Text;

namespace AuthService.UnitTests.Features.ConfirmEmail
{
    public class ConfirmEmailCommandHandlerTest
    {
        private readonly Mock<IUserManagerCommandService> _mockUserManagerService;
        private readonly Mock<IUserManagerQueryService> _mockUserQueryService;
        private readonly ConfirmEmailCommandHandler _handler;
        private readonly CancellationToken _cancellationToken;

        public ConfirmEmailCommandHandlerTest()
        {
            _mockUserManagerService = new Mock<IUserManagerCommandService>();
            _mockUserQueryService = new Mock<IUserManagerQueryService>();
            _handler = new(_mockUserManagerService.Object, _mockUserQueryService.Object);
            _cancellationToken = CancellationToken.None;
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenEmailConfirmationSucceeds()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            string emailConfirmationToken = Guid.NewGuid().ToString();

            string encodedEmailConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));

            var command = new ConfirmEmailCommand(userId, encodedEmailConfirmationToken);

            var user = new ApplicationUser
            {
                Id = userId,
                Email = "test@gmail.com",
                EmailConfirmed = false,
            };

            _mockUserQueryService
                .Setup(x => x.GetUserByIdAsync(
                    userId: userId,
                    cancellationToken: _cancellationToken))
                .ReturnsAsync(user);

            _mockUserManagerService
                .Setup(x => x.ConfirmUserEmailAsync(
                   user: It.Is<ApplicationUser>(u => u.Id == user.Id),
                   emailConfirmationToken: It.Is<string>(e => e == emailConfirmationToken)))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeTrue();

            result.Value.Should().NotBeNull();

            result.Error.Should().BeNull();

            _mockUserQueryService
               .Verify(x => x.GetUserByIdAsync(
                    userId: userId,
                    cancellationToken: _cancellationToken),
                  Times.Once);

            _mockUserManagerService
               .Verify(x => x.ConfirmUserEmailAsync(
                   user: It.Is<ApplicationUser>(u => u.Id == user.Id),
                   emailConfirmationToken: It.Is<string>(e => e == emailConfirmationToken)),
                  Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenGetUserByIdReturnsNull()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            string emailConfirmationToken = Guid.NewGuid().ToString();

            string encodedEmailConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));

            var command = new ConfirmEmailCommand(userId, encodedEmailConfirmationToken);

            _mockUserQueryService
                .Setup(x => x.GetUserByIdAsync(
                    userId: userId,
                    cancellationToken: _cancellationToken))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeFalse();

            result.Value.Should().BeNull();

            result.Error.Should().NotBeNull();
            result.Error.Value.ErrorType.Should().Be(ErrorType.NOT_FOUND);

            _mockUserQueryService
               .Verify(x => x.GetUserByIdAsync(
                    userId: userId,
                    cancellationToken: _cancellationToken),
                  Times.Once);

            _mockUserManagerService
               .Verify(x => x.ConfirmUserEmailAsync(
                   user: It.IsAny<ApplicationUser>(),
                   emailConfirmationToken: It.IsAny<string>()),
                  Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenEmailConfirmationFails()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            string emailConfirmationToken = Guid.NewGuid().ToString();

            string encodedEmailConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));

            var command = new ConfirmEmailCommand(userId, encodedEmailConfirmationToken);

            var user = new ApplicationUser
            {
                Id = userId,
                Email = "test@gmail.com",
                EmailConfirmed = false,
            };
            _mockUserQueryService
                .Setup(x => x.GetUserByIdAsync(
                    userId: userId,
                    cancellationToken: _cancellationToken))
                .ReturnsAsync(user);

            _mockUserManagerService
                .Setup(x => x.ConfirmUserEmailAsync(
                   user: It.Is<ApplicationUser>(u => u.Id == user.Id),
                   emailConfirmationToken: It.Is<string>(e => e == emailConfirmationToken)))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeFalse();

            result.Value.Should().BeNull();

            result.Error.Should().NotBeNull();
            result.Error.Value.ErrorType.Should().Be(ErrorType.INTERNAL_SERVER_ERROR);

            _mockUserQueryService
               .Verify(x => x.GetUserByIdAsync(
                    userId: userId,
                    cancellationToken: _cancellationToken),
                  Times.Once);

            _mockUserManagerService
               .Verify(x => x.ConfirmUserEmailAsync(
                   user: It.Is<ApplicationUser>(u => u.Id == user.Id),
                   emailConfirmationToken: It.Is<string>(e => e == emailConfirmationToken)),
                  Times.Once);
        }
    }
}
