using CleanCode.Domain.Entities;

namespace CleanCode.Domain.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
