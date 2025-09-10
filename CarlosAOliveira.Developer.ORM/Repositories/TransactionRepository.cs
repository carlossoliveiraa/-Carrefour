using CarlosAOliveira.Developer.Domain.Entities;
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

        public async Task<IEnumerable<Transaction>> GetByMerchantIdAsync(Guid merchantId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(t => t.MerchantId == merchantId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

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
    }
}
