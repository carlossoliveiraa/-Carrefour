using AutoMapper;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.DailySummary;
using CarlosAOliveira.Developer.Application.Handlers.Base;
using CarlosAOliveira.Developer.Application.Queries.DailySummary;
using CarlosAOliveira.Developer.Domain.Repositories;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Handlers.Queries
{
    /// <summary>
    /// Handler for getting period summary
    /// </summary>
    public class GetPeriodSummaryHandler : BaseHandler, IRequestHandler<GetPeriodSummaryQuery, BaseResponse<PeriodSummaryDto>>
    {
        private readonly IDailySummaryRepository _dailySummaryRepository;

        public GetPeriodSummaryHandler(IMapper mapper, IDailySummaryRepository dailySummaryRepository) : base(mapper)
        {
            _dailySummaryRepository = dailySummaryRepository;
        }

        public async Task<BaseResponse<PeriodSummaryDto>> Handle(GetPeriodSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var dailySummaries = await _dailySummaryRepository.GetByMerchantIdAndDateRangeAsync(
                    request.MerchantId, 
                    request.StartDate, 
                    request.EndDate, 
                    cancellationToken);

                var summariesList = dailySummaries.ToList();
                
                var periodSummary = new PeriodSummaryDto
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalDays = (int)(request.EndDate - request.StartDate).TotalDays + 1,
                    TotalCredits = summariesList.Sum(s => s.TotalCredits),
                    TotalDebits = summariesList.Sum(s => s.TotalDebits),
                    TotalTransactions = summariesList.Sum(s => s.TransactionCount),
                    DailySummaries = Mapper.Map<List<DailySummaryDto>>(summariesList)
                };

                periodSummary.NetAmount = periodSummary.TotalCredits - periodSummary.TotalDebits;
                periodSummary.AverageDailyCredits = periodSummary.TotalDays > 0 ? periodSummary.TotalCredits / periodSummary.TotalDays : 0;
                periodSummary.AverageDailyDebits = periodSummary.TotalDays > 0 ? periodSummary.TotalDebits / periodSummary.TotalDays : 0;
                periodSummary.AverageDailyNet = periodSummary.TotalDays > 0 ? periodSummary.NetAmount / periodSummary.TotalDays : 0;

                return Success(periodSummary);
            }
            catch (Exception ex)
            {
                return Error<PeriodSummaryDto>($"Error retrieving period summary: {ex.Message}");
            }
        }
    }
}
