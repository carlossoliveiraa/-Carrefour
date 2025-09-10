using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Domain.Repositories
{
    /// <summary>
    /// Repository interface for merchant operations
    /// </summary>
    public interface IMerchantRepository
    {
        /// <summary>
        /// Gets a merchant by ID
        /// </summary>
        Task<Merchant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a merchant by email
        /// </summary>
        Task<Merchant?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new merchant
        /// </summary>
        Task<Merchant> AddAsync(Merchant merchant, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing merchant
        /// </summary>
        Task<Merchant> UpdateAsync(Merchant merchant, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}