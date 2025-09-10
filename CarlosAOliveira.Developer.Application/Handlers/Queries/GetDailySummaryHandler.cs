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
    /// Handler for getting daily summary
    /// </summary>
    public class GetDailySummaryHandler : BaseHandler, IRequestHandler<GetDailySummaryQuery, BaseResponse<DailySummaryDto>>
    {
        private readonly IDailySummaryRepository _dailySummaryRepository;

        public GetDailySummaryHandler(IMapper mapper, IDailySummaryRepository dailySummaryRepository) : base(mapper)
        {
            _dailySummaryRepository = dailySummaryRepository;
        }

        public async Task<BaseResponse<DailySummaryDto>> Handle(GetDailySummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var dailySummary = await _dailySummaryRepository.GetByMerchantIdAndDateAsync(request.MerchantId, request.Date);
                if (dailySummary == null)
                {
                    return Error<DailySummaryDto>("Daily summary not found for the specified date");
                }

                var dailySummaryDto = Mapper.Map<DailySummaryDto>(dailySummary);
                return Success(dailySummaryDto);
            }
            catch (Exception ex)
            {
                return Error<DailySummaryDto>($"Error retrieving daily summary: {ex.Message}");
            }
        }
    }
}
