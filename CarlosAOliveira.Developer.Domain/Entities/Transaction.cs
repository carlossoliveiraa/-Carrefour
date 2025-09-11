using CarlosAOliveira.Developer.Domain.Common;
using CarlosAOliveira.Developer.Domain.Events;
using CarlosAOliveira.Developer.Domain.Enums;

namespace CarlosAOliveira.Developer.Domain.Entities
{
    /// <summary>
    /// Represents a financial transaction in the cash flow system
    /// </summary>
    public class Transaction : BaseEntity
    {
        public DateOnly Date { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public string Category { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        private Transaction() { } // EF Core constructor

        public Transaction(DateOnly date, decimal amount, TransactionType type, string category, string description)
        {
            Id = Guid.NewGuid();
            Date = date;
            Amount = amount;
            Type = type;
            Category = category;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Checks if this is a credit transaction
        /// </summary>
        public bool IsCredit => Type == TransactionType.Credit;

        /// <summary>
        /// Checks if this is a debit transaction
        /// </summary>
        public bool IsDebit => Type == TransactionType.Debit;

        /// <summary>
        /// Gets the signed amount (positive for credit, negative for debit)
        /// </summary>
        public decimal SignedAmount => IsCredit ? Amount : -Amount;
    }
}