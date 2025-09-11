using CarlosAOliveira.Developer.Domain.Common;
using CarlosAOliveira.Developer.Domain.ValueObjects;

namespace CarlosAOliveira.Developer.Domain.Entities
{
    /// <summary>
    /// Represents a daily summary of transactions for a merchant
    /// </summary>
    public class DailySummary : BaseEntity
    {
        /// <summary>
        /// Merchant ID
        /// </summary>
        public Guid MerchantId { get; private set; }

        /// <summary>
        /// Summary date
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Total credits amount
        /// </summary>
        public Money TotalCredits { get; private set; }

        /// <summary>
        /// Total debits amount
        /// </summary>
        public Money TotalDebits { get; private set; }

        /// <summary>
        /// Net amount (credits - debits)
        /// </summary>
        public Money NetAmount { get; private set; }

        /// <summary>
        /// Number of transactions
        /// </summary>
        public int TransactionCount { get; private set; }

        private DailySummary() 
        { 
            TotalCredits = new Money(0);
            TotalDebits = new Money(0);
            NetAmount = new Money(0);
        } // EF Core constructor

        /// <summary>
        /// Initializes a new instance of DailySummary
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="date">Summary date</param>
        public DailySummary(Guid merchantId, DateTime date)
        {
            Id = Guid.NewGuid();
            MerchantId = merchantId;
            Date = date.Date; // Ensure only date part
            TotalCredits = new Money(0);
            TotalDebits = new Money(0);
            NetAmount = new Money(0);
            TransactionCount = 0;
        }

        /// <summary>
        /// Initializes a new instance of DailySummary with values
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="date">Summary date</param>
        /// <param name="netAmount">Net amount</param>
        /// <param name="transactionCount">Transaction count</param>
        public DailySummary(Guid merchantId, DateTime date, decimal netAmount, int transactionCount)
        {
            Id = Guid.NewGuid();
            MerchantId = merchantId;
            Date = date.Date; // Ensure only date part
            NetAmount = new Money(netAmount);
            TransactionCount = transactionCount;
            
            // Calculate credits and debits based on net amount
            if (netAmount >= 0)
            {
                TotalCredits = new Money(netAmount);
                TotalDebits = new Money(0);
            }
            else
            {
                TotalCredits = new Money(0);
                TotalDebits = new Money(Math.Abs(netAmount));
            }
        }

        /// <summary>
        /// Adds a transaction to the daily summary
        /// </summary>
        /// <param name="transaction">Transaction to add</param>
        /// <exception cref="InvalidOperationException">Thrown when transaction date doesn't match summary date</exception>
        public void AddTransaction(Transaction transaction)
        {
            if (transaction.Date.ToDateTime(TimeOnly.MinValue).Date != Date)
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
        public bool HasPositiveBalance => NetAmount.IsPositive;

        /// <summary>
        /// Checks if the summary has a negative balance
        /// </summary>
        public bool HasNegativeBalance => NetAmount.IsNegative;

        /// <summary>
        /// Checks if the summary is balanced (zero net amount)
        /// </summary>
        public bool IsBalanced => NetAmount.IsZero;

        /// <summary>
        /// Gets the net amount as decimal
        /// </summary>
        public decimal NetAmountDecimal => NetAmount.Amount;

        /// <summary>
        /// Gets the total credits as decimal
        /// </summary>
        public decimal TotalCreditsDecimal => TotalCredits.Amount;

        /// <summary>
        /// Gets the total debits as decimal
        /// </summary>
        public decimal TotalDebitsDecimal => TotalDebits.Amount;
    }
}