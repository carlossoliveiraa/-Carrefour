using CleanCode.Domain.Repositories.Interfaces;
using CleanCode.IoC.Interfaces;
using CleanCode.ORM;
using CleanCode.ORM.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CleanCode.IoC.ModuleInitializers
{
    public class InfrastructureModuleInitializer : IModuleInitializer
    {
        public void Initialize(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<IDailyBalanceRepository, DailyBalanceRepository>();
        }
    }
}
