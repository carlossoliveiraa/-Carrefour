namespace CarlosAOliveira.Developer.Api.DTOs.Cashflow
{
    /// <summary>
    /// Response DTO for period summary
    /// </summary>
    public class PeriodSummaryResponse
    {
        /// <summary>
        /// Merchant ID
        /// </summary>
        public Guid MerchantId { get; set; }

        /// <summary>
        /// Start date of the period
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the period
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Total credits amount for the period
        /// </summary>
        public decimal TotalCredits { get; set; }

        /// <summary>
        /// Total debits amount for the period
        /// </summary>
        public decimal TotalDebits { get; set; }

        /// <summary>
        /// Net amount for the period (credits - debits)
        /// </summary>
        public decimal NetAmount { get; set; }

        /// <summary>
        /// Total number of transactions in the period
        /// </summary>
        public int TotalTransactionCount { get; set; }

        /// <summary>
        /// Number of days in the period
        /// </summary>
        public int DaysInPeriod { get; set; }

        /// <summary>
        /// Average daily net amount
        /// </summary>
        public decimal AverageDailyNetAmount { get; set; }

        /// <summary>
        /// Indicates if period has positive balance
        /// </summary>
        public bool HasPositiveBalance { get; set; }

        /// <summary>
        /// Indicates if period has negative balance
        /// </summary>
        public bool HasNegativeBalance { get; set; }

        /// <summary>
        /// Indicates if period is balanced
        /// </summary>
        public bool IsBalanced { get; set; }
    }
}
