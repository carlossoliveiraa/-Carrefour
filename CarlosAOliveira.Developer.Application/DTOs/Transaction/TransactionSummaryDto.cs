namespace CarlosAOliveira.Developer.Application.DTOs.Transaction
{
    public class TransactionSummaryDto
    {
        public int TotalTransactions { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}
