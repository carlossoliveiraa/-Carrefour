using CarlosAOliveira.Developer.Domain.Common;
using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Domain.Events
{
    /// <summary>
    /// Domain event raised when a new transaction is created
    /// </summary>
    public class TransactionCreatedEvent : IDomainEvent
    {
        public Guid TransactionId { get; }
        public DateOnly Date { get; }
        public decimal Amount { get; }
        public string TransactionType { get; }
        public string Category { get; }
        public string Description { get; }
        public DateTime CreatedAt { get; }

        public TransactionCreatedEvent(Transaction transaction)
        {
            TransactionId = transaction.Id;
            Date = transaction.Date;
            Amount = transaction.Amount;
            TransactionType = transaction.Type.ToString();
            Category = transaction.Category;
            Description = transaction.Description;
            CreatedAt = transaction.CreatedAt;
        }
    }
}
