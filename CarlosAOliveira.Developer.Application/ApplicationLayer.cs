using AutoMapper;
using CarlosAOliveira.Developer.Application.Mappings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CarlosAOliveira.Developer.Application
{
    /// <summary>
    /// Application Layer configuration and dependency injection
    /// </summary>
    public static class ApplicationLayer
    {
        /// <summary>
        /// Registers all application layer services
        /// </summary>
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(ApplicationLayer));

            // MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
