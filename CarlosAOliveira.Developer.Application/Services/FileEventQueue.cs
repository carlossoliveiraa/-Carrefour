using CarlosAOliveira.Developer.Domain.Common;
using CarlosAOliveira.Developer.Domain.Events;
using System.Text.Json;

namespace CarlosAOliveira.Developer.Application.Services
{
    /// <summary>
    /// File-based event queue implementation using NDJSON format
    /// </summary>
    public class FileEventQueue : IEventQueue
    {
        private readonly string _queueFilePath;
        private readonly string _runtimeDirectory;
        private readonly JsonSerializerOptions _jsonOptions;

        public FileEventQueue()
        {
            _runtimeDirectory = Path.Combine(Directory.GetCurrentDirectory(), "runtime");
            _queueFilePath = Path.Combine(_runtimeDirectory, "queue.ndjson");
            
            // Ensure runtime directory exists
            Directory.CreateDirectory(_runtimeDirectory);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task PublishAsync<T>(T domainEvent) where T : IDomainEvent
        {
            var queueEvent = new QueueEvent
            {
                EventId = Guid.NewGuid().ToString(),
                Type = typeof(T).Name,
                Payload = domainEvent,
                Timestamp = DateTime.UtcNow
            };

            var jsonLine = JsonSerializer.Serialize(queueEvent, _jsonOptions) + Environment.NewLine;

            // Retry logic for file operations
            await RetryAsync(async () =>
            {
                await File.AppendAllTextAsync(_queueFilePath, jsonLine, CancellationToken.None);
            }, 3);
        }

        public async Task<IEnumerable<QueueEvent>> ReadEventsAsync(long offset = 0, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_queueFilePath))
                return Enumerable.Empty<QueueEvent>();

            var events = new List<QueueEvent>();
            var currentOffset = 0L;

            using var fileStream = new FileStream(_queueFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(fileStream);

            // Skip to offset
            if (offset > 0)
            {
                fileStream.Seek(offset, SeekOrigin.Begin);
                currentOffset = offset;
            }

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                try
                {
                    var queueEvent = JsonSerializer.Deserialize<QueueEvent>(line, _jsonOptions);
                    if (queueEvent != null)
                    {
                        queueEvent.Offset = currentOffset;
                        events.Add(queueEvent);
                    }
                }
                catch (JsonException)
                {
                    // Skip malformed lines
                    continue;
                }

                currentOffset = fileStream.Position;
            }

            return events;
        }

        public Task<long> GetFileSizeAsync()
        {
            if (!File.Exists(_queueFilePath))
                return Task.FromResult(0L);

            var fileInfo = new FileInfo(_queueFilePath);
            return Task.FromResult(fileInfo.Length);
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
