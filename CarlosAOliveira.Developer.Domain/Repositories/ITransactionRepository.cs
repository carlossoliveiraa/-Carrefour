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
        /// Gets all transactions for a specific date
        /// </summary>
        Task<IEnumerable<Transaction>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets transactions within a date range
        /// </summary>
        Task<IEnumerable<Transaction>> GetByDateRangeAsync(
            DateOnly startDate, 
            DateOnly endDate, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets transactions by type
        /// </summary>
        Task<IEnumerable<Transaction>> GetByTypeAsync(
            Domain.Enums.TransactionType type, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets transactions with pagination
        /// </summary>
        Task<(IEnumerable<Transaction> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total amount by transaction type
        /// </summary>
        Task<decimal> GetTotalAmountByTypeAsync(
            Domain.Enums.TransactionType type, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of transactions by type
        /// </summary>
        Task<int> GetCountByTypeAsync(
            Domain.Enums.TransactionType type, 
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