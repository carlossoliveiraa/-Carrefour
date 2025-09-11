using AutoMapper;
using CarlosAOliveira.Developer.Application.Mappings;
using CarlosAOliveira.Developer.Application.Services;
using CarlosAOliveira.Developer.Common.Validation;
using FluentValidation;
using MediatR;
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

            // Validation Behavior for MediatR
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Application Services
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<ICashflowApplicationService, CashflowApplicationService>();
            services.AddScoped<ICacheService, MemoryCacheService>();
            services.AddScoped<IMetricsService, LoggingMetricsService>();

            // Memory Cache
            services.AddMemoryCache();

            return services;
        }
    }
}
