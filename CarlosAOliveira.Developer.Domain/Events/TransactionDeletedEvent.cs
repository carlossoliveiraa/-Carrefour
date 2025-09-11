using CarlosAOliveira.Developer.Domain.Common;
using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Domain.Events
{
    /// <summary>
    /// Domain event raised when a transaction is deleted
    /// </summary>
    public class TransactionDeletedEvent : IDomainEvent
    {
        public Guid TransactionId { get; }
        public DateOnly Date { get; }
        public decimal Amount { get; }
        public string TransactionType { get; }
        public DateTime DeletedAt { get; }

        public TransactionDeletedEvent(Transaction transaction)
        {
            TransactionId = transaction.Id;
            Date = transaction.Date;
            Amount = transaction.Amount;
            TransactionType = transaction.Type.ToString();
            DeletedAt = DateTime.UtcNow;
        }
    }
}
