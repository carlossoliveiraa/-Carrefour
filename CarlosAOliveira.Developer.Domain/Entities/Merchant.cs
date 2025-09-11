using CarlosAOliveira.Developer.Domain.Common;
using CarlosAOliveira.Developer.Domain.Events;
using CarlosAOliveira.Developer.Domain.Enums;

namespace CarlosAOliveira.Developer.Domain.Entities
{
    /// <summary>
    /// Represents a merchant in the cash flow system
    /// </summary>
    public class Merchant : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        private Merchant() { } // EF Core constructor

        public Merchant(string name, string email)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a new transaction for this merchant
        /// </summary>
        public Transaction CreateTransaction(DateOnly date, decimal amount, TransactionType type, string category, string description)
        {
            var transaction = new Transaction(date, amount, type, category, description);
            AddDomainEvent(new TransactionCreatedEvent(transaction));
            return transaction;
        }

        /// <summary>
        /// Updates merchant information
        /// </summary>
        public void UpdateInformation(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}