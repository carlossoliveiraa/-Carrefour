using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarlosAOliveira.Developer.ORM.Repositories
{
    /// <summary>
    /// Repository implementation for DailySummary entity
    /// </summary>
    public class DailySummaryRepository : BaseRepository<DailySummary>, IDailySummaryRepository
    {
        public DailySummaryRepository(DefaultContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets a daily summary by merchant ID and date
        /// </summary>
        public async Task<DailySummary?> GetByMerchantIdAndDateAsync(Guid merchantId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(ds => ds.MerchantId == merchantId && ds.Date.Date == date.Date, cancellationToken);
        }

        /// <summary>
        /// Gets daily summaries for a merchant within a date range
        /// </summary>
        public async Task<IEnumerable<DailySummary>> GetByMerchantIdAndDateRangeAsync(
            Guid merchantId, 
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(ds => ds.MerchantId == merchantId && 
                           ds.Date >= startDate.Date && 
                           ds.Date <= endDate.Date)
                .OrderBy(ds => ds.Date)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets daily summaries with pagination for a merchant
        /// </summary>
        public async Task<(IEnumerable<DailySummary> Items, int TotalCount)> GetPagedByMerchantIdAsync(
            Guid merchantId,
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.Where(ds => ds.MerchantId == merchantId);
            var totalCount = await query.CountAsync(cancellationToken);
            
            var items = await query
                .OrderByDescending(ds => ds.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        /// <summary>
        /// Gets the total net amount for a merchant within a date range
        /// </summary>
        public async Task<decimal> GetTotalNetAmountByMerchantIdAndDateRangeAsync(
            Guid merchantId, 
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(ds => ds.MerchantId == merchantId && 
                           ds.Date >= startDate.Date && 
                           ds.Date <= endDate.Date)
                .SumAsync(ds => ds.NetAmount, cancellationToken);
        }

        /// <summary>
        /// Gets the total transaction count for a merchant within a date range
        /// </summary>
        public async Task<int> GetTotalTransactionCountByMerchantIdAndDateRangeAsync(
            Guid merchantId, 
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(ds => ds.MerchantId == merchantId && 
                           ds.Date >= startDate.Date && 
                           ds.Date <= endDate.Date)
                .SumAsync(ds => ds.TransactionCount, cancellationToken);
        }

        /// <summary>
        /// Gets daily summaries with positive balance for a merchant
        /// </summary>
        public async Task<IEnumerable<DailySummary>> GetPositiveBalanceByMerchantIdAsync(
            Guid merchantId, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(ds => ds.MerchantId == merchantId && ds.NetAmount > 0)
                .OrderByDescending(ds => ds.Date)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets daily summaries with negative balance for a merchant
        /// </summary>
        public async Task<IEnumerable<DailySummary>> GetNegativeBalanceByMerchantIdAsync(
            Guid merchantId, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(ds => ds.MerchantId == merchantId && ds.NetAmount < 0)
                .OrderByDescending(ds => ds.Date)
                .ToListAsync(cancellationToken);
        }
    }
}
