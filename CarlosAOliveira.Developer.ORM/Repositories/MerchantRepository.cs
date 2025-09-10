using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarlosAOliveira.Developer.ORM.Repositories
{
    /// <summary>
    /// Repository implementation for Merchant entity
    /// </summary>
    public class MerchantRepository : BaseRepository<Merchant>, IMerchantRepository
    {
        public MerchantRepository(DefaultContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets a merchant by email
        /// </summary>
        public async Task<Merchant?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(m => m.Email == email, cancellationToken);
        }

        /// <summary>
        /// Gets merchants with pagination
        /// </summary>
        public new async Task<(IEnumerable<Merchant> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken = default)
        {
            var totalCount = await DbSet.CountAsync(cancellationToken);
            var items = await DbSet
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        /// <summary>
        /// Gets merchants created within a date range
        /// </summary>
        public async Task<IEnumerable<Merchant>> GetByDateRangeAsync(
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Checks if a merchant exists by email
        /// </summary>
        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await DbSet.AnyAsync(m => m.Email == email, cancellationToken);
        }
    }
}
