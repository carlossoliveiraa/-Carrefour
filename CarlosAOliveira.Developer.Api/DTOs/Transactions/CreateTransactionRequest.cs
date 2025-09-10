using CarlosAOliveira.Developer.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarlosAOliveira.Developer.Api.DTOs.Transactions
{
    /// <summary>
    /// Create transaction request DTO
    /// </summary>
    public class CreateTransactionRequest
    {
        /// <summary>
        /// Merchant ID
        /// </summary>
        [Required(ErrorMessage = "Merchant ID is required")]
        public Guid MerchantId { get; set; }

        /// <summary>
        /// Transaction amount
        /// </summary>
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        [Required(ErrorMessage = "Transaction type is required")]
        public TransactionType Type { get; set; }

        /// <summary>
        /// Transaction description
        /// </summary>
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;
    }
}
