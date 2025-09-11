using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.ORM.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace CarlosAOliveira.Developer.ORM
{
    public class DefaultContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<DailySummary> DailySummaries { get; set; }

        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply Identity configurations first
            base.OnModelCreating(modelBuilder);
            
            // Apply custom configurations for other entities only (excluding User)
            modelBuilder.ApplyConfiguration(new MerchantConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new DailySummaryConfiguration());
        }
    }
    public class DefaultContextFactory : IDesignTimeDbContextFactory<DefaultContext>
    {
        public DefaultContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "CarlosAOliveira.Developer.Api"))
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var builder = new DbContextOptionsBuilder<DefaultContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
                       
            builder.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly("CarlosAOliveira.Developer.ORM")
            );

            return new DefaultContext(builder.Options);
        }
    }
}
