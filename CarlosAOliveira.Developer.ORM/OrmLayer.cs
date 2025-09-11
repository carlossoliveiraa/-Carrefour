using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Repositories;
using CarlosAOliveira.Developer.ORM.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

            // Identity Services
            services.AddIdentityCore<User>(options =>
            {
                // Password settings - mais flexível para desenvolvimento
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // User settings
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                
                // SignIn settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<DefaultContext>();

            // Repositories
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IDailySummaryRepository, DailySummaryRepository>();
            services.AddScoped<IDailyBalanceRepository, DailyBalanceRepository>();

            return services;
        }
    }
}
