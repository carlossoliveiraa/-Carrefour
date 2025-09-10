using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.DailySummary;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Queries.DailySummary
{
    /// <summary>
    /// Query to get period summary for a merchant (multiple days)
    /// </summary>
    public class GetPeriodSummaryQuery : IRequest<BaseResponse<PeriodSummaryDto>>
    {
        public Guid MerchantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public GetPeriodSummaryQuery(Guid merchantId, DateTime startDate, DateTime endDate)
        {
            MerchantId = merchantId;
            StartDate = startDate.Date;
            EndDate = endDate.Date;
        }
    }
}
