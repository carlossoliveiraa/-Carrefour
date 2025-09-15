using CleanCode.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace CleanCode.ORM
{
    public sealed class DefaultContext : DbContext
    {
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Transaction> Transactions { get; set; } = default!;
        public DbSet<DailyBalance> DailyBalances { get; set; } = default!;

        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }

    /// <summary>
    /// Usada pelo EF Tools (Add-Migration/Update-Database) para criar o DbContext em tempo de design.
    /// </summary>
    public sealed class DefaultContextFactory : IDesignTimeDbContextFactory<DefaultContext>
    {
        public DefaultContext CreateDbContext(string[] args)
        {
            // Quando você roda Add-Migration, o working directory é o do Startup Project.
            // Então o appsettings deve ser encontrado lá (ex.: CleanCode.Api).
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // pasta do StartupProject
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' não encontrada. " +
                    "Adicione em appsettings*.json do StartupProject.");

            var builder = new DbContextOptionsBuilder<DefaultContext>();

            builder.UseSqlServer(
                connectionString,
                sql => sql.MigrationsAssembly(typeof(DefaultContext).Assembly.FullName) // migrações no projeto ORM
            );

            return new DefaultContext(builder.Options);
        }
    }
}
