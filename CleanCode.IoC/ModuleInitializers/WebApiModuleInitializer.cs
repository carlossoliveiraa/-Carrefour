using CleanCode.IoC.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CleanCode.IoC.ModuleInitializers
{
    public class WebApiModuleInitializer : IModuleInitializer
    {
        public void Initialize(WebApplicationBuilder builder)
        {

            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();
        }
    }
}
