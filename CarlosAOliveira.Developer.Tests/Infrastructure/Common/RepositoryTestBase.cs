using CarlosAOliveira.Developer.Domain.Common;
using CarlosAOliveira.Developer.ORM;
using CarlosAOliveira.Developer.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CarlosAOliveira.Developer.Tests.Infrastructure.Common
{
    /// <summary>
    /// Base class for repository tests with in-memory database setup
    /// </summary>
    public abstract class RepositoryTestBase<T> : TestBase where T : BaseEntity
    {
        protected DefaultContext Context { get; private set; } = null!;
        protected DbSet<T> DbSet { get; private set; } = null!;

        protected virtual void Setup()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new DefaultContext(options);
            DbSet = Context.Set<T>();
        }

        protected virtual void Cleanup()
        {
            Context?.Dispose();
        }

        protected async Task<T> AddEntityAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        protected async Task<IEnumerable<T>> AddEntitiesAsync(IEnumerable<T> entities)
        {
            await DbSet.AddRangeAsync(entities);
            await Context.SaveChangesAsync();
            return entities;
        }

        protected async Task<T?> GetEntityByIdAsync(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        protected async Task<IEnumerable<T>> GetAllEntitiesAsync()
        {
            return await DbSet.ToListAsync();
        }

        protected async Task<int> GetEntityCountAsync()
        {
            return await DbSet.CountAsync();
        }

        protected async Task<bool> EntityExistsAsync(Guid id)
        {
            return await DbSet.AnyAsync(e => e.Id == id);
        }
    }
}
