using System.ComponentModel.DataAnnotations;

namespace CarlosAOliveira.Developer.Api.DTOs.Auth
{
    /// <summary>
    /// Request DTO for generating test tokens
    /// </summary>
    public class TestTokenRequest
    {
        /// <summary>
        /// Merchant ID (optional, will generate if not provided)
        /// </summary>
        public Guid? MerchantId { get; set; }

        /// <summary>
        /// Merchant name
        /// </summary>
        [Required(ErrorMessage = "Merchant name is required")]
        [StringLength(100, ErrorMessage = "Merchant name cannot exceed 100 characters")]
        public string MerchantName { get; set; } = string.Empty;
    }
}
