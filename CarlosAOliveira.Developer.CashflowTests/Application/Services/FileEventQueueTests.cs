using CarlosAOliveira.Developer.Application.Services;
using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Domain.Events;
using System.Text.Json;
using Xunit;

namespace CarlosAOliveira.Developer.Tests.Application.Services
{
    public class FileEventQueueTests : IDisposable
    {
        private readonly string _testRuntimeDirectory;
        private readonly FileEventQueue _eventQueue;

        public FileEventQueueTests()
        {
            _testRuntimeDirectory = Path.Combine(Path.GetTempPath(), "cashflow_test", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testRuntimeDirectory);
            
            // Change to test directory
            var originalDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(_testRuntimeDirectory);
            
            _eventQueue = new FileEventQueue();
        }

        [Fact]
        public async Task PublishAsync_WithValidEvent_ShouldWriteToFile()
        {
            // Arrange
            var transaction = new Transaction(
                DateOnly.FromDateTime(DateTime.Today),
                100.50m,
                TransactionType.Credit,
                "Sales",
                "Test transaction"
            );
            var domainEvent = new TransactionCreatedEvent(transaction);

            // Act
            await _eventQueue.PublishAsync(domainEvent);

            // Assert
            var queueFilePath = Path.Combine(_testRuntimeDirectory, "runtime", "queue.ndjson");
            Assert.True(File.Exists(queueFilePath));
            
            var content = await File.ReadAllTextAsync(queueFilePath);
            Assert.NotEmpty(content);
            Assert.Contains("TransactionCreated", content);
        }

        [Fact]
        public async Task ReadEventsAsync_WithEmptyFile_ShouldReturnEmptyList()
        {
            // Act
            var events = await _eventQueue.ReadEventsAsync();

            // Assert
            Assert.Empty(events);
        }

        [Fact]
        public async Task ReadEventsAsync_WithValidEvents_ShouldReturnEvents()
        {
            // Arrange
            var transaction = new Transaction(
                DateOnly.FromDateTime(DateTime.Today),
                100.50m,
                TransactionType.Credit,
                "Sales",
                "Test transaction"
            );
            var domainEvent = new TransactionCreatedEvent(transaction);

            await _eventQueue.PublishAsync(domainEvent);

            // Act
            var events = await _eventQueue.ReadEventsAsync();

            // Assert
            Assert.Single(events);
            var queueEvent = events.First();
            Assert.Equal("TransactionCreatedEvent", queueEvent.Type);
            Assert.NotNull(queueEvent.Payload);
        }

        [Fact]
        public async Task GetFileSizeAsync_WithEmptyFile_ShouldReturnZero()
        {
            // Act
            var size = await _eventQueue.GetFileSizeAsync();

            // Assert
            Assert.Equal(0, size);
        }

        [Fact]
        public async Task GetFileSizeAsync_WithContent_ShouldReturnFileSize()
        {
            // Arrange
            var transaction = new Transaction(
                DateOnly.FromDateTime(DateTime.Today),
                100.50m,
                TransactionType.Credit,
                "Sales",
                "Test transaction"
            );
            var domainEvent = new TransactionCreatedEvent(transaction);

            await _eventQueue.PublishAsync(domainEvent);

            // Act
            var size = await _eventQueue.GetFileSizeAsync();

            // Assert
            Assert.True(size > 0);
        }

        public void Dispose()
        {
            // Clean up test directory
            if (Directory.Exists(_testRuntimeDirectory))
            {
                Directory.Delete(_testRuntimeDirectory, true);
            }
        }
    }
}
