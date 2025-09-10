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

        public async Task<DailySummary?> GetByMerchantIdAndDateAsync(Guid merchantId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(ds => ds.MerchantId == merchantId && ds.Date.Date == date.Date, cancellationToken);
        }

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
    }
}
