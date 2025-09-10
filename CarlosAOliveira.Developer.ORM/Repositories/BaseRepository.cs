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

        /// <summary>
        /// Gets an entity by ID
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync([id], cancellationToken);
        }

        /// <summary>
        /// Gets all entities
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets entities with pagination
        /// </summary>
        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken = default)
        {
            var totalCount = await DbSet.CountAsync(cancellationToken);
            var items = await DbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        /// <summary>
        /// Adds a new entity
        /// </summary>
        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        /// <summary>
        /// Adds multiple entities
        /// </summary>
        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
            return entities;
        }

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        public virtual Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            DbSet.Update(entity);
            return Task.FromResult(entity);
        }

        /// <summary>
        /// Updates multiple entities
        /// </summary>
        public virtual Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            DbSet.UpdateRange(entities);
            return Task.FromResult(entities);
        }

        /// <summary>
        /// Deletes an entity by ID
        /// </summary>
        public virtual async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return false;

            DbSet.Remove(entity);
            return true;
        }

        /// <summary>
        /// Deletes an entity
        /// </summary>
        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            DbSet.Remove(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Checks if an entity exists by ID
        /// </summary>
        public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet.AnyAsync(e => e.Id == id, cancellationToken);
        }

        /// <summary>
        /// Gets the count of entities
        /// </summary>
        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.CountAsync(cancellationToken);
        }

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
