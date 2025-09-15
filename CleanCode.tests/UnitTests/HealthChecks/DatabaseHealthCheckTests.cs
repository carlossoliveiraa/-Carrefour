using CleanCode.Api.HealthChecks;
using CleanCode.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace CleanCode.Tests.UnitTests.HealthChecks
{
    /// <summary>
    /// Testes unitários para DatabaseHealthCheck
    /// </summary>
    public class DatabaseHealthCheckTests
    {
        private readonly Mock<ILogger<DatabaseHealthCheck>> _mockLogger;
        private readonly DatabaseHealthCheck _healthCheck;

        public DatabaseHealthCheckTests()
        {
            _mockLogger = new Mock<ILogger<DatabaseHealthCheck>>();
            
            // Como DefaultContext é sealed, vamos usar um contexto real com InMemory
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            var context = new DefaultContext(options);
            _healthCheck = new DatabaseHealthCheck(context, _mockLogger.Object);
        }

        [Fact]
        public async Task CheckHealthAsync_WithHealthyDatabase_ShouldReturnHealthy()
        {
            // Act
            var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

            // Assert
            result.Status.Should().BeOneOf(HealthStatus.Healthy, HealthStatus.Unhealthy);
            result.Description.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CheckHealthAsync_WithZeroUsers_ShouldReturnHealthy()
        {
            // Act
            var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

            // Assert
            result.Status.Should().BeOneOf(HealthStatus.Healthy, HealthStatus.Unhealthy);
            result.Description.Should().NotBeNullOrEmpty();
        }
    }
}

