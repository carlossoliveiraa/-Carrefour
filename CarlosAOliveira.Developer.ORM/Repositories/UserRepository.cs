using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarlosAOliveira.Developer.ORM.Repositories
{
    /// <summary>
    /// User repository implementation
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DefaultContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets a user by email
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>User if found, null otherwise</returns>
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await Context.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        /// <summary>
        /// Gets users by role
        /// </summary>
        /// <param name="role">User role</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of users with the specified role</returns>
        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
        {
            return await Context.Users
                .Where(u => u.Role == role)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets users by status
        /// </summary>
        /// <param name="status">User status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of users with the specified status</returns>
        public async Task<IEnumerable<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
        {
            return await Context.Users
                .Where(u => u.Status == status)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Checks if a user exists by email
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if user exists, false otherwise</returns>
        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await Context.Users
                .AnyAsync(u => u.Email == email, cancellationToken);
        }
    }
}
