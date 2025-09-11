using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarlosAOliveira.Developer.ORM.Repositories
{
    /// <summary>
    /// Repository implementation for Transaction entity
    /// </summary>
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(DefaultContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets all transactions for a specific date
        /// </summary>
        public async Task<IEnumerable<Transaction>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(t => t.Date == date)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets transactions within a date range
        /// </summary>
        public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(
            DateOnly startDate, 
            DateOnly endDate, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets transactions by type
        /// </summary>
        public async Task<IEnumerable<Transaction>> GetByTypeAsync(
            TransactionType type, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(t => t.Type == type)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets transactions with pagination
        /// </summary>
        public new async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.AsQueryable();
            var totalCount = await query.CountAsync(cancellationToken);
            
            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        /// <summary>
        /// Gets the total amount by transaction type
        /// </summary>
        public async Task<decimal> GetTotalAmountByTypeAsync(
            TransactionType type, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(t => t.Type == type)
                .SumAsync(t => t.Amount, cancellationToken);
        }

        /// <summary>
        /// Gets the count of transactions by type
        /// </summary>
        public async Task<int> GetCountByTypeAsync(
            TransactionType type, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .CountAsync(t => t.Type == type, cancellationToken);
        }
    }
}
