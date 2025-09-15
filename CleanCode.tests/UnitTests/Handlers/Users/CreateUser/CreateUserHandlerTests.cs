using AutoMapper;
using CleanCode.Application.Users.CreateUser;
using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Common.Messaging.Models;
using CleanCode.Common.Security.Interfaces;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;
using CleanCode.Domain.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Bogus;
using FluentAssertions;

namespace CleanCode.Tests.UnitTests.Handlers.Users.CreateUser
{
    /// <summary>
    /// Testes unitários para CreateUserHandler
    /// </summary>
    public class CreateUserHandlerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<ILogger<CreateUserHandler>> _mockLogger;
        private readonly CreateUserHandler _handler;
        private readonly Faker<CreateUserCommand> _commandFaker;
        private readonly Faker<User> _userFaker;

        public CreateUserHandlerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockMessageService = new Mock<IMessageService>();
            _mockLogger = new Mock<ILogger<CreateUserHandler>>();

            _handler = new CreateUserHandler(
                _mockUserRepository.Object,
                _mockMapper.Object,
                _mockPasswordHasher.Object,
                _mockMessageService.Object,
                _mockLogger.Object);

            _commandFaker = new Faker<CreateUserCommand>()
                .RuleFor(c => c.Username, f => f.Person.UserName)
                .RuleFor(c => c.Email, f => f.Person.Email)
                .RuleFor(c => c.Password, f => f.Internet.Password(8) + "1A!")
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber("+5511999999999"))
                .RuleFor(c => c.Role, f => f.PickRandom(new[] { UserRole.Admin, UserRole.Manager, UserRole.Customer }))
                .RuleFor(c => c.Status, f => f.PickRandom(new[] { UserStatus.Active, UserStatus.Inactive, UserStatus.Suspended }));

            _userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.Username, f => f.Person.UserName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Password, f => f.Internet.Password())
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Role, f => f.PickRandom<UserRole>())
                .RuleFor(u => u.Status, f => f.PickRandom<UserStatus>())
                .RuleFor(u => u.CreatedAt, f => f.Date.Recent());
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateUserAndSendMessage()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            var result = new CreateUserResult
            {
                Id = user.Id
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            
            _mockMapper.Setup(x => x.Map<User>(command)).Returns(user);
            _mockPasswordHasher.Setup(x => x.HashPassword(command.Password)).Returns("hashed_password");
            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(x => x.Map<CreateUserResult>(user)).Returns(result);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(user.Id);

            _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockMessageService.Verify(x => x.SendMessageAsync(It.IsAny<UserCreatedMessage>(), "user_created", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithExistingEmail_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var existingUser = _userFaker.Generate();

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain($"User with email {command.Email} already exists");
            _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithInvalidCommand_ShouldThrowValidationException()
        {
            // Arrange
            var command = new CreateUserCommand(); // Invalid command with empty properties

            // Act & Assert
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => 
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenMessagingFails_ShouldNotFailOperation()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            var result = new CreateUserResult
            {
                Id = user.Id
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            
            _mockMapper.Setup(x => x.Map<User>(command)).Returns(user);
            _mockPasswordHasher.Setup(x => x.HashPassword(command.Password)).Returns("hashed_password");
            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(x => x.Map<CreateUserResult>(user)).Returns(result);
            
            _mockMessageService.Setup(x => x.SendMessageAsync(It.IsAny<UserCreatedMessage>(), "user_created", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Messaging service failed"));

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockMessageService.Verify(x => x.SendMessageAsync(It.IsAny<UserCreatedMessage>(), "user_created", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldHashPassword()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            var result = new CreateUserResult { Id = user.Id };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            
            _mockMapper.Setup(x => x.Map<User>(command)).Returns(user);
            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(x => x.Map<CreateUserResult>(user)).Returns(result);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockPasswordHasher.Verify(x => x.HashPassword(command.Password), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldSendCorrectMessageToQueue()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            var result = new CreateUserResult { Id = user.Id };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            
            _mockMapper.Setup(x => x.Map<User>(command)).Returns(user);
            _mockPasswordHasher.Setup(x => x.HashPassword(command.Password)).Returns("hashed_password");
            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(x => x.Map<CreateUserResult>(user)).Returns(result);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockMessageService.Verify(x => x.SendMessageAsync(
                It.Is<UserCreatedMessage>(m => 
                    m.UserId == user.Id &&
                    m.Username == user.Username &&
                    m.Email == user.Email &&
                    m.Role == user.Role.ToString() &&
                    m.Status == user.Status.ToString()),
                "user_created",
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

