namespace CarlosAOliveira.Developer.Application.DTOs.DailySummary
{
    public class PeriodSummaryDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal NetAmount { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageDailyCredits { get; set; }
        public decimal AverageDailyDebits { get; set; }
        public decimal AverageDailyNet { get; set; }
        public List<DailySummaryDto> DailySummaries { get; set; } = new();
    }
}
