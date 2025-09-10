using CarlosAOliveira.Developer.Domain.Enums;

namespace CarlosAOliveira.Developer.Application.DTOs.Transaction
{
    public class CreateTransactionDto
    {
        public Guid MerchantId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
