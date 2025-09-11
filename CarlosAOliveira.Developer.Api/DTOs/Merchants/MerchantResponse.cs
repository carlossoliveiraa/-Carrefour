namespace CarlosAOliveira.Developer.Api.DTOs.Merchants
{
    /// <summary>
    /// Response DTO for merchant
    /// </summary>
    public class MerchantResponse
    {
        /// <summary>
        /// Merchant ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Merchant name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Merchant email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
