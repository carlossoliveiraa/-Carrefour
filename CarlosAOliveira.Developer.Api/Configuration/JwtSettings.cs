namespace CarlosAOliveira.Developer.Api.Configuration
{
    /// <summary>
    /// JWT configuration settings
    /// </summary>
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        
        /// <summary>
        /// Secret key for JWT token signing
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;
        
        /// <summary>
        /// JWT token issuer
        /// </summary>
        public string Issuer { get; set; } = string.Empty;
        
        /// <summary>
        /// JWT token audience
        /// </summary>
        public string Audience { get; set; } = string.Empty;
        
        /// <summary>
        /// Token expiration time in minutes
        /// </summary>
        public int ExpirationInMinutes { get; set; }
        
        /// <summary>
        /// Refresh token expiration time in days
        /// </summary>
        public int RefreshTokenExpirationInDays { get; set; }
    }
}
