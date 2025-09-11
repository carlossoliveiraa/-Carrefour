namespace CarlosAOliveira.Developer.Application.DTOs.Cashflow
{
    /// <summary>
    /// Response DTO for transaction operations
    /// </summary>
    public class TransactionResponse
    {
        /// <summary>
        /// Transaction ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Transaction date
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Transaction amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Transaction category
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Transaction description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
