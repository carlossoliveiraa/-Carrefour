using CleanCode.Common.Security.Interfaces;
using CleanCode.Common.Security;
using CleanCode.IoC.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CleanCode.IoC.ModuleInitializers
{
    public class ApplicationModuleInitializer : IModuleInitializer
    {
        public void Initialize(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        }
    }
}
