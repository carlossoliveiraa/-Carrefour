using CleanCode.Application.Auth.AuthenticateUser;
using CleanCode.Common.Security.Interfaces;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;
using CleanCode.Domain.Repositories.Interfaces;
using CleanCode.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Bogus;
using FluentAssertions;

namespace CleanCode.Tests.UnitTests.Handlers.Auth
{
    /// <summary>
    /// Testes unitários para AuthenticateUserHandler
    /// </summary>
    public class AuthenticateUserHandlerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
        private readonly AuthenticateUserHandler _handler;
        private readonly Faker<AuthenticateUserCommand> _commandFaker;
        private readonly Faker<User> _userFaker;

        public AuthenticateUserHandlerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();

            _handler = new AuthenticateUserHandler(
                _mockUserRepository.Object,
                _mockPasswordHasher.Object,
                _mockJwtTokenGenerator.Object);

            _commandFaker = new Faker<AuthenticateUserCommand>()
                .RuleFor(c => c.Email, f => f.Person.Email)
                .RuleFor(c => c.Password, f => f.Internet.Password());

            _userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.Username, f => f.Person.UserName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Password, f => f.Internet.Password())
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Role, f => f.PickRandom<UserRole>())
                .RuleFor(u => u.Status, UserStatus.Active) // Always active for valid tests
                .RuleFor(u => u.CreatedAt, f => f.Date.Recent());
        }

        [Fact]
        public async Task Handle_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            var token = "valid_jwt_token";

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(x => x.VerifyPassword(command.Password, user.Password))
                .Returns(true);
            _mockJwtTokenGenerator.Setup(x => x.GenerateToken(user))
                .Returns(token);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be(token);
            result.Email.Should().Be(user.Email);
            result.Name.Should().Be(user.Username);
            result.Role.Should().Be(user.Role.ToString());
        }

        [Fact]
        public async Task Handle_WithNonExistentUser_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var command = _commandFaker.Generate();

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("Invalid credentials");
        }

        [Fact]
        public async Task Handle_WithInvalidPassword_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(x => x.VerifyPassword(command.Password, user.Password))
                .Returns(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("Invalid credentials");
        }

        [Fact]
        public async Task Handle_WithInactiveUser_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            user.Status = UserStatus.Inactive; // Inactive user

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(x => x.VerifyPassword(command.Password, user.Password))
                .Returns(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("User is not active");
        }

        [Fact]
        public async Task Handle_WithSuspendedUser_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            user.Status = UserStatus.Suspended; // Suspended user

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(x => x.VerifyPassword(command.Password, user.Password))
                .Returns(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("User is not active");
        }

        [Fact]
        public async Task Handle_ShouldVerifyPassword()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            var token = "valid_jwt_token";

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(x => x.VerifyPassword(command.Password, user.Password))
                .Returns(true);
            _mockJwtTokenGenerator.Setup(x => x.GenerateToken(user))
                .Returns(token);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockPasswordHasher.Verify(x => x.VerifyPassword(command.Password, user.Password), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldGenerateToken()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            var token = "valid_jwt_token";

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(x => x.VerifyPassword(command.Password, user.Password))
                .Returns(true);
            _mockJwtTokenGenerator.Setup(x => x.GenerateToken(user))
                .Returns(token);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockJwtTokenGenerator.Verify(x => x.GenerateToken(user), Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidUser_ShouldReturnCompleteResult()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            var token = "valid_jwt_token";

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(x => x.VerifyPassword(command.Password, user.Password))
                .Returns(true);
            _mockJwtTokenGenerator.Setup(x => x.GenerateToken(user))
                .Returns(token);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be(token);
            result.Email.Should().Be(user.Email);
            result.Name.Should().Be(user.Username);
            result.Role.Should().Be(user.Role.ToString());
            result.Id.Should().Be(user.Id);
            result.Phone.Should().Be(user.Phone);
        }
    }
}

