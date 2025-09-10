using System.ComponentModel.DataAnnotations;

namespace CarlosAOliveira.Developer.Api.DTOs.Merchants
{
    /// <summary>
    /// Create merchant request DTO
    /// </summary>
    public class CreateMerchantRequest
    {
        /// <summary>
        /// Merchant name
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Merchant email
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; } = string.Empty;
    }
}
