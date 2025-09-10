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
        /// Gets all transactions for a merchant
        /// </summary>
        public async Task<IEnumerable<Transaction>> GetByMerchantIdAsync(Guid merchantId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(t => t.MerchantId == merchantId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets transactions for a merchant within a date range
        /// </summary>
        public async Task<IEnumerable<Transaction>> GetByMerchantIdAndDateRangeAsync(
            Guid merchantId, 
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(t => t.MerchantId == merchantId && 
                           t.CreatedAt >= startDate && 
                           t.CreatedAt <= endDate)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets transactions by type for a merchant
        /// </summary>
        public async Task<IEnumerable<Transaction>> GetByMerchantIdAndTypeAsync(
            Guid merchantId, 
            TransactionType type, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(t => t.MerchantId == merchantId && t.Type == type)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets transactions with pagination for a merchant
        /// </summary>
        public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetPagedByMerchantIdAsync(
            Guid merchantId,
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.Where(t => t.MerchantId == merchantId);
            var totalCount = await query.CountAsync(cancellationToken);
            
            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        /// <summary>
        /// Gets the total amount for a merchant by transaction type
        /// </summary>
        public async Task<decimal> GetTotalAmountByMerchantIdAndTypeAsync(
            Guid merchantId, 
            TransactionType type, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(t => t.MerchantId == merchantId && t.Type == type)
                .SumAsync(t => t.Amount, cancellationToken);
        }

        /// <summary>
        /// Gets the count of transactions for a merchant by type
        /// </summary>
        public async Task<int> GetCountByMerchantIdAndTypeAsync(
            Guid merchantId, 
            TransactionType type, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .CountAsync(t => t.MerchantId == merchantId && t.Type == type, cancellationToken);
        }
    }
}
