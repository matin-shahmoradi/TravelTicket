using AuthService.Auth.Login;
using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Model.DTOs.LoginDtos;
using BuildingBlocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AuthService.UnitTests.Features.Login
{
    public class LoginCommandHandlerTest
    {
        private readonly Mock<IUserManagerQueryService> _mockUserQueryService;
        private readonly Mock<IUserRoleQueryService> _mockUserRoleQueryService;
        private readonly Mock<IUserSignInManagerService> _mockUserSignInManagerService;
        private readonly Mock<IJsonWebTokenService> _mockJsonWebTokenService;
        private readonly Mock<ILogger<LoginCommandHandler>> _mockLogger;
        private readonly LoginCommandHandler _handler;
        public LoginCommandHandlerTest()
        {
            _mockUserQueryService = new Mock<IUserManagerQueryService>();
            _mockUserRoleQueryService = new Mock<IUserRoleQueryService>();
            _mockUserSignInManagerService = new Mock<IUserSignInManagerService>();
            _mockJsonWebTokenService = new Mock<IJsonWebTokenService>();
            _mockLogger = new Mock<ILogger<LoginCommandHandler>>();

            _handler = new LoginCommandHandler(
                _mockUserQueryService.Object,
                _mockUserRoleQueryService.Object,
                _mockUserSignInManagerService.Object,
                _mockJsonWebTokenService.Object,
                _mockLogger.Object
                );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserCredentialsIsValid()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "ValidUsername",
                Email = "validaPassword@gmail.com",
                FirstName = "IM Valid",
                LastName = "Validian",
                PhoneNumber = "09900000000",
            };

            IReadOnlyList<string> role = new List<string>
            {
                "User"
            };

            var cancelationToken = CancellationToken.None;

            var generatedAccessToken = Guid.NewGuid().ToString();

            var loginRequest = new LoginRequest(user.Email, "ValidaPassword123@");
            var command = new LoginCommand(loginRequest);

            _mockUserQueryService
                .Setup(x => x.GetUserByEmailAsync(user.Email, cancelationToken))
                .ReturnsAsync(user);

            _mockUserSignInManagerService
                .Setup(x => x.ValidateUserPassword(user, loginRequest.Password))
                .ReturnsAsync(true);

            _mockUserRoleQueryService
                .Setup(x => x.GetUserRolesAsync(user.Id))
                .ReturnsAsync(role);

            _mockJsonWebTokenService
                .Setup(x => x.GenerateAccessToken(user, role))
                .Returns(generatedAccessToken);

            // Act
            var result = await _handler.Handle(command, cancelationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();

            result.Value.Should().NotBeNull();
            result.Value.email.Should().Be(user.Email);
            result.Value.token.Should().Be(generatedAccessToken);

            result.Error.Should().BeNull();

            _mockUserQueryService
                .Verify(x => x.GetUserByEmailAsync(user.Email, cancelationToken), Times.Once);

            _mockUserSignInManagerService
                .Verify(x => x.ValidateUserPassword(user, loginRequest.Password), Times.Once);

            _mockUserRoleQueryService
                .Verify(x => x.GetUserRolesAsync(user.Id), Times.Once);

            _mockJsonWebTokenService
                .Verify(x => x.GenerateAccessToken(user, role), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIsNotExist()
        {
            // Arrange
            var email = "NotExistUser@gmail.com";

            var loginRequest = new LoginRequest(
                Email: email,
                Password: "MatiuMatiu12@");

            var command = new LoginCommand(loginRequest);
            var cancelationToken = CancellationToken.None;

            _mockUserQueryService
               .Setup(x => x.GetUserByEmailAsync(loginRequest.Email, cancelationToken))
               .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _handler.Handle(command, cancelationToken);

            // Assert

            result.IsSuccess.Should().BeFalse();

            result.Error.Should().NotBeNull();
            result.Error.Value.ErrorType.Should().Be(ErrorType.UNAUTHORIZED_ERROR);


            _mockUserQueryService
                .Verify(x => x.GetUserByEmailAsync(
                    email: loginRequest.Email,
                    cancellationToken: cancelationToken),
                    Times.Once);

            _mockUserSignInManagerService
               .Verify(x => x.ValidateUserPassword(
                   user: It.IsAny<ApplicationUser>(),
                   password: It.IsAny<string>()),
                   Times.Never);

            _mockUserRoleQueryService
                .Verify(x => x.GetUserRolesAsync(
                    userId: It.IsAny<string>()),
                    Times.Never);

            _mockJsonWebTokenService
                .Verify(x => x.GenerateAccessToken(
                    user: It.IsAny<ApplicationUser>(),
                    roles: It.IsAny<IReadOnlyList<string>>()),
                    Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserPasswordIsNotValid()
        {
            // Arrange
            string email = "IMvalidEmail@gmail.com";
            string userPassword = "ImValidPassword1@";

            var loginRequest = new LoginRequest(
                Email: email,
                Password: userPassword);

            var command = new LoginCommand(loginRequest);
            var cancelationToken = CancellationToken.None;

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "ValidUsername",
                Email = email,
                FirstName = "IM Valid",
                LastName = "Validian",
                PhoneNumber = "09900000000",
            };

            _mockUserQueryService
                .Setup(x => x.GetUserByEmailAsync(email, cancelationToken))
                .ReturnsAsync(user);

            _mockUserSignInManagerService
                .Setup(x => x.ValidateUserPassword(user, userPassword))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, cancelationToken);

            // Arrange
            result.IsSuccess.Should().BeFalse();

            result.Error.Should().NotBeNull();
            result.Error.Value.ErrorType.Should().Be(ErrorType.UNAUTHORIZED_ERROR);

            _mockUserQueryService
                .Verify(x => x.GetUserByEmailAsync(
                    email: email,
                    cancellationToken: cancelationToken),
                    Times.Once);

            _mockUserSignInManagerService
               .Verify(x => x.ValidateUserPassword(
                   user: user,
                   password: userPassword),
                   Times.Once);

            _mockUserRoleQueryService
                .Verify(x => x.GetUserRolesAsync(
                    userId: It.IsAny<string>()),
                    Times.Never);

            _mockJsonWebTokenService
                .Verify(x => x.GenerateAccessToken(
                    user: It.IsAny<ApplicationUser>(),
                    roles: It.IsAny<IReadOnlyList<string>>()),
                    Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserHasNotAnyRole()
        {
            // Arrange
            string email = "IMvalidEmail@gmail.com";
            string userPassword = "ImValidPassword1@";

            var loginRequest = new LoginRequest(
                Email: email,
                Password: userPassword);

            var command = new LoginCommand(loginRequest);
            var cancelationToken = CancellationToken.None;

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "ValidUsername",
                Email = email,
                FirstName = "IM Valid",
                LastName = "Validian",
                PhoneNumber = "09900000000",
            };

            var roles = Array.Empty<string>();

            _mockUserQueryService
                .Setup(x => x.GetUserByEmailAsync(email, cancelationToken))
                .ReturnsAsync(user);

            _mockUserSignInManagerService
                .Setup(x => x.ValidateUserPassword(user, userPassword))
                .ReturnsAsync(true);

            _mockUserRoleQueryService
                .Setup(x => x.GetUserRolesAsync(user.Id))
                .ReturnsAsync(roles);

            // Act
            var result = await _handler.Handle(command, cancelationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();

            result.Error.Should().NotBeNull();
            result.Error.Value.ErrorType.Should().Be(ErrorType.FORBIDDEN_ERROR);

            _mockUserQueryService
                .Verify(x => x.GetUserByEmailAsync(
                    email: email,
                    cancellationToken: cancelationToken),
                    Times.Once);

            _mockUserSignInManagerService
               .Verify(x => x.ValidateUserPassword(
                   user: user,
                   password: userPassword),
                   Times.Once);

            _mockUserRoleQueryService
                .Verify(x => x.GetUserRolesAsync(
                    userId: user.Id),
                    Times.Once);

            _mockJsonWebTokenService
                .Verify(x => x.GenerateAccessToken(
                    user: It.IsAny<ApplicationUser>(),
                    roles: It.IsAny<IReadOnlyList<string>>()),
                    Times.Never);
        }
    }
}
