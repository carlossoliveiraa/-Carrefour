using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarlosAOliveira.Developer.ORM.Repositories
{
    /// <summary>
    /// Repository implementation for User entity
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DefaultContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets a user by email
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        /// <summary>
        /// Checks if a user exists by email
        /// </summary>
        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await DbSet.AnyAsync(u => u.Email == email, cancellationToken);
        }

        /// <summary>
        /// Gets users with pagination
        /// </summary>
        public new async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken = default)
        {
            var totalCount = await DbSet.CountAsync(cancellationToken);
            var items = await DbSet
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        /// <summary>
        /// Gets users by role
        /// </summary>
        public async Task<IEnumerable<User>> GetByRoleAsync(Domain.Enums.UserRole role, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(u => u.Role == role)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets users by status
        /// </summary>
        public async Task<IEnumerable<User>> GetByStatusAsync(Domain.Enums.UserStatus status, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(u => u.Status == status)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
