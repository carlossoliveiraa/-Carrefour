using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Domain.Repositories
{
    /// <summary>
    /// Repository interface for daily summary operations
    /// </summary>
    public interface IDailySummaryRepository
    {
        /// <summary>
        /// Gets a daily summary by merchant ID and date
        /// </summary>
        Task<DailySummary?> GetByMerchantIdAndDateAsync(Guid merchantId, DateTime date, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets daily summaries for a merchant within a date range
        /// </summary>
        Task<IEnumerable<DailySummary>> GetByMerchantIdAndDateRangeAsync(
            Guid merchantId, 
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new daily summary
        /// </summary>
        Task<DailySummary> AddAsync(DailySummary dailySummary, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing daily summary
        /// </summary>
        Task<DailySummary> UpdateAsync(DailySummary dailySummary, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}