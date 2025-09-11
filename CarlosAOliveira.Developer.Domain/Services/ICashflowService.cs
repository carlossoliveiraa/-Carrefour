using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Domain.Services
{
    /// <summary>
    /// Service for cashflow business logic
    /// </summary>
    public interface ICashflowService
    {
        /// <summary>
        /// Calculates daily balance for a merchant
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="date">Date to calculate balance for</param>
        /// <param name="transactions">Transactions for the date</param>
        /// <returns>Daily balance</returns>
        DailySummary CalculateDailyBalance(Guid merchantId, DateTime date, IEnumerable<Transaction> transactions);

        /// <summary>
        /// Validates if a transaction can be created
        /// </summary>
        /// <param name="transaction">Transaction to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        bool CanCreateTransaction(Transaction transaction);

        /// <summary>
        /// Calculates period summary for a merchant
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="dailySummaries">Daily summaries for the period</param>
        /// <returns>Period summary</returns>
        PeriodSummary CalculatePeriodSummary(Guid merchantId, DateTime startDate, DateTime endDate, IEnumerable<DailySummary> dailySummaries);
    }

    /// <summary>
    /// Period summary result
    /// </summary>
    public record PeriodSummary
    {
        public Guid MerchantId { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public decimal TotalCredits { get; init; }
        public decimal TotalDebits { get; init; }
        public decimal NetAmount { get; init; }
        public int TotalTransactionCount { get; init; }
        public int DaysInPeriod { get; init; }
        public decimal AverageDailyNetAmount { get; init; }
    }
}
