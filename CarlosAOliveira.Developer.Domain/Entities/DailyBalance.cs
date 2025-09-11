using CarlosAOliveira.Developer.Domain.Common;

namespace CarlosAOliveira.Developer.Domain.Entities
{
    /// <summary>
    /// Represents the consolidated daily balance in the cash flow system
    /// </summary>
    public class DailyBalance : BaseEntity
    {
        public DateOnly Date { get; private set; }
        public decimal Balance { get; private set; }
        public DateTime LastUpdated { get; private set; }

        private DailyBalance() { } // EF Core constructor

        public DailyBalance(DateOnly date, decimal balance)
        {
            Id = Guid.NewGuid();
            Date = date;
            Balance = balance;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the balance for this day
        /// </summary>
        /// <param name="newBalance">The new balance amount</param>
        public void UpdateBalance(decimal newBalance)
        {
            Balance = newBalance;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds an amount to the current balance
        /// </summary>
        /// <param name="amount">Amount to add (can be negative for subtraction)</param>
        public void AddAmount(decimal amount)
        {
            Balance += amount;
            LastUpdated = DateTime.UtcNow;
        }
    }
}
