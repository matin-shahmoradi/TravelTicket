using AuthService.Auth.GetUsersByEmail;
using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Model.DTOs.UserDtos;
using AutoMapper;
using FluentAssertions;
using Moq;

namespace AuthSerive.UnitTests.Features.Users.GetUserByEmailTests
{
    public class GetUserByEmailQueryHandlerTest
    {
        private readonly Mock<IUserManagerQueryService> _userManagerQueryServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetUsersByEmailQueryHandler _handler;
        public GetUserByEmailQueryHandlerTest()
        {
            _userManagerQueryServiceMock = new Mock<IUserManagerQueryService>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetUsersByEmailQueryHandler(
                _userManagerQueryServiceMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserExists()
        {
            // Arrange 
            var email = "test@gmail.com";
            var query = new GetUserByEmailQuery(email);
            var cancelationToken = CancellationToken.None;

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                UserName = "matiu",
                PhoneNumber = "09900000000",
                FirstName = "Matiu",
                LastName = "Maitui"
            };

            var dto = new UserResponseDto
            {
                Id = user.Id,
                Email = email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            _userManagerQueryServiceMock
                .Setup(x => x.GetUserByEmailAsync(email, cancelationToken))
                .ReturnsAsync(user);

            _mapperMock.
                Setup(x => x.Map<UserResponseDto>(user))
                .Returns(dto);

            // Act
            var result = await _handler.Handle(query, cancelationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(dto);
            result.Error.Should().BeNull();

            _userManagerQueryServiceMock.Verify(
                x => x.GetUserByEmailAsync(email, cancelationToken),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<UserResponseDto>(user),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldFailure_WhenUserDoesNotExist()
        {
            // Arrange
            var email = "notfound@example.com";
            var query = new GetUserByEmailQuery(email);
            var cancellationToken = CancellationToken.None;

            _userManagerQueryServiceMock
                .Setup(x => x.GetUserByEmailAsync(email, cancellationToken))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Value.Message.Should().Be($"User with email {email} is not exist");

            _userManagerQueryServiceMock.Verify(
                x => x.GetUserByEmailAsync(email, cancellationToken),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<UserResponseDto>(It.IsAny<ApplicationUser>()),
                Times.Never);
        }
    }
}
