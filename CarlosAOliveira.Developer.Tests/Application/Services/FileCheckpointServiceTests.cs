using CarlosAOliveira.Developer.Application.Services;
using Xunit;

namespace CarlosAOliveira.Developer.Tests.Application.Services
{
    public class FileCheckpointServiceTests : IDisposable
    {
        private readonly string _testRuntimeDirectory;
        private readonly FileCheckpointService _checkpointService;

        public FileCheckpointServiceTests()
        {
            _testRuntimeDirectory = Path.Combine(Path.GetTempPath(), "cashflow_test", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testRuntimeDirectory);
            
            // Change to test directory
            var originalDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(_testRuntimeDirectory);
            
            _checkpointService = new FileCheckpointService();
        }

        [Fact]
        public async Task GetLastOffsetAsync_WithNoCheckpointFile_ShouldReturnZero()
        {
            // Act
            var offset = await _checkpointService.GetLastOffsetAsync();

            // Assert
            Assert.Equal(0, offset);
        }

        [Fact]
        public async Task UpdateOffsetAsync_AndGetLastOffsetAsync_ShouldWork()
        {
            // Arrange
            var testOffset = 12345L;

            // Act
            await _checkpointService.UpdateOffsetAsync(testOffset);
            var retrievedOffset = await _checkpointService.GetLastOffsetAsync();

            // Assert
            Assert.Equal(testOffset, retrievedOffset);
        }

        [Fact]
        public async Task IsEventProcessedAsync_WithNoProcessedFile_ShouldReturnFalse()
        {
            // Arrange
            var eventId = Guid.NewGuid().ToString();

            // Act
            var isProcessed = await _checkpointService.IsEventProcessedAsync(eventId);

            // Assert
            Assert.False(isProcessed);
        }

        [Fact]
        public async Task MarkEventAsProcessedAsync_AndIsEventProcessedAsync_ShouldWork()
        {
            // Arrange
            var eventId = Guid.NewGuid().ToString();

            // Act
            await _checkpointService.MarkEventAsProcessedAsync(eventId);
            var isProcessed = await _checkpointService.IsEventProcessedAsync(eventId);

            // Assert
            Assert.True(isProcessed);
        }

        [Fact]
        public async Task SelfTestAsync_ShouldReturnTrue()
        {
            // Act
            var result = await _checkpointService.SelfTestAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task MultipleEvents_ShouldBeTrackedCorrectly()
        {
            // Arrange
            var eventId1 = Guid.NewGuid().ToString();
            var eventId2 = Guid.NewGuid().ToString();
            var eventId3 = Guid.NewGuid().ToString();

            // Act
            await _checkpointService.MarkEventAsProcessedAsync(eventId1);
            await _checkpointService.MarkEventAsProcessedAsync(eventId2);

            var isProcessed1 = await _checkpointService.IsEventProcessedAsync(eventId1);
            var isProcessed2 = await _checkpointService.IsEventProcessedAsync(eventId2);
            var isProcessed3 = await _checkpointService.IsEventProcessedAsync(eventId3);

            // Assert
            Assert.True(isProcessed1);
            Assert.True(isProcessed2);
            Assert.False(isProcessed3);
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
