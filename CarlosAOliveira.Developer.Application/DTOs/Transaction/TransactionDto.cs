using CarlosAOliveira.Developer.Domain.Enums;

namespace CarlosAOliveira.Developer.Application.DTOs.Transaction
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public Guid MerchantId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsCredit { get; set; }
        public bool IsDebit { get; set; }
        public decimal SignedAmount { get; set; }
    }
}