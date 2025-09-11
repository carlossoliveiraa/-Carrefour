using System.ComponentModel.DataAnnotations;

namespace CarlosAOliveira.Developer.Api.DTOs.Cashflow
{
    /// <summary>
    /// Request DTO for creating a new cashflow transaction
    /// </summary>
    public class CreateCashflowTransactionRequest
    {
        /// <summary>
        /// Transaction date
        /// </summary>
        [Required(ErrorMessage = "Date is required")]
        public DateOnly Date { get; set; }

        /// <summary>
        /// Transaction amount (must be positive)
        /// </summary>
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Transaction type (Credit or Debit)
        /// </summary>
        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Transaction category
        /// </summary>
        [Required(ErrorMessage = "Category is required")]
        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Transaction description
        /// </summary>
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;
    }
}
