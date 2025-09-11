using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Cashflow;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Queries.Cashflow
{
    /// <summary>
    /// Query to get daily balance for a specific date
    /// </summary>
    public class GetDailyBalanceQuery : IRequest<BaseResponse<DailyBalanceResponse>>
    {
        /// <summary>
        /// Date to get balance for
        /// </summary>
        public DateOnly Date { get; set; }
    }
}
