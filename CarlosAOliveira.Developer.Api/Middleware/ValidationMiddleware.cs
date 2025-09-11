using FluentValidation;
using System.Text.Json;

namespace CarlosAOliveira.Developer.Api.Middleware
{
    /// <summary>
    /// Middleware for handling validation errors globally
    /// </summary>
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationMiddleware> _logger;

        public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation error occurred: {Message}", ex.Message);

                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = "Validation failed",
                    errors = ex.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        message = e.ErrorMessage,
                        attemptedValue = e.AttemptedValue
                    }).ToList()
                };

                var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
