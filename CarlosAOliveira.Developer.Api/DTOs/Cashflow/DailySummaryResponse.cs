namespace CarlosAOliveira.Developer.Api.DTOs.Cashflow
{
    /// <summary>
    /// Response DTO for daily summary
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
        /// Indicates if balance is positive
        /// </summary>
        public bool HasPositiveBalance { get; set; }

        /// <summary>
        /// Indicates if balance is negative
        /// </summary>
        public bool HasNegativeBalance { get; set; }

        /// <summary>
        /// Indicates if balance is zero
        /// </summary>
        public bool IsBalanced { get; set; }
    }
}
