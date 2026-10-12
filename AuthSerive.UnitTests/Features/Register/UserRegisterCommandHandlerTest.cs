using AuthService.Auth.Register;
using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Model.DTOs.RegisterDtos;
using BuildingBlocks;
using BuildingBlocks.Abstractions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace AuthService.UnitTests.Features.Register
{
    public class UserRegisterCommandHandlerTest
    {
        private readonly Mock<IUserManagerQueryService> _mockUserQueryService;
        private readonly Mock<IUserManagerCommandService> _mockUserCommandService;
        private readonly Mock<IUserRegistrationEmailSender> _mockEmailSender;
        private readonly Mock<ILogger<UserRegisterCommandHandler>> _mockLogger;
        private readonly Mock<ITransactionExecutor> _mockTransactionExecutor;
        private readonly UserRegisterCommandHandler _handler;
        private readonly CancellationToken _cancellationToken;
        public UserRegisterCommandHandlerTest()
        {
            _mockUserQueryService = new Mock<IUserManagerQueryService>();
            _mockUserCommandService = new Mock<IUserManagerCommandService>();
            _mockEmailSender = new Mock<IUserRegistrationEmailSender>();
            _mockLogger = new Mock<ILogger<UserRegisterCommandHandler>>();
            _mockTransactionExecutor = new Mock<ITransactionExecutor>();
            _cancellationToken = CancellationToken.None;
            _handler = new UserRegisterCommandHandler(
                userManagerQueryService: _mockUserQueryService.Object,
                userManagerCommandService: _mockUserCommandService.Object,
                transactionExecutor: _mockTransactionExecutor.Object,
                emailSender: _mockEmailSender.Object,
                logger: _mockLogger.Object);

        }

        [Fact]
        public async Task Handle_WhenValidRequest_ShouldCreateUserAndReturnSuccess()
        {
            var registerRequestDto = new RegisterRequestDto(
                Email: "ValidEmail@gmail.com",
                Password: "ValidPassword1234@",
                Firstname: "FirstName",
                Lastname: "Lastname",
                PhoneNumber: "09878578951"
                );


            var command = new UserRegisterCommand(registerRequestDto);

            var role = Roles.User;

            _mockUserQueryService
                .Setup(x => x.GetUserByEmailAsync(registerRequestDto.Email, _cancellationToken))
                .ReturnsAsync((ApplicationUser?)null);

            _mockTransactionExecutor
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<Func<CancellationToken, Task<Result<RegisterResponseDto>>>>(),
                    _cancellationToken))
                    .Returns<Func<CancellationToken, Task<Result<RegisterResponseDto>>>, CancellationToken>(
                        (operation, ct) => operation(ct));

            _mockUserCommandService
                .Setup(x => x.CreateUserAsync(
                    It.Is<ApplicationUser>(u => IsExpectedUser(u, registerRequestDto)), registerRequestDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserCommandService
                .Setup(x => x.AddUserToRoleAsync(
                    It.Is<ApplicationUser>(u => IsExpectedUser(u, registerRequestDto)), role))
                .ReturnsAsync(IdentityResult.Success);

            _mockEmailSender
                .Setup(x => x.SendEmailRegistration(It.IsAny<ApplicationUser>(), _cancellationToken))
                .ReturnsAsync(Result<string>.Success(""));

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeTrue();

            result.Value.Should().NotBeNull();
            result.Value.Email.Should().Be(registerRequestDto.Email);
            result.Value.FullName.Should().Be($"{registerRequestDto.Firstname} {registerRequestDto.Lastname}");

            result.Error.Should().BeNull();

            _mockUserQueryService
                .Verify(x => x.GetUserByEmailAsync(registerRequestDto.Email, _cancellationToken),
               times: Times.Once);

            _mockTransactionExecutor
                .Verify(x => x.ExecuteAsync(
                    It.IsAny<Func<CancellationToken, Task<Result<RegisterResponseDto>>>>(),
                    _cancellationToken),
                times: Times.Once);

            _mockUserCommandService
                .Verify(x => x.CreateUserAsync(
                    It.Is<ApplicationUser>(u => IsExpectedUser(u, registerRequestDto)), registerRequestDto.Password),
                 times: Times.Once);

            _mockUserCommandService
                .Verify(x => x.AddUserToRoleAsync(
                    It.Is<ApplicationUser>(u => IsExpectedUser(u, registerRequestDto)), role),
                times: Times.Once);

            _mockEmailSender
                .Verify(x => x.SendEmailRegistration(It.IsAny<ApplicationUser>(), _cancellationToken),
                times: Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenEmailIsAlreadyExist()
        {
            // Arrange
            var registerRequestDto = new RegisterRequestDto(
                Email: "ValidEmail@gmail.com",
                Password: "ValidPassword1234@",
                Firstname: "FirstName",
                Lastname: "Lastname",
                PhoneNumber: "09878578951"
                );

            var existedUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = registerRequestDto.Email,
                UserName = registerRequestDto.Email,
                FirstName = registerRequestDto.Firstname,
                LastName = registerRequestDto.Lastname,
                PhoneNumber = "0990000000"
            };
            var command = new UserRegisterCommand(registerRequestDto);

            _mockUserQueryService
                .Setup(x => x.GetUserByEmailAsync(
                    email: registerRequestDto.Email,
                    cancellationToken: _cancellationToken))
                .ReturnsAsync(existedUser);

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();

            result.Value.Should().BeNull();

            result.Error.Should().NotBeNull();
            result.Error.Value.ErrorType.Should().Be(ErrorType.CONFLICT_ERROR);

            _mockUserQueryService
                .Verify(x => x.GetUserByEmailAsync(registerRequestDto.Email, _cancellationToken),
               times: Times.Once);

            _mockUserCommandService
                .Verify(x => x.CreateUserAsync(
                     user: It.IsAny<ApplicationUser>(),
                     password: It.IsAny<string>()),
                times: Times.Never);

            _mockUserCommandService
                .Verify(x => x.AddUserToRoleAsync(
                     user: It.IsAny<ApplicationUser>(),
                     role: It.IsAny<string>()),
                times: Times.Never);

            _mockEmailSender
                .Verify(x => x.SendEmailRegistration(
                    user: It.IsAny<ApplicationUser>(),
                    cancellationToken: It.IsAny<CancellationToken>()),
                times: Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenCreateUserFail()
        {
            var registerRequestDto = new RegisterRequestDto(
                Email: "ValidEmail@gmail.com",
                Password: "ValidPassword1234@",
                Firstname: "FirstName",
                Lastname: "Lastname",
                PhoneNumber: "09878578951"
                );

            var command = new UserRegisterCommand(registerRequestDto);

            _mockUserQueryService
                .Setup(x => x.GetUserByEmailAsync(registerRequestDto.Email, _cancellationToken))
                .ReturnsAsync((ApplicationUser?)null);

            _mockTransactionExecutor
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<Func<CancellationToken, Task<Result<RegisterResponseDto>>>>(),
                    _cancellationToken))
                    .Returns<Func<CancellationToken, Task<Result<RegisterResponseDto>>>, CancellationToken>(
                        (operation, ct) => operation(ct));

            _mockUserCommandService
                .Setup(x => x.CreateUserAsync(
                    user: It.Is<ApplicationUser>(u => IsExpectedUser(u, registerRequestDto)),
                    password: registerRequestDto.Password))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert

            result.IsSuccess.Should().BeFalse();

            result.Value.Should().BeNull();

            result.Error.Should().NotBeNull();

            _mockUserQueryService
                .Verify(x => x.GetUserByEmailAsync(registerRequestDto.Email, _cancellationToken),
               times: Times.Once);

            _mockTransactionExecutor
                .Verify(x => x.ExecuteAsync(
                    It.IsAny<Func<CancellationToken, Task<Result<RegisterResponseDto>>>>(),
                    _cancellationToken),
                times: Times.Once);

            _mockUserCommandService
                .Verify(x => x.CreateUserAsync(
                     user: It.Is<ApplicationUser>(u => IsExpectedUser(u, registerRequestDto)),
                     password: It.IsAny<string>()),
                times: Times.Once);

            _mockUserCommandService
                .Verify(x => x.AddUserToRoleAsync(
                     user: It.IsAny<ApplicationUser>(),
                     role: It.IsAny<string>()),
                times: Times.Never);

            _mockEmailSender
                .Verify(x => x.SendEmailRegistration(
                    user: It.IsAny<ApplicationUser>(),
                    cancellationToken: It.IsAny<CancellationToken>()),
                times: Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenAddUserToRoleFail()
        {
            var registerRequestDto = new RegisterRequestDto(
                Email: "ValidEmail@gmail.com",
                Password: "ValidPassword1234@",
                Firstname: "FirstName",
                Lastname: "Lastname",
                PhoneNumber: "09878578951"
                );

            string role = Roles.User;

            var command = new UserRegisterCommand(registerRequestDto);

            _mockUserQueryService
                .Setup(x => x.GetUserByEmailAsync(registerRequestDto.Email, _cancellationToken))
                .ReturnsAsync((ApplicationUser?)null);

            _mockTransactionExecutor
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<Func<CancellationToken, Task<Result<RegisterResponseDto>>>>(),
                    _cancellationToken))
                    .Returns<Func<CancellationToken, Task<Result<RegisterResponseDto>>>, CancellationToken>(
                        (operation, ct) => operation(ct));

            _mockUserCommandService
                .Setup(x => x.CreateUserAsync(
                    user: It.Is<ApplicationUser>(u => IsExpectedUser(u, registerRequestDto)),
                    password: registerRequestDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserCommandService
                .Setup(x => x.AddUserToRoleAsync(
                    It.Is<ApplicationUser>(u => IsExpectedUser(u, registerRequestDto)), role))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            result.IsSuccess.Should().BeFalse();

            result.Value.Should().BeNull();

            result.Error.Should().NotBeNull();

            _mockUserQueryService
                .Verify(x => x.GetUserByEmailAsync(registerRequestDto.Email, _cancellationToken),
               times: Times.Once);

            _mockTransactionExecutor
                .Verify(x => x.ExecuteAsync(
                    It.IsAny<Func<CancellationToken, Task<Result<RegisterResponseDto>>>>(),
                    _cancellationToken),
                times: Times.Once);

            _mockUserCommandService
                .Verify(x => x.CreateUserAsync(
                     user: It.Is<ApplicationUser>(u => IsExpectedUser(u, registerRequestDto)),
                     password: It.IsAny<string>()),
                times: Times.Once);

            _mockUserCommandService
                .Verify(x => x.AddUserToRoleAsync(
                     user: It.Is<ApplicationUser>(u => IsExpectedUser(u, registerRequestDto)),
                     role: role),
                times: Times.Once);

            _mockEmailSender
                .Verify(x => x.SendEmailRegistration(
                    user: It.IsAny<ApplicationUser>(),
                    cancellationToken: It.IsAny<CancellationToken>()),
                times: Times.Never);

        }
        private static bool IsExpectedUser(ApplicationUser user, RegisterRequestDto dto)
        {
            return user.Email == dto.Email &&
                   user.UserName == dto.Email &&
                   user.FirstName == dto.Firstname &&
                   user.LastName == dto.Lastname;
        }
        private static ApplicationUser MapToApplicationUser(RegisterRequestDto dto)
        {
            return ApplicationUser.CreateUser(dto.Email, dto.Email, dto.Firstname, dto.Lastname, dto.PhoneNumber);
        }
    }
}
