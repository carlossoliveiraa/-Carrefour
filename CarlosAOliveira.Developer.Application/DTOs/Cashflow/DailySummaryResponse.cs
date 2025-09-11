namespace CarlosAOliveira.Developer.Application.DTOs.Cashflow
{
    /// <summary>
    /// Response DTO for daily summary operations
    /// </summary>
    public class DailySummaryResponse
    {
        /// <summary>
        /// Summary ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Merchant ID
        /// </summary>
        public Guid MerchantId { get; set; }

        /// <summary>
        /// Summary date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Total credits amount
        /// </summary>
        public decimal TotalCredits { get; set; }

        /// <summary>
        /// Total debits amount
        /// </summary>
        public decimal TotalDebits { get; set; }

        /// <summary>
        /// Net amount (credits - debits)
        /// </summary>
        public decimal NetAmount { get; set; }

        /// <summary>
        /// Number of transactions
        /// </summary>
        public int TransactionCount { get; set; }

        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
