using Microsoft.AspNetCore.Builder;

namespace CleanCode.IoC.Interfaces
{
    public interface IModuleInitializer
    {
        void Initialize(WebApplicationBuilder builder);
    }
}
