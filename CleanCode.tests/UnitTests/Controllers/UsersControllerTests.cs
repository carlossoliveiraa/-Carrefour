using AutoMapper;
using CleanCode.Api.Common;
using CleanCode.Api.Features.Users;
using CleanCode.Api.Features.Users.CreateUser;
using CleanCode.Api.Features.Users.GetUser;
using CleanCode.Application.Users.CreateUser;
using CleanCode.Application.Users.GetUser;
using CleanCode.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Bogus;
using FluentAssertions;

namespace CleanCode.Tests.UnitTests.Controllers
{
    /// <summary>
    /// Testes unitários para UsersController
    /// </summary>
    public class UsersControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UsersController _controller;
        private readonly Faker<CreateUserRequest> _createRequestFaker;
        private readonly Faker<GetUserRequest> _getRequestFaker;

        public UsersControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockMapper = new Mock<IMapper>();

            _controller = new UsersController(_mockMediator.Object, _mockMapper.Object);

            _createRequestFaker = new Faker<CreateUserRequest>()
                .RuleFor(r => r.Username, f => f.Person.UserName)
                .RuleFor(r => r.Email, f => f.Person.Email)
                .RuleFor(r => r.Password, f => f.Internet.Password())
                .RuleFor(r => r.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(r => r.Role, f => f.PickRandom<UserRole>())
                .RuleFor(r => r.Status, f => f.PickRandom<UserStatus>());

            _getRequestFaker = new Faker<GetUserRequest>()
                .RuleFor(r => r.Id, f => f.Random.Guid());
        }

        [Fact]
        public async Task CreateUser_WithValidRequest_ShouldReturnCreatedResult()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                Phone = "+5511999999999",
                Role = UserRole.Customer,
                Status = UserStatus.Active
            };
            var command = new CreateUserCommand();
            var result = new CreateUserResult
            {
                Id = Guid.NewGuid()
            };
            var response = new CreateUserResponse
            {
                Id = result.Id,
                Name = request.Username,
                Email = request.Email
            };

            _mockMapper.Setup(x => x.Map<CreateUserCommand>(request)).Returns(command);
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(x => x.Map<CreateUserResponse>(result)).Returns(response);

            // Act
            var actionResult = await _controller.CreateUser(request, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(actionResult);
            var apiResponse = Assert.IsType<ApiResponseWithData<CreateUserResponse>>(createdResult.Value);
            
            apiResponse.Success.Should().BeTrue();
            apiResponse.Message.Should().Be("User created successfully");
            apiResponse.Data!.Id.Should().Be(response.Id);
        }

        [Fact]
        public async Task CreateUser_WithInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateUserRequest(); // Invalid request

            // Act
            var actionResult = await _controller.CreateUser(request, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task GetUser_WithValidId_ShouldReturnOkResult()
        {
            // Arrange
            var request = _getRequestFaker.Generate();
            var command = new GetUserCommand(request.Id);
            var result = new GetUserResult
            {
                Id = request.Id,
                Name = "testuser",
                Email = "test@example.com"
            };
            var response = new GetUserResponse
            {
                Id = result.Id,
                Name = result.Name,
                Email = result.Email
            };

            _mockMapper.Setup(x => x.Map<GetUserCommand>(request.Id)).Returns(command);
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(x => x.Map<GetUserResponse>(result)).Returns(response);

            // Act
            var actionResult = await _controller.GetUser(request.Id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var apiResponse = Assert.IsType<ApiResponseWithData<GetUserResponse>>(okResult.Value);
            
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data!.Id.Should().Be(response.Id);
        }

        [Fact]
        public async Task GetUser_WithInvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var actionResult = await _controller.GetUser(invalidId, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateUser_ShouldMapRequestToCommand()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                Phone = "+5511999999999",
                Role = UserRole.Customer,
                Status = UserStatus.Active
            };
            var command = new CreateUserCommand();
            var result = new CreateUserResult { Id = Guid.NewGuid() };
            var response = new CreateUserResponse { Id = result.Id };

            _mockMapper.Setup(x => x.Map<CreateUserCommand>(request)).Returns(command);
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(x => x.Map<CreateUserResponse>(result)).Returns(response);

            // Act
            await _controller.CreateUser(request, CancellationToken.None);

            // Assert
            _mockMapper.Verify(x => x.Map<CreateUserCommand>(request), Times.Once);
        }

        [Fact]
        public async Task GetUser_ShouldMapIdToCommand()
        {
            // Arrange
            var request = _getRequestFaker.Generate();
            var command = new GetUserCommand(request.Id);
            var result = new GetUserResult { Id = request.Id };
            var response = new GetUserResponse { Id = result.Id };

            _mockMapper.Setup(x => x.Map<GetUserCommand>(request.Id)).Returns(command);
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(x => x.Map<GetUserResponse>(result)).Returns(response);

            // Act
            await _controller.GetUser(request.Id, CancellationToken.None);

            // Assert
            _mockMapper.Verify(x => x.Map<GetUserCommand>(request.Id), Times.Once);
        }

        [Fact]
        public async Task CreateUser_ShouldCallMediator()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                Phone = "+5511999999999",
                Role = UserRole.Customer,
                Status = UserStatus.Active
            };
            var command = new CreateUserCommand();
            var result = new CreateUserResult { Id = Guid.NewGuid() };
            var response = new CreateUserResponse { Id = result.Id };

            _mockMapper.Setup(x => x.Map<CreateUserCommand>(request)).Returns(command);
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(x => x.Map<CreateUserResponse>(result)).Returns(response);

            // Act
            await _controller.CreateUser(request, CancellationToken.None);

            // Assert
            _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUser_ShouldCallMediator()
        {
            // Arrange
            var request = _getRequestFaker.Generate();
            var command = new GetUserCommand(request.Id);
            var result = new GetUserResult { Id = request.Id };
            var response = new GetUserResponse { Id = result.Id };

            _mockMapper.Setup(x => x.Map<GetUserCommand>(request.Id)).Returns(command);
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(x => x.Map<GetUserResponse>(result)).Returns(response);

            // Act
            await _controller.GetUser(request.Id, CancellationToken.None);

            // Assert
            _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
