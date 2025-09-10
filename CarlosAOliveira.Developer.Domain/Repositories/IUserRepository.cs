using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;

namespace CarlosAOliveira.Developer.Domain.Repositories
{
    /// <summary>
    /// Repository interface for user operations
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a user by ID
        /// </summary>
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a user by email
        /// </summary>
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all users
        /// </summary>
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets users with pagination
        /// </summary>
        Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets users by role
        /// </summary>
        Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets users by status
        /// </summary>
        Task<IEnumerable<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new user
        /// </summary>
        Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing user
        /// </summary>
        Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a user exists by email
        /// </summary>
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
