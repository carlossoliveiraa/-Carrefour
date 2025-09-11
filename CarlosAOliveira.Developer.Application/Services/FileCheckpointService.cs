namespace CarlosAOliveira.Developer.Application.Services
{
    /// <summary>
    /// File-based checkpoint service implementation
    /// </summary>
    public class FileCheckpointService : ICheckpointService
    {
        private readonly string _runtimeDirectory;
        private readonly string _checkpointFilePath;
        private readonly string _processedIdsFilePath;

        public FileCheckpointService()
        {
            _runtimeDirectory = Path.Combine(Directory.GetCurrentDirectory(), "runtime");
            _checkpointFilePath = Path.Combine(_runtimeDirectory, "checkpoint.txt");
            _processedIdsFilePath = Path.Combine(_runtimeDirectory, "processed.ids");
            
            // Ensure runtime directory exists
            Directory.CreateDirectory(_runtimeDirectory);
        }

        public async Task<long> GetLastOffsetAsync()
        {
            if (!File.Exists(_checkpointFilePath))
                return 0;

            try
            {
                var content = await File.ReadAllTextAsync(_checkpointFilePath);
                return long.TryParse(content.Trim(), out var offset) ? offset : 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task UpdateOffsetAsync(long offset)
        {
            await RetryAsync(async () =>
            {
                await File.WriteAllTextAsync(_checkpointFilePath, offset.ToString());
            }, 3);
        }

        public async Task<bool> IsEventProcessedAsync(string eventId)
        {
            if (!File.Exists(_processedIdsFilePath))
                return false;

            try
            {
                var content = await File.ReadAllTextAsync(_processedIdsFilePath);
                return content.Contains(eventId);
            }
            catch
            {
                return false;
            }
        }

        public async Task MarkEventAsProcessedAsync(string eventId)
        {
            await RetryAsync(async () =>
            {
                await File.AppendAllTextAsync(_processedIdsFilePath, eventId + Environment.NewLine);
            }, 3);
        }

        public async Task<bool> SelfTestAsync()
        {
            try
            {
                // Test checkpoint operations
                var testOffset = 12345L;
                await UpdateOffsetAsync(testOffset);
                var retrievedOffset = await GetLastOffsetAsync();
                
                if (retrievedOffset != testOffset)
                    return false;

                // Test processed events operations
                var testEventId = Guid.NewGuid().ToString();
                var wasProcessed = await IsEventProcessedAsync(testEventId);
                
                if (wasProcessed)
                    return false;

                await MarkEventAsProcessedAsync(testEventId);
                var isNowProcessed = await IsEventProcessedAsync(testEventId);
                
                return isNowProcessed;
            }
            catch
            {
                return false;
            }
        }

        private static async Task RetryAsync(Func<Task> operation, int maxRetries)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await operation();
                    return;
                }
                catch (Exception) when (i < maxRetries - 1)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100 * Math.Pow(2, i))); // Exponential backoff
                }
            }
        }
    }
}
