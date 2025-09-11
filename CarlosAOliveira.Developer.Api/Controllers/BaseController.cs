using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarlosAOliveira.Developer.Api.Controllers
{
    /// <summary>
    /// Base controller with common functionality
    /// </summary>
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Gets the current user ID from claims
        /// </summary>
        /// <returns>User ID or null if not found</returns>
        protected Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("user_id");
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }

        /// <summary>
        /// Gets the current user email from claims
        /// </summary>
        /// <returns>User email or null if not found</returns>
        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst("email")?.Value;
        }

        /// <summary>
        /// Gets the correlation ID from headers
        /// </summary>
        /// <returns>Correlation ID or null if not found</returns>
        protected string? GetCorrelationId()
        {
            return Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? 
                   HttpContext.TraceIdentifier;
        }

        /// <summary>
        /// Creates a standardized error response
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errors">Additional errors</param>
        /// <returns>Error response</returns>
        protected IActionResult Error(string message, object? errors = null)
        {
            var response = new
            {
                error = message,
                errors = errors,
                correlationId = GetCorrelationId(),
                timestamp = DateTime.UtcNow
            };

            return BadRequest(response);
        }

        /// <summary>
        /// Creates a standardized success response
        /// </summary>
        /// <param name="data">Response data</param>
        /// <param name="message">Success message</param>
        /// <returns>Success response</returns>
        protected IActionResult Success<T>(T data, string? message = null)
        {
            var response = new
            {
                data = data,
                message = message,
                correlationId = GetCorrelationId(),
                timestamp = DateTime.UtcNow
            };

            return Ok(response);
        }

        /// <summary>
        /// Creates a standardized created response
        /// </summary>
        /// <param name="data">Created data</param>
        /// <param name="actionName">Action name for location header</param>
        /// <param name="routeValues">Route values for location header</param>
        /// <param name="message">Success message</param>
        /// <returns>Created response</returns>
        protected IActionResult Created<T>(T data, string actionName, object routeValues, string? message = null)
        {
            var response = new
            {
                data = data,
                message = message,
                correlationId = GetCorrelationId(),
                timestamp = DateTime.UtcNow
            };

            return CreatedAtAction(actionName, routeValues, response);
        }
    }
}
