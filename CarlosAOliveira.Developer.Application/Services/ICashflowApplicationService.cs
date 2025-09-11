using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Cashflow;

namespace CarlosAOliveira.Developer.Application.Services
{
    /// <summary>
    /// Application service for cashflow operations
    /// </summary>
    public interface ICashflowApplicationService
    {
        /// <summary>
        /// Creates a new transaction
        /// </summary>
        /// <param name="request">Transaction creation request</param>
        /// <returns>Created transaction response</returns>
        Task<BaseResponse<TransactionResponse>> CreateTransactionAsync(CreateCashflowTransactionRequest request);

        /// <summary>
        /// Gets a transaction by ID
        /// </summary>
        /// <param name="id">Transaction ID</param>
        /// <returns>Transaction response</returns>
        Task<BaseResponse<TransactionResponse>> GetTransactionAsync(Guid id);

        /// <summary>
        /// Gets daily balance for a specific date
        /// </summary>
        /// <param name="date">Date to get balance for</param>
        /// <returns>Daily balance response</returns>
        Task<BaseResponse<DailyBalanceResponse>> GetDailyBalanceAsync(DateOnly date);

        /// <summary>
        /// Gets daily summary for a merchant and date
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="date">Date</param>
        /// <returns>Daily summary response</returns>
        Task<BaseResponse<DailySummaryResponse>> GetDailySummaryAsync(Guid merchantId, DateTime date);

        /// <summary>
        /// Gets period summary for a merchant
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Period summary response</returns>
        Task<BaseResponse<PeriodSummaryResponse>> GetPeriodSummaryAsync(Guid merchantId, DateTime startDate, DateTime endDate);
    }
}
