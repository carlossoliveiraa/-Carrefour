using System.Collections.Concurrent;

namespace CarlosAOliveira.Developer.Api.Middleware
{
    /// <summary>
    /// Middleware for rate limiting requests
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimitStore = new();
        private readonly int _maxRequests;
        private readonly TimeSpan _windowSize;

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _maxRequests = configuration.GetValue<int>("RateLimit:MaxRequests", 100);
            _windowSize = TimeSpan.FromMinutes(configuration.GetValue<int>("RateLimit:WindowMinutes", 1));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = GetClientId(context);
            var now = DateTime.UtcNow;

            var rateLimitInfo = _rateLimitStore.AddOrUpdate(
                clientId,
                new RateLimitInfo { Count = 1, WindowStart = now },
                (key, existing) =>
                {
                    if (now - existing.WindowStart > _windowSize)
                    {
                        // Reset window
                        return new RateLimitInfo { Count = 1, WindowStart = now };
                    }
                    else
                    {
                        // Increment count
                        return new RateLimitInfo { Count = existing.Count + 1, WindowStart = existing.WindowStart };
                    }
                });

            // Add rate limit headers
            context.Response.Headers.Add("X-RateLimit-Limit", _maxRequests.ToString());
            context.Response.Headers.Add("X-RateLimit-Remaining", Math.Max(0, _maxRequests - rateLimitInfo.Count).ToString());
            context.Response.Headers.Add("X-RateLimit-Reset", (rateLimitInfo.WindowStart.Add(_windowSize) - DateTime.UnixEpoch).TotalSeconds.ToString());

            if (rateLimitInfo.Count > _maxRequests)
            {
                _logger.LogWarning("Rate limit exceeded for client: {ClientId}. Count: {Count}, Limit: {Limit}", 
                    clientId, rateLimitInfo.Count, _maxRequests);

                context.Response.StatusCode = 429; // Too Many Requests
                context.Response.ContentType = "application/json";
                
                var response = new
                {
                    error = "Rate limit exceeded",
                    message = $"Too many requests. Limit: {_maxRequests} requests per {_windowSize.TotalMinutes} minutes",
                    retryAfter = (int)_windowSize.TotalSeconds
                };

                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
                return;
            }

            await _next(context);
        }

        private static string GetClientId(HttpContext context)
        {
            // Try to get client IP from various headers
            var clientIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
                          context.Request.Headers["X-Real-IP"].FirstOrDefault() ??
                          context.Connection.RemoteIpAddress?.ToString() ??
                          "unknown";

            return clientIp;
        }

        private class RateLimitInfo
        {
            public int Count { get; set; }
            public DateTime WindowStart { get; set; }
        }
    }
}
