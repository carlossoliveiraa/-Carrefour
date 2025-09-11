using Microsoft.Extensions.Logging;

namespace CarlosAOliveira.Developer.Application.Services
{
    /// <summary>
    /// Logging-based metrics service implementation
    /// </summary>
    public class LoggingMetricsService : IMetricsService
    {
        private readonly ILogger<LoggingMetricsService> _logger;

        public LoggingMetricsService(ILogger<LoggingMetricsService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Increments a counter metric
        /// </summary>
        /// <param name="name">Metric name</param>
        /// <param name="tags">Optional tags</param>
        public void IncrementCounter(string name, Dictionary<string, string>? tags = null)
        {
            var tagsString = tags != null ? string.Join(", ", tags.Select(kvp => $"{kvp.Key}={kvp.Value}")) : "";
            _logger.LogInformation("METRIC: Counter {Name} incremented. Tags: {Tags}", name, tagsString);
        }

        /// <summary>
        /// Records a timing metric
        /// </summary>
        /// <param name="name">Metric name</param>
        /// <param name="duration">Duration to record</param>
        /// <param name="tags">Optional tags</param>
        public void RecordTiming(string name, TimeSpan duration, Dictionary<string, string>? tags = null)
        {
            var tagsString = tags != null ? string.Join(", ", tags.Select(kvp => $"{kvp.Key}={kvp.Value}")) : "";
            _logger.LogInformation("METRIC: Timing {Name} recorded: {Duration}ms. Tags: {Tags}", 
                name, duration.TotalMilliseconds, tagsString);
        }

        /// <summary>
        /// Records a gauge metric
        /// </summary>
        /// <param name="name">Metric name</param>
        /// <param name="value">Value to record</param>
        /// <param name="tags">Optional tags</param>
        public void RecordGauge(string name, double value, Dictionary<string, string>? tags = null)
        {
            var tagsString = tags != null ? string.Join(", ", tags.Select(kvp => $"{kvp.Key}={kvp.Value}")) : "";
            _logger.LogInformation("METRIC: Gauge {Name} recorded: {Value}. Tags: {Tags}", name, value, tagsString);
        }

        /// <summary>
        /// Records a histogram metric
        /// </summary>
        /// <param name="name">Metric name</param>
        /// <param name="value">Value to record</param>
        /// <param name="tags">Optional tags</param>
        public void RecordHistogram(string name, double value, Dictionary<string, string>? tags = null)
        {
            var tagsString = tags != null ? string.Join(", ", tags.Select(kvp => $"{kvp.Key}={kvp.Value}")) : "";
            _logger.LogInformation("METRIC: Histogram {Name} recorded: {Value}. Tags: {Tags}", name, value, tagsString);
        }

        /// <summary>
        /// Creates a timer for measuring operation duration
        /// </summary>
        /// <param name="name">Metric name</param>
        /// <param name="tags">Optional tags</param>
        /// <returns>Timer instance</returns>
        public IDisposable StartTimer(string name, Dictionary<string, string>? tags = null)
        {
            return new Timer(this, name, tags);
        }
    }
}
