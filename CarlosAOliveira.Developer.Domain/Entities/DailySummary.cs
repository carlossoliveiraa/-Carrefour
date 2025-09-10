using CarlosAOliveira.Developer.Domain.Common;

namespace CarlosAOliveira.Developer.Domain.Entities
{
    /// <summary>
    /// Represents a daily summary of transactions for a merchant
    /// </summary>
    public class DailySummary : BaseEntity
    {
        public Guid MerchantId { get; private set; }
        public DateTime Date { get; private set; }
        public decimal TotalCredits { get; private set; }
        public decimal TotalDebits { get; private set; }
        public decimal NetAmount { get; private set; }
        public int TransactionCount { get; private set; }

        private DailySummary() { } // EF Core constructor

        public DailySummary(Guid merchantId, DateTime date)
        {
            Id = Guid.NewGuid();
            MerchantId = merchantId;
            Date = date.Date; // Ensure only date part
            TotalCredits = 0;
            TotalDebits = 0;
            NetAmount = 0;
            TransactionCount = 0;
        }

        public DailySummary(Guid merchantId, DateTime date, decimal netAmount, int transactionCount)
        {
            Id = Guid.NewGuid();
            MerchantId = merchantId;
            Date = date.Date; // Ensure only date part
            NetAmount = netAmount;
            TransactionCount = transactionCount;
            
            // Calculate credits and debits based on net amount
            if (netAmount >= 0)
            {
                TotalCredits = netAmount;
                TotalDebits = 0;
            }
            else
            {
                TotalCredits = 0;
                TotalDebits = Math.Abs(netAmount);
            }
        }

        /// <summary>
        /// Adds a transaction to the daily summary
        /// </summary>
        public void AddTransaction(Transaction transaction)
        {
            if (transaction.MerchantId != MerchantId)
                throw new InvalidOperationException("Transaction merchant ID does not match summary merchant ID");

            if (transaction.CreatedAt.Date != Date)
                throw new InvalidOperationException("Transaction date does not match summary date");

            if (transaction.IsCredit)
            {
                TotalCredits += transaction.Amount;
            }
            else
            {
                TotalDebits += transaction.Amount;
            }

            TransactionCount++;
            NetAmount = TotalCredits - TotalDebits;
        }

        /// <summary>
        /// Checks if the summary has a positive balance
        /// </summary>
        public bool HasPositiveBalance => NetAmount > 0;

        /// <summary>
        /// Checks if the summary has a negative balance
        /// </summary>
        public bool HasNegativeBalance => NetAmount < 0;

        /// <summary>
        /// Checks if the summary is balanced (zero net amount)
        /// </summary>
        public bool IsBalanced => NetAmount == 0;
    }
}