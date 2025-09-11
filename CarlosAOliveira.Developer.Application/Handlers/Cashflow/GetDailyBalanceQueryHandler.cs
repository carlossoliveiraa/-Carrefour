using AutoMapper;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Cashflow;
using CarlosAOliveira.Developer.Application.Queries.Cashflow;
using CarlosAOliveira.Developer.Domain.Repositories;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Handlers.Cashflow
{
    /// <summary>
    /// Handler for getting daily balance
    /// </summary>
    public class GetDailyBalanceQueryHandler : IRequestHandler<GetDailyBalanceQuery, BaseResponse<DailyBalanceResponse>>
    {
        private readonly IDailyBalanceRepository _dailyBalanceRepository;
        private readonly IMapper _mapper;

        public GetDailyBalanceQueryHandler(IDailyBalanceRepository dailyBalanceRepository, IMapper mapper)
        {
            _dailyBalanceRepository = dailyBalanceRepository;
            _mapper = mapper;
        }

        public async Task<BaseResponse<DailyBalanceResponse>> Handle(GetDailyBalanceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var dailyBalance = await _dailyBalanceRepository.GetByDateAsync(request.Date, cancellationToken);
                
                if (dailyBalance == null)
                {
                    // Return zero balance if no record exists for the date
                    var zeroBalance = new DailyBalanceResponse
                    {
                        Date = request.Date,
                        Balance = 0,
                        LastUpdated = DateTime.UtcNow
                    };
                    return BaseResponse<DailyBalanceResponse>.CreateSuccess(zeroBalance);
                }

                var response = _mapper.Map<DailyBalanceResponse>(dailyBalance);
                return BaseResponse<DailyBalanceResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                return BaseResponse<DailyBalanceResponse>.CreateError($"Error retrieving daily balance: {ex.Message}");
            }
        }
    }
}
