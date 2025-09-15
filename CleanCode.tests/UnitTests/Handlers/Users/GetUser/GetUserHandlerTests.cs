using AutoMapper;
using CleanCode.Application.Users.GetUser;
using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Common.Messaging.Models;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;
using CleanCode.Domain.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Bogus;
using FluentAssertions;

namespace CleanCode.Tests.UnitTests.Handlers.Users.GetUser
{
    /// <summary>
    /// Testes unitários para GetUserHandler
    /// </summary>
    public class GetUserHandlerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<ILogger<GetUserHandler>> _mockLogger;
        private readonly GetUserHandler _handler;
        private readonly Faker<GetUserCommand> _commandFaker;
        private readonly Faker<User> _userFaker;

        public GetUserHandlerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockMessageService = new Mock<IMessageService>();
            _mockLogger = new Mock<ILogger<GetUserHandler>>();

            _handler = new GetUserHandler(
                _mockUserRepository.Object,
                _mockMapper.Object,
                _mockMessageService.Object,
                _mockLogger.Object);

            _commandFaker = new Faker<GetUserCommand>()
                .CustomInstantiator(f => new GetUserCommand(f.Random.Guid()));

            _userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.Username, f => f.Person.UserName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Role, f => f.PickRandom<UserRole>())
                .RuleFor(u => u.Status, f => f.PickRandom<UserStatus>())
                .RuleFor(u => u.CreatedAt, f => f.Date.Recent());
        }

        [Fact]
        public async Task Handle_WithValidId_ShouldReturnUserAndSendMessage()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            var result = new GetUserResult
            {
                Id = user.Id,
                Name = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                Status = user.Status
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(x => x.Map<GetUserResult>(user)).Returns(result);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(user.Id);
            response.Name.Should().Be(user.Username);
            response.Email.Should().Be(user.Email);

            _mockUserRepository.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
            _mockMessageService.Verify(x => x.SendMessageAsync(It.IsAny<UserAccessedMessage>(), "user_accessed", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var command = _commandFaker.Generate();

            _mockUserRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain($"User with ID {command.Id} not found");
            _mockMessageService.Verify(x => x.SendMessageAsync(It.IsAny<UserAccessedMessage>(), "user_accessed", It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithInvalidCommand_ShouldThrowValidationException()
        {
            // Arrange
            var command = new GetUserCommand(Guid.Empty); // Invalid command

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
            var result = new GetUserResult
            {
                Id = user.Id,
                Name = user.Username,
                Email = user.Email
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(x => x.Map<GetUserResult>(user)).Returns(result);
            
            _mockMessageService.Setup(x => x.SendMessageAsync(It.IsAny<UserAccessedMessage>(), "user_accessed", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Messaging service failed"));

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            _mockUserRepository.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
            _mockMessageService.Verify(x => x.SendMessageAsync(It.IsAny<UserAccessedMessage>(), "user_accessed", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldMapUserToResult()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            var result = new GetUserResult { Id = user.Id };

            _mockUserRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(x => x.Map<GetUserResult>(user)).Returns(result);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockMapper.Verify(x => x.Map<GetUserResult>(user), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldSendCorrectMessageToQueue()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var user = _userFaker.Generate();
            var result = new GetUserResult { Id = user.Id };

            _mockUserRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(x => x.Map<GetUserResult>(user)).Returns(result);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockMessageService.Verify(x => x.SendMessageAsync(
                It.Is<UserAccessedMessage>(m => 
                    m.UserId == user.Id &&
                    m.Username == user.Username &&
                    m.Email == user.Email &&
                    m.AccessType == "GET"),
                "user_accessed",
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

