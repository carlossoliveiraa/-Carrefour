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
        public Guid MerchantId { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        private Transaction() { } // EF Core constructor

        public Transaction(Guid merchantId, decimal amount, TransactionType type, string description)
        {
            Id = Guid.NewGuid();
            MerchantId = merchantId;
            Amount = amount;
            Type = type;
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