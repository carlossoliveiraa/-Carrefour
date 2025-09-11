using CarlosAOliveira.Developer.Domain.Common;

namespace CarlosAOliveira.Developer.Domain.Events
{
    /// <summary>
    /// Interface for event queue operations
    /// </summary>
    public interface IEventQueue
    {
        /// <summary>
        /// Publishes an event to the queue
        /// </summary>
        Task PublishAsync<T>(T domainEvent) where T : IDomainEvent;

        /// <summary>
        /// Reads events from the queue starting from a specific offset
        /// </summary>
        Task<IEnumerable<QueueEvent>> ReadEventsAsync(long offset = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current file size (for checkpoint tracking)
        /// </summary>
        Task<long> GetFileSizeAsync();
    }

    /// <summary>
    /// Represents an event in the queue
    /// </summary>
    public class QueueEvent
    {
        public string EventId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public object Payload { get; set; } = new();
        public DateTime Timestamp { get; set; }
        public long Offset { get; set; }
    }
}
