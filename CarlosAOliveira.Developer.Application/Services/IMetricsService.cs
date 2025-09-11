namespace CarlosAOliveira.Developer.Application.Services
{
    /// <summary>
    /// Service for application metrics and monitoring
    /// </summary>
    public interface IMetricsService
    {
        /// <summary>
        /// Increments a counter metric
        /// </summary>
        /// <param name="name">Metric name</param>
        /// <param name="tags">Optional tags</param>
        void IncrementCounter(string name, Dictionary<string, string>? tags = null);

        /// <summary>
        /// Records a timing metric
        /// </summary>
        /// <param name="name">Metric name</param>
        /// <param name="duration">Duration to record</param>
        /// <param name="tags">Optional tags</param>
        void RecordTiming(string name, TimeSpan duration, Dictionary<string, string>? tags = null);

        /// <summary>
        /// Records a gauge metric
        /// </summary>
        /// <param name="name">Metric name</param>
        /// <param name="value">Value to record</param>
        /// <param name="tags">Optional tags</param>
        void RecordGauge(string name, double value, Dictionary<string, string>? tags = null);

        /// <summary>
        /// Records a histogram metric
        /// </summary>
        /// <param name="name">Metric name</param>
        /// <param name="value">Value to record</param>
        /// <param name="tags">Optional tags</param>
        void RecordHistogram(string name, double value, Dictionary<string, string>? tags = null);

        /// <summary>
        /// Creates a timer for measuring operation duration
        /// </summary>
        /// <param name="name">Metric name</param>
        /// <param name="tags">Optional tags</param>
        /// <returns>Timer instance</returns>
        IDisposable StartTimer(string name, Dictionary<string, string>? tags = null);
    }

    /// <summary>
    /// Timer for measuring operation duration
    /// </summary>
    public class Timer : IDisposable
    {
        private readonly IMetricsService _metricsService;
        private readonly string _name;
        private readonly Dictionary<string, string>? _tags;
        private readonly DateTime _startTime;
        private bool _disposed;

        public Timer(IMetricsService metricsService, string name, Dictionary<string, string>? tags = null)
        {
            _metricsService = metricsService;
            _name = name;
            _tags = tags;
            _startTime = DateTime.UtcNow;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                var duration = DateTime.UtcNow - _startTime;
                _metricsService.RecordTiming(_name, duration, _tags);
                _disposed = true;
            }
        }
    }
}
