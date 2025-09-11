using CarlosAOliveira.Developer.Domain.Repositories;
using CarlosAOliveira.Developer.ORM.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarlosAOliveira.Developer.ORM
{
    /// <summary>
    /// ORM Layer configuration and dependency injection
    /// </summary>
    public static class OrmLayer
    {
        /// <summary>
        /// Registers all ORM layer services
        /// </summary>
        public static IServiceCollection AddOrmLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context - SQLite by default, SQL Server if connection string is provided
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<DefaultContext>(options =>
            {
                if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("Server="))
                {
                    options.UseSqlServer(connectionString);
                }
                else
                {
                    options.UseSqlite("Data Source=./runtime/cashflow.db");
                }
            });

            // Identity Services removed - User entity was deleted

            // Repositories
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IDailySummaryRepository, DailySummaryRepository>();
            services.AddScoped<IDailyBalanceRepository, DailyBalanceRepository>();

            return services;
        }
    }
}
