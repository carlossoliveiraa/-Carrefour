using CarlosAOliveira.Developer.Domain.Entities;
using System.Security.Claims;

namespace CarlosAOliveira.Developer.Api.Services
{
    /// <summary>
    /// JWT service interface for token generation and validation
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT access token for the user
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>JWT access token</returns>
        string GenerateAccessToken(User user);
        
        /// <summary>
        /// Generates a refresh token
        /// </summary>
        /// <returns>Refresh token</returns>
        string GenerateRefreshToken();
        
        /// <summary>
        /// Validates a JWT token and extracts claims
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Claims principal if valid, null otherwise</returns>
        ClaimsPrincipal? ValidateToken(string token);
        
        /// <summary>
        /// Extracts user ID from JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>User ID if valid, null otherwise</returns>
        Guid? GetUserIdFromToken(string token);
    }
}
