using CarlosAOliveira.Developer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace CarlosAOliveira.Developer.ORM
{
    public class DefaultContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<DailySummary> DailySummaries { get; set; }

        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
    //public class YourDbContextFactory : IDesignTimeDbContextFactory<DefaultContext>
    //{
    //    public DefaultContext CreateDbContext(string[] args)
    //    {
    //        IConfigurationRoot configuration = new ConfigurationBuilder()
    //            .SetBasePath(Directory.GetCurrentDirectory())
    //            .AddJsonFile("appsettings.json")
    //            .Build();

    //        var builder = new DbContextOptionsBuilder<DefaultContext>();
    //        var connectionString = configuration.GetConnectionString("DefaultConnection");
                       
    //        builder.UseSqlServer(
    //            connectionString,
    //            b => b.MigrationsAssembly("CarlosAOliveira.Developer.WebApi")
    //        );

    //        return new DefaultContext(builder.Options);
    //    }
    //}
}
