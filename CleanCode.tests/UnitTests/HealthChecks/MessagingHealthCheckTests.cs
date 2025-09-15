using CleanCode.Api.HealthChecks;
using CleanCode.Common.Messaging.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace CleanCode.Tests.UnitTests.HealthChecks
{
    /// <summary>
    /// Testes unitários para MessagingHealthCheck
    /// </summary>
    public class MessagingHealthCheckTests
    {
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<ILogger<MessagingHealthCheck>> _mockLogger;
        private readonly MessagingHealthCheck _healthCheck;

        public MessagingHealthCheckTests()
        {
            _mockMessageService = new Mock<IMessageService>();
            _mockLogger = new Mock<ILogger<MessagingHealthCheck>>();
            _healthCheck = new MessagingHealthCheck(_mockMessageService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CheckHealthAsync_WithHealthyMessaging_ShouldReturnHealthy()
        {
            // Arrange
            var testMessages = new[] { "Test message 1", "Test message 2" };
            
            _mockMessageService.Setup(x => x.SendMessageAsync(
                It.IsAny<string>(), 
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            _mockMessageService.Setup(x => x.ReadMessagesAsync(
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(testMessages);

            // Act
            var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

            // Assert
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Description.Should().Be("Messaging service is healthy");
            result.Data.Should().ContainKey("test_queue");
            result.Data.Should().ContainKey("messages_found");
            result.Data.Should().ContainKey("timestamp");
            result.Data["test_queue"].Should().Be("health_check_test");
            result.Data["messages_found"].Should().Be(2);
        }

        [Fact]
        public async Task CheckHealthAsync_WithEmptyMessages_ShouldReturnHealthy()
        {
            // Arrange
            _mockMessageService.Setup(x => x.SendMessageAsync(
                It.IsAny<string>(), 
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            _mockMessageService.Setup(x => x.ReadMessagesAsync(
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<string>());

            // Act
            var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

            // Assert
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Description.Should().Be("Messaging service is healthy");
            result.Data["messages_found"].Should().Be(0);
        }

        [Fact]
        public async Task CheckHealthAsync_WithSendMessageException_ShouldReturnUnhealthy()
        {
            // Arrange
            _mockMessageService.Setup(x => x.SendMessageAsync(
                It.IsAny<string>(), 
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Send message failed"));

            // Act
            var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

            // Assert
            result.Status.Should().Be(HealthStatus.Unhealthy);
            result.Description.Should().Be("Messaging service health check failed");
            result.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task CheckHealthAsync_WithReadMessagesException_ShouldReturnUnhealthy()
        {
            // Arrange
            _mockMessageService.Setup(x => x.SendMessageAsync(
                It.IsAny<string>(), 
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            _mockMessageService.Setup(x => x.ReadMessagesAsync(
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Read messages failed"));

            // Act
            var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

            // Assert
            result.Status.Should().Be(HealthStatus.Unhealthy);
            result.Description.Should().Be("Messaging service health check failed");
            result.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSendTestMessage()
        {
            // Arrange
            var testMessages = new[] { "Test message" };
            
            _mockMessageService.Setup(x => x.SendMessageAsync(
                It.IsAny<string>(), 
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            _mockMessageService.Setup(x => x.ReadMessagesAsync(
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(testMessages);

            // Act
            await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

            // Assert
            _mockMessageService.Verify(x => x.SendMessageAsync(
                It.Is<string>(s => s.Contains("Health check test")),
                "health_check_test",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldReadMessagesFromTestQueue()
        {
            // Arrange
            var testMessages = new[] { "Test message" };
            
            _mockMessageService.Setup(x => x.SendMessageAsync(
                It.IsAny<string>(), 
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            _mockMessageService.Setup(x => x.ReadMessagesAsync(
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(testMessages);

            // Act
            await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

            // Assert
            _mockMessageService.Verify(x => x.ReadMessagesAsync(
                "health_check_test",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldLogDebugMessages()
        {
            // Arrange
            var testMessages = new[] { "Test message" };
            
            _mockMessageService.Setup(x => x.SendMessageAsync(
                It.IsAny<string>(), 
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            _mockMessageService.Setup(x => x.ReadMessagesAsync(
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(testMessages);

            // Act
            await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Checking messaging service health")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Messaging service health check passed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task CheckHealthAsync_WithException_ShouldLogError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.SendMessageAsync(
                It.IsAny<string>(), 
                "health_check_test", 
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Messaging service health check failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}

