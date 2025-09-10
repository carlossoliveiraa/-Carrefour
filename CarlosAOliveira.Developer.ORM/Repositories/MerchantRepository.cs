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

        public async Task<Merchant?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(m => m.Email == email, cancellationToken);
        }
    }
}
