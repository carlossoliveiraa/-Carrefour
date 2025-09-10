using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Domain.Repositories
{
    /// <summary>
    /// Repository interface for transaction operations
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Gets a transaction by ID
        /// </summary>
        Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all transactions for a merchant
        /// </summary>
        Task<IEnumerable<Transaction>> GetByMerchantIdAsync(Guid merchantId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets transactions for a merchant within a date range
        /// </summary>
        Task<IEnumerable<Transaction>> GetByMerchantIdAndDateRangeAsync(
            Guid merchantId, 
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new transaction
        /// </summary>
        Task<Transaction> AddAsync(Transaction transaction, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}