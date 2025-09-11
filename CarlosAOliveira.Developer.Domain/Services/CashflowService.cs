using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.ValueObjects;

namespace CarlosAOliveira.Developer.Domain.Services
{
    /// <summary>
    /// Service for cashflow business logic
    /// </summary>
    public class CashflowService : ICashflowService
    {
        /// <summary>
        /// Calculates daily balance for a merchant
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="date">Date to calculate balance for</param>
        /// <param name="transactions">Transactions for the date</param>
        /// <returns>Daily balance</returns>
        public DailySummary CalculateDailyBalance(Guid merchantId, DateTime date, IEnumerable<Transaction> transactions)
        {
            var transactionList = transactions.ToList();
            var summary = new DailySummary(merchantId, date);

            foreach (var transaction in transactionList)
            {
                summary.AddTransaction(transaction);
            }

            return summary;
        }

        /// <summary>
        /// Validates if a transaction can be created
        /// </summary>
        /// <param name="transaction">Transaction to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool CanCreateTransaction(Transaction transaction)
        {
            // Business rules for transaction creation
            if (transaction.Amount.IsZero)
                return false;

            if (transaction.Amount.Amount > 1000000) // Max transaction amount
                return false;

            if (transaction.Date > DateOnly.FromDateTime(DateTime.Today))
                return false;

            if (transaction.Date < DateOnly.FromDateTime(DateTime.Today.AddYears(-1)))
                return false;

            if (string.IsNullOrWhiteSpace(transaction.Category))
                return false;

            if (string.IsNullOrWhiteSpace(transaction.Description))
                return false;

            return true;
        }

        /// <summary>
        /// Calculates period summary for a merchant
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="dailySummaries">Daily summaries for the period</param>
        /// <returns>Period summary</returns>
        public PeriodSummary CalculatePeriodSummary(Guid merchantId, DateTime startDate, DateTime endDate, IEnumerable<DailySummary> dailySummaries)
        {
            var summaries = dailySummaries.ToList();
            var totalCredits = summaries.Sum(s => s.TotalCreditsDecimal);
            var totalDebits = summaries.Sum(s => s.TotalDebitsDecimal);
            var totalTransactionCount = summaries.Sum(s => s.TransactionCount);
            var daysInPeriod = (endDate.Date - startDate.Date).Days + 1;
            var netAmount = totalCredits - totalDebits;
            var averageDailyNetAmount = daysInPeriod > 0 ? netAmount / daysInPeriod : 0;

            return new PeriodSummary
            {
                MerchantId = merchantId,
                StartDate = startDate,
                EndDate = endDate,
                TotalCredits = totalCredits,
                TotalDebits = totalDebits,
                NetAmount = netAmount,
                TotalTransactionCount = totalTransactionCount,
                DaysInPeriod = daysInPeriod,
                AverageDailyNetAmount = averageDailyNetAmount
            };
        }
    }
}
