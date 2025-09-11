using System.Security.Claims;

namespace CarlosAOliveira.Developer.Api.Services
{
    /// <summary>
    /// JWT service interface for token generation and validation
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT access token for a merchant
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="merchantName">Merchant name</param>
        /// <returns>JWT access token</returns>
        string GenerateAccessToken(Guid merchantId, string merchantName);
        
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
        /// Extracts merchant ID from JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Merchant ID if valid, null otherwise</returns>
        Guid? GetMerchantIdFromToken(string token);
    }
}
