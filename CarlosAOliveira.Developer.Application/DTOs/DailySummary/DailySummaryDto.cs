namespace CarlosAOliveira.Developer.Application.DTOs.DailySummary
{
    public class DailySummaryDto
    {
        public Guid Id { get; set; }
        public Guid MerchantId { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal NetAmount { get; set; }
        public int TransactionCount { get; set; }
        public bool HasPositiveBalance { get; set; }
        public bool HasNegativeBalance { get; set; }
        public bool IsBalanced { get; set; }
    }
}