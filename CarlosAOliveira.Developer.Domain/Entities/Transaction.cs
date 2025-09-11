using CarlosAOliveira.Developer.Domain.Common;
using CarlosAOliveira.Developer.Domain.Events;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Domain.ValueObjects;

namespace CarlosAOliveira.Developer.Domain.Entities
{
    /// <summary>
    /// Represents a financial transaction in the cash flow system
    /// </summary>
    public class Transaction : BaseEntity
    {
        /// <summary>
        /// Transaction date
        /// </summary>
        public DateOnly Date { get; private set; }

        /// <summary>
        /// Transaction amount
        /// </summary>
        public Money Amount { get; private set; }

        /// <summary>
        /// Transaction type (Credit or Debit)
        /// </summary>
        public TransactionType Type { get; private set; }

        /// <summary>
        /// Transaction category
        /// </summary>
        public string Category { get; private set; } = string.Empty;

        /// <summary>
        /// Transaction description
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        private Transaction() 
        { 
            Amount = new Money(0);
        } // EF Core constructor

        /// <summary>
        /// Initializes a new instance of Transaction
        /// </summary>
        /// <param name="date">Transaction date</param>
        /// <param name="amount">Transaction amount</param>
        /// <param name="type">Transaction type</param>
        /// <param name="category">Transaction category</param>
        /// <param name="description">Transaction description</param>
        public Transaction(DateOnly date, decimal amount, TransactionType type, string category, string description)
        {
            Id = Guid.NewGuid();
            Date = date;
            Amount = new Money(amount);
            Type = type;
            Category = category ?? throw new ArgumentNullException(nameof(category));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            CreatedAt = DateTime.UtcNow;

            // Add domain event
            AddDomainEvent(new TransactionCreatedEvent(this));
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
        public decimal SignedAmount => IsCredit ? Amount.Amount : -Amount.Amount;

        /// <summary>
        /// Gets the absolute amount value
        /// </summary>
        public decimal AbsoluteAmount => Amount.Amount;
    }
}