using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Domain.Repositories
{
    /// <summary>
    /// Repository interface for daily balance operations
    /// </summary>
    public interface IDailyBalanceRepository
    {
        /// <summary>
        /// Gets the daily balance for a specific date
        /// </summary>
        Task<DailyBalance?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets daily balances within a date range
        /// </summary>
        Task<IEnumerable<DailyBalance>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new daily balance
        /// </summary>
        Task<DailyBalance> AddAsync(DailyBalance dailyBalance, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing daily balance
        /// </summary>
        Task<DailyBalance> UpdateAsync(DailyBalance dailyBalance, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
