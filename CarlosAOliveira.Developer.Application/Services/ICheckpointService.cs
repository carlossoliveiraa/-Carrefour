namespace CarlosAOliveira.Developer.Application.Services
{
    /// <summary>
    /// Service for managing checkpoint and processed events tracking
    /// </summary>
    public interface ICheckpointService
    {
        /// <summary>
        /// Gets the last processed offset
        /// </summary>
        Task<long> GetLastOffsetAsync();

        /// <summary>
        /// Updates the last processed offset
        /// </summary>
        Task UpdateOffsetAsync(long offset);

        /// <summary>
        /// Checks if an event has already been processed
        /// </summary>
        Task<bool> IsEventProcessedAsync(string eventId);

        /// <summary>
        /// Marks an event as processed
        /// </summary>
        Task MarkEventAsProcessedAsync(string eventId);

        /// <summary>
        /// Performs a self-test to verify the service is working
        /// </summary>
        Task<bool> SelfTestAsync();
    }
}
