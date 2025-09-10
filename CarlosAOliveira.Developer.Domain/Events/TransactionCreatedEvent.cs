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
        public Guid MerchantId { get; }
        public decimal Amount { get; }
        public string TransactionType { get; }
        public DateTime CreatedAt { get; }

        public TransactionCreatedEvent(Transaction transaction)
        {
            TransactionId = transaction.Id;
            MerchantId = transaction.MerchantId;
            Amount = transaction.Amount;
            TransactionType = transaction.Type.ToString();
            CreatedAt = transaction.CreatedAt;
        }
    }
}
