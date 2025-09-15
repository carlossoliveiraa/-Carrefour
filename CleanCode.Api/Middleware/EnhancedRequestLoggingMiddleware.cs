using System.Diagnostics;
using System.Text;

namespace CleanCode.Api.Middleware
{
    /// <summary>
    /// Middleware aprimorado para logging de requisições HTTP
    /// </summary>
    public class EnhancedRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<EnhancedRequestLoggingMiddleware> _logger;

        public EnhancedRequestLoggingMiddleware(RequestDelegate next, ILogger<EnhancedRequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            // Adiciona o request ID ao contexto
            context.Items["RequestId"] = requestId;

            // Captura informações da requisição
            var requestInfo = await CaptureRequestInfo(context, requestId);
            
            _logger.LogInformation("Request started: {RequestInfo}", requestInfo);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                var errorInfo = new
                {
                    RequestId = requestId,
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    QueryString = context.Request.QueryString.ToString(),
                    StatusCode = context.Response.StatusCode,
                    Duration = stopwatch.ElapsedMilliseconds,
                    Error = ex.Message,
                    StackTrace = ex.StackTrace
                };

                _logger.LogError(ex, "Request failed: {ErrorInfo}", errorInfo);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                
                var responseInfo = new
                {
                    RequestId = requestId,
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    QueryString = context.Request.QueryString.ToString(),
                    StatusCode = context.Response.StatusCode,
                    Duration = stopwatch.ElapsedMilliseconds,
                    ContentLength = context.Response.ContentLength,
                    UserAgent = context.Request.Headers.UserAgent.ToString(),
                    RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString()
                };

                var logLevel = GetLogLevel(context.Response.StatusCode);
                _logger.Log(logLevel, "Request completed: {ResponseInfo}", responseInfo);
            }
        }

        private async Task<object> CaptureRequestInfo(HttpContext context, string requestId)
        {
            var request = context.Request;
            
            // Captura o body da requisição se for POST/PUT/PATCH
            string? requestBody = null;
            if (request.Method is "POST" or "PUT" or "PATCH")
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            return new
            {
                RequestId = requestId,
                Method = request.Method,
                Path = request.Path,
                QueryString = request.QueryString.ToString(),
                ContentType = request.ContentType,
                ContentLength = request.ContentLength,
                UserAgent = request.Headers.UserAgent.ToString(),
                RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
                RequestBody = requestBody
            };
        }

        private static LogLevel GetLogLevel(int statusCode)
        {
            return statusCode switch
            {
                >= 500 => LogLevel.Error,
                >= 400 => LogLevel.Warning,
                _ => LogLevel.Information
            };
        }
    }
}
