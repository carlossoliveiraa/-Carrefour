namespace CarlosAOliveira.Developer.Api.DTOs.Auth
{
    /// <summary>
    /// Response DTO for successful login
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// JWT access token
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration time in seconds
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Token type (Bearer)
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Merchant ID
        /// </summary>
        public Guid MerchantId { get; set; }

        /// <summary>
        /// Merchant name
        /// </summary>
        public string MerchantName { get; set; } = string.Empty;
    }
}