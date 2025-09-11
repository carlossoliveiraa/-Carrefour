using System.ComponentModel.DataAnnotations;

namespace CarlosAOliveira.Developer.Api.DTOs.Merchants
{
    /// <summary>
    /// Request DTO for updating a merchant
    /// </summary>
    public class UpdateMerchantRequest
    {
        /// <summary>
        /// Merchant name
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
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