namespace CarlosAOliveira.Developer.Application.DTOs.Merchant
{
    public class MerchantWithSummaryDto : MerchantDto
    {
        public int TotalTransactions { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime LastTransactionDate { get; set; }
    }
}
