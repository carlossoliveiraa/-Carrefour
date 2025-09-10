using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.DailySummary;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Queries.DailySummary
{
    /// <summary>
    /// Query to get daily summary for a specific merchant and date
    /// </summary>
    public class GetDailySummaryQuery : IRequest<BaseResponse<DailySummaryDto>>
    {
        public Guid MerchantId { get; set; }
        public DateTime Date { get; set; }

        public GetDailySummaryQuery(Guid merchantId, DateTime date)
        {
            MerchantId = merchantId;
            Date = date.Date; // Ensure only date part
        }
    }
}
