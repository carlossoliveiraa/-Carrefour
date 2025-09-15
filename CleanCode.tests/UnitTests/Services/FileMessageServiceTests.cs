using CleanCode.Common.Messaging;
using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Common.Messaging.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Bogus;
using FluentAssertions;

namespace CleanCode.Tests.UnitTests.Services
{
    /// <summary>
    /// Testes unitários para FileMessageService
    /// </summary>
    public class FileMessageServiceTests : IDisposable
    {
        private readonly Mock<ILogger<FileMessageService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly FileMessageService _service;
        private readonly string _testBasePath;
        private readonly Faker<UserCreatedMessage> _userCreatedMessageFaker;
        private readonly Faker<UserAccessedMessage> _userAccessedMessageFaker;

        public FileMessageServiceTests()
        {
            _mockLogger = new Mock<ILogger<FileMessageService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Configuração para usar pasta temporária de teste
            _testBasePath = Path.Combine(Path.GetTempPath(), "messaging_tests", Guid.NewGuid().ToString("N")[..8]);
            _mockConfiguration.Setup(x => x["Messaging:BasePath"]).Returns(_testBasePath);
            
            _service = new FileMessageService(_mockLogger.Object, _mockConfiguration.Object);

            _userCreatedMessageFaker = new Faker<UserCreatedMessage>()
                .RuleFor(u => u.UserId, f => f.Random.Guid())
                .RuleFor(u => u.Username, f => f.Person.UserName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Role, f => f.PickRandom("Admin", "Manager", "Customer"))
                .RuleFor(u => u.Status, f => f.PickRandom("Active", "Inactive", "Suspended"))
                .RuleFor(u => u.CreatedAt, f => f.Date.Recent())
                .RuleFor(u => u.SourceIp, f => f.Internet.Ip())
                .RuleFor(u => u.UserAgent, f => f.Internet.UserAgent());

            _userAccessedMessageFaker = new Faker<UserAccessedMessage>()
                .RuleFor(u => u.UserId, f => f.Random.Guid())
                .RuleFor(u => u.Username, f => f.Person.UserName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.AccessType, f => f.PickRandom("GET", "PUT", "DELETE", "PATCH"))
                .RuleFor(u => u.AccessedAt, f => f.Date.Recent())
                .RuleFor(u => u.SourceIp, f => f.Internet.Ip())
                .RuleFor(u => u.UserAgent, f => f.Internet.UserAgent())
                .RuleFor(u => u.RequestedBy, f => f.Random.Guid());
        }

        [Fact]
        public async Task SendMessageAsync_WithStringMessage_ShouldCreateFile()
        {
            // Arrange
            var message = "Test message content";
            var queueName = "test_queue";

            // Act
            await _service.SendMessageAsync(message, queueName);

            // Assert
            var queueDirectory = Path.Combine(_testBasePath, queueName);
            Directory.Exists(queueDirectory).Should().BeTrue();
            
            var files = Directory.GetFiles(queueDirectory, "*.txt");
            files.Should().HaveCount(1);
            
            var fileContent = await File.ReadAllTextAsync(files[0]);
            fileContent.Should().Contain(message);
            fileContent.Should().Contain(queueName);
        }

        [Fact]
        public async Task SendMessageAsync_WithUserCreatedMessage_ShouldCreateFileWithJson()
        {
            // Arrange
            var testMessage = _userCreatedMessageFaker.Generate();
            var queueName = "user_created";

            // Act
            await _service.SendMessageAsync(testMessage, queueName);

            // Assert
            var queueDirectory = Path.Combine(_testBasePath, queueName);
            Directory.Exists(queueDirectory).Should().BeTrue();
            
            var files = Directory.GetFiles(queueDirectory, "*.txt");
            files.Should().HaveCount(1);
            
            var fileContent = await File.ReadAllTextAsync(files[0]);
            fileContent.Should().Contain(testMessage.Username);
            fileContent.Should().Contain(testMessage.Email);
            fileContent.Should().Contain("UserCreatedMessage");
        }

