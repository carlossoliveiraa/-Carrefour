using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;
using System.Text;

namespace CleanCode.Api.Middleware
{
    /// <summary>
    /// Middleware para logging detalhado de requisições HTTP
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
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            // Adicionar request ID ao contexto
            context.Items["RequestId"] = requestId;
            
            // Capturar informações da requisição
            var requestInfo = new
            {
                RequestId = requestId,
                Method = context.Request.Method,
                Path = context.Request.Path.Value,
                QueryString = context.Request.QueryString.Value,
                UserAgent = context.Request.Headers.UserAgent.ToString(),
                RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
                ContentType = context.Request.ContentType,
                ContentLength = context.Request.ContentLength
            };

            // Log da requisição recebida
            _logger.LogInformation("🌐 Request started: {RequestId} {Method} {Path}{QueryString} from {RemoteIpAddress} | UserAgent: {UserAgent}",
                requestId, requestInfo.Method, requestInfo.Path, requestInfo.QueryString, requestInfo.RemoteIpAddress, requestInfo.UserAgent);

            // Capturar o corpo da requisição se for POST/PUT/PATCH
            string requestBody = string.Empty;
            if (ShouldLogRequestBody(context.Request.Method))
            {
                requestBody = await ReadRequestBodyAsync(context.Request);
            }

            // Interceptar a resposta
            var originalResponseStream = context.Response.Body;
            using var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                _logger.LogError(ex, "Request failed: {RequestId} {Method} {Path} - Error: {ErrorMessage} - Duration: {Duration}ms",
                    requestId, requestInfo.Method, requestInfo.Path, ex.Message, stopwatch.ElapsedMilliseconds);
                
                throw;
            }

            stopwatch.Stop();

            // Capturar informações da resposta
            var responseInfo = new
            {
                StatusCode = context.Response.StatusCode,
                ContentType = context.Response.ContentType,
                ContentLength = context.Response.ContentLength
            };

            // Capturar o corpo da resposta
            string responseBody = string.Empty;
            if (ShouldLogResponseBody(context.Response.StatusCode, context.Response.ContentType))
            {
                responseBody = await ReadResponseBodyAsync(responseStream);
            }

            // Log da resposta
            var logLevel = GetLogLevel(context.Response.StatusCode);
            var statusEmoji = GetStatusEmoji(context.Response.StatusCode);
            _logger.Log(logLevel, "{StatusEmoji} Request completed: {RequestId} {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms",
                statusEmoji, requestId, requestInfo.Method, requestInfo.Path, responseInfo.StatusCode, stopwatch.ElapsedMilliseconds);

            // Log detalhado para requisições com erro ou duração longa
            if (context.Response.StatusCode >= 400 || stopwatch.ElapsedMilliseconds > 1000)
            {
                _logger.LogWarning("Request details: {RequestId} - Request: {RequestBody} - Response: {ResponseBody}",
                    requestId, requestBody, responseBody);
            }

            // Copiar a resposta de volta para o stream original
            responseStream.Position = 0;
            await responseStream.CopyToAsync(originalResponseStream);
            context.Response.Body = originalResponseStream;
        }

        private static bool ShouldLogRequestBody(string method)
        {
            return method is "POST" or "PUT" or "PATCH";
        }

        private static bool ShouldLogResponseBody(int statusCode, string? contentType)
        {
            return statusCode >= 400 && 
                   contentType != null && 
                   (contentType.Contains("application/json") || contentType.Contains("text/"));
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

        private static string GetStatusEmoji(int statusCode)
        {
            return statusCode switch
            {
                >= 200 and < 300 => "✅",
                >= 300 and < 400 => "🔄",
                >= 400 and < 500 => "⚠️",
                >= 500 => "❌",
                _ => "❓"
            };
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            request.Body.Position = 0;
            
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            
            return body.Length > 1000 ? body[..1000] + "..." : body;
        }

        private static async Task<string> ReadResponseBodyAsync(Stream responseStream)
        {
            responseStream.Position = 0;
            using var reader = new StreamReader(responseStream, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            responseStream.Position = 0;
            
            return body.Length > 1000 ? body[..1000] + "..." : body;
        }
    }
}
