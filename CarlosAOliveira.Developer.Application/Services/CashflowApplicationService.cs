using AutoMapper;
using CarlosAOliveira.Developer.Application.Commands.Cashflow;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Cashflow;
using CarlosAOliveira.Developer.Application.Queries.Cashflow;
using CarlosAOliveira.Developer.Application.Queries.DailySummary;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Services
{
    /// <summary>
    /// Application service for cashflow operations
    /// </summary>
    public class CashflowApplicationService : ICashflowApplicationService
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly IMetricsService _metrics;

        public CashflowApplicationService(
            IMediator mediator, 
            IMapper mapper, 
            ICacheService cache, 
            IMetricsService metrics)
        {
            _mediator = mediator;
            _mapper = mapper;
            _cache = cache;
            _metrics = metrics;
        }

        /// <summary>
        /// Creates a new transaction
        /// </summary>
        /// <param name="request">Transaction creation request</param>
        /// <returns>Created transaction response</returns>
        public async Task<BaseResponse<TransactionResponse>> CreateTransactionAsync(CreateCashflowTransactionRequest request)
        {
            using var timer = _metrics.StartTimer("transaction.create");
            _metrics.IncrementCounter("transaction.create.attempted");

            var command = _mapper.Map<CreateTransactionCommand>(request);
            var result = await _mediator.Send(command);

            if (result.Success)
            {
                _metrics.IncrementCounter("transaction.create.success");
                // Invalidate cache for daily balance
                await _cache.RemoveByPatternAsync($"daily_balance_{request.Date:yyyy-MM-dd}*");
            }
            else
            {
                _metrics.IncrementCounter("transaction.create.failed");
            }

            return result;
        }

        /// <summary>
        /// Gets a transaction by ID
        /// </summary>
        /// <param name="id">Transaction ID</param>
        /// <returns>Transaction response</returns>
        public async Task<BaseResponse<TransactionResponse>> GetTransactionAsync(Guid id)
        {
            using var timer = _metrics.StartTimer("transaction.get");
            _metrics.IncrementCounter("transaction.get.attempted");

            var cacheKey = $"transaction_{id}";
            var cachedResult = await _cache.GetAsync<BaseResponse<TransactionResponse>>(cacheKey);
            
            if (cachedResult != null)
            {
                _metrics.IncrementCounter("transaction.get.cache_hit");
                return cachedResult;
            }

            _metrics.IncrementCounter("transaction.get.cache_miss");
            var query = new GetTransactionByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result.Success)
            {
                await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
                _metrics.IncrementCounter("transaction.get.success");
            }
            else
            {
                _metrics.IncrementCounter("transaction.get.failed");
            }

            return result;
        }

        /// <summary>
        /// Gets daily balance for a specific date
        /// </summary>
        /// <param name="date">Date to get balance for</param>
        /// <returns>Daily balance response</returns>
        public async Task<BaseResponse<DailyBalanceResponse>> GetDailyBalanceAsync(DateOnly date)
        {
            using var timer = _metrics.StartTimer("daily_balance.get");
            _metrics.IncrementCounter("daily_balance.get.attempted");

            var cacheKey = $"daily_balance_{date:yyyy-MM-dd}";
            var cachedResult = await _cache.GetAsync<BaseResponse<DailyBalanceResponse>>(cacheKey);
            
            if (cachedResult != null)
            {
                _metrics.IncrementCounter("daily_balance.get.cache_hit");
                return cachedResult;
            }

            _metrics.IncrementCounter("daily_balance.get.cache_miss");
            var query = new GetDailyBalanceQuery { Date = date };
            var result = await _mediator.Send(query);

            if (result.Success)
            {
                await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
                _metrics.IncrementCounter("daily_balance.get.success");
            }
            else
            {
                _metrics.IncrementCounter("daily_balance.get.failed");
            }

            return result;
        }

        /// <summary>
        /// Gets daily summary for a merchant and date
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="date">Date</param>
        /// <returns>Daily summary response</returns>
        public async Task<BaseResponse<DailySummaryResponse>> GetDailySummaryAsync(Guid merchantId, DateTime date)
        {
            var query = new GetDailySummaryQuery(merchantId, date);
            var result = await _mediator.Send(query);
            
            if (result == null)
            {
                return BaseResponse<DailySummaryResponse>.CreateError("Daily summary not found");
            }

            var response = _mapper.Map<DailySummaryResponse>(result);
            return BaseResponse<DailySummaryResponse>.CreateSuccess(response);
        }

        /// <summary>
        /// Gets period summary for a merchant
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Period summary response</returns>
        public async Task<BaseResponse<PeriodSummaryResponse>> GetPeriodSummaryAsync(Guid merchantId, DateTime startDate, DateTime endDate)
        {
            var query = new GetPeriodSummaryQuery(merchantId, startDate, endDate);
            var result = await _mediator.Send(query);
            
            var response = _mapper.Map<PeriodSummaryResponse>(result);
            return BaseResponse<PeriodSummaryResponse>.CreateSuccess(response);
        }
    }
}
