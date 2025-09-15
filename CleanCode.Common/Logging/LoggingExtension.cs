using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Sinks.SystemConsole.Themes;
using System.Diagnostics;

namespace CleanCode.Common.Logging
{
    /// <summary> Add default Logging configuration to project. This configuration supports Serilog logs with DataDog compatible output.</summary>
    public static class LoggingExtension
    {
        /// <summary>
        /// The destructuring options builder configured with default destructurers and a custom DbUpdateExceptionDestructurer.
        /// </summary>
        static readonly DestructuringOptionsBuilder _destructuringOptionsBuilder = new DestructuringOptionsBuilder()
            .WithDefaultDestructurers()
            .WithDestructurers([new DbUpdateExceptionDestructurer()]);

        /// <summary>
        /// A filter predicate to exclude log events with specific criteria.
        /// </summary>
        static readonly Func<LogEvent, bool> _filterPredicate = exclusionPredicate =>
        {
            // Não filtrar logs de erro ou warning
            if (exclusionPredicate.Level >= LogEventLevel.Warning) return true;

            // Filtrar health checks em modo Information (exceto se for erro)
            exclusionPredicate.Properties.TryGetValue("Path", out var path);
            var excludeByPath = path?.ToString().Contains("/health") ?? false;

            // Filtrar logs de request muito verbosos
            exclusionPredicate.Properties.TryGetValue("RequestPath", out var requestPath);
            var excludeRequestPath = requestPath?.ToString().Contains("/health") ?? false;

            return !excludeByPath && !excludeRequestPath;
        };

        /// <summary>
        /// This method configures the logging with commonly used features for DataDog integration.
        /// </summary>
        /// <param name="builder">The <see cref="WebApplicationBuilder" /> to add services to.</param>
        /// <returns>A <see cref="WebApplicationBuilder"/> that can be used to further configure the API services.</returns>
        /// <remarks>
        /// <para>Logging output are diferents on Debug and Release modes.</para>
        /// </remarks> 
        public static WebApplicationBuilder AddDefaultLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
                    .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails(_destructuringOptionsBuilder)
                    .Filter.ByExcluding(_filterPredicate);
            });

            builder.Services.AddLogging();

            return builder;
        }

        /// <summary>Adds middleware for Swagger documetation generation.</summary>
        /// <param name="app">The <see cref="WebApplication"/> instance this method extends.</param>
        /// <returns>The <see cref="WebApplication"/> for Swagger documentation.</returns>
        public static WebApplication UseDefaultLogging(this WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Logger>>();

            var mode = Debugger.IsAttached ? "Debug" : "Release";
            logger.LogInformation("Logging enabled for '{Application}' on '{Environment}' - Mode: {Mode}", app.Environment.ApplicationName, app.Environment.EnvironmentName, mode);
            return app;

        }
    }
}