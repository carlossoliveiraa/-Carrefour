using CarlosAOliveira.Developer.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CarlosAOliveira.Developer.ORM.Repositories
{
    /// <summary>
    /// Base repository implementation with common functionality
    /// </summary>
    public abstract class BaseRepository<T> where T : BaseEntity
    {
        protected readonly DefaultContext Context;
        protected readonly DbSet<T> DbSet;

        protected BaseRepository(DefaultContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync([id], cancellationToken);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public virtual Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            DbSet.Update(entity);
            return Task.FromResult(entity);
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
