namespace CarlosAOliveira.Developer.Application.DTOs.DailySummary
{
    public class DailySummaryWithMerchantDto : DailySummaryDto
    {
        public string MerchantName { get; set; } = string.Empty;
        public string MerchantEmail { get; set; } = string.Empty;
    }
}
