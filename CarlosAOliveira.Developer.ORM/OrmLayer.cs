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
            // Database Context
            services.AddDbContext<DefaultContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IDailySummaryRepository, DailySummaryRepository>();

            return services;
        }
    }
}
