using System.Diagnostics;

namespace CarlosAOliveira.Developer.Api.Middleware
{
    /// <summary>
    /// Middleware for detailed request logging
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var correlationId = context.TraceIdentifier;
            
            // Add correlation ID to response headers
            context.Response.Headers.Add("X-Correlation-ID", correlationId);

            // Log request start
            _logger.LogInformation(
                "Request started: {Method} {Path} from {RemoteIp} - CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress,
                correlationId);

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                // Log request completion
                _logger.LogInformation(
                    "Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms - CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    correlationId);

                // Log slow requests
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    _logger.LogWarning(
                        "Slow request detected: {Method} {Path} - Duration: {Duration}ms - CorrelationId: {CorrelationId}",
                        context.Request.Method,
                        context.Request.Path,
                        stopwatch.ElapsedMilliseconds,
                        correlationId);
                }
            }
        }
    }
}