        [Fact]
        public async Task SendMessageAsync_WithUserAccessedMessage_ShouldCreateFileWithJson()
        {
            // Arrange
            var testMessage = _userAccessedMessageFaker.Generate();
            var queueName = "user_accessed";

            // Act
            await _service.SendMessageAsync(testMessage, queueName);

            // Assert
            var queueDirectory = Path.Combine(_testBasePath, queueName);
            Directory.Exists(queueDirectory).Should().BeTrue();
            
            var files = Directory.GetFiles(queueDirectory, "*.txt");
            files.Should().HaveCount(1);
            
            var fileContent = await File.ReadAllTextAsync(files[0]);
            fileContent.Should().Contain(testMessage.Username);
            fileContent.Should().Contain(testMessage.Email);
            fileContent.Should().Contain("UserAccessedMessage");
        }

        [Fact]
        public async Task ReadMessagesAsync_WithExistingFiles_ShouldReturnAllMessages()
        {
            // Arrange
            var queueName = "test_queue";
            var queueDirectory = Path.Combine(_testBasePath, queueName);
            Directory.CreateDirectory(queueDirectory);

            var message1 = "First message";
            var message2 = "Second message";
            
            await _service.SendMessageAsync(message1, queueName);
            await _service.SendMessageAsync(message2, queueName);

            // Act
            var messages = await _service.ReadMessagesAsync(queueName);

            // Assert
            messages.Should().HaveCount(2);
            messages.Should().Contain(m => m.Contains(message1));
            messages.Should().Contain(m => m.Contains(message2));
        }

        [Fact]
        public async Task ReadMessagesAsync_WithNonExistentQueue_ShouldReturnEmpty()
        {
            // Arrange
            var queueName = "non_existent_queue";

            // Act
            var messages = await _service.ReadMessagesAsync(queueName);

            // Assert
            messages.Should().BeEmpty();
        }

        [Fact]
        public async Task SendMessageAsync_ShouldLogInformation()
        {
            // Arrange
            var message = "Test message";
            var queueName = "test_queue";

            // Act
            await _service.SendMessageAsync(message, queueName);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Message sent to queue")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SendMessageAsync_WithMultipleMessages_ShouldCreateMultipleFiles()
        {
            // Arrange
            var queueName = "test_queue";
            var messages = new[] { "Message 1", "Message 2", "Message 3" };

            // Act
            foreach (var message in messages)
            {
                await _service.SendMessageAsync(message, queueName);
            }

            // Assert
            var queueDirectory = Path.Combine(_testBasePath, queueName);
            var files = Directory.GetFiles(queueDirectory, "*.txt");
            files.Should().HaveCount(3);
        }

        [Fact]
        public async Task SendMessageAsync_ShouldCreateUniqueFileNames()
        {
            // Arrange
            var queueName = "test_queue";
            var message = "Test message";

            // Act
            await _service.SendMessageAsync(message, queueName);
            await _service.SendMessageAsync(message, queueName);

            // Assert
            var queueDirectory = Path.Combine(_testBasePath, queueName);
            var files = Directory.GetFiles(queueDirectory, "*.txt");
            files.Should().HaveCount(2);
            
            // Verifica se os nomes dos arquivos são únicos
            var fileNames = files.Select(Path.GetFileName).ToList();
            fileNames.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public async Task SendMessageAsync_ShouldIncludeTimestampInFileName()
        {
            // Arrange
            var queueName = "test_queue";
            var message = "Test message";
            var beforeSend = DateTime.UtcNow;

            // Act
            await _service.SendMessageAsync(message, queueName);

            // Assert
            var queueDirectory = Path.Combine(_testBasePath, queueName);
            var files = Directory.GetFiles(queueDirectory, "*.txt");
            files.Should().HaveCount(1);
            
            var fileName = Path.GetFileName(files[0]);
            fileName.Should().Contain(beforeSend.ToString("yyyyMMdd"));
        }

        public void Dispose()
        {
            // Cleanup - remove test directory
            if (Directory.Exists(_testBasePath))
            {
                Directory.Delete(_testBasePath, true);
            }
        }
    }
}
