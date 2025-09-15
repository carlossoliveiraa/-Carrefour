using CleanCode.Api.Middleware;
using CleanCode.Api.HealthChecks;
using CleanCode.Application;
using CleanCode.Common.HealthChecks;
using CleanCode.Common.Logging;
using CleanCode.Common.Messaging;
using CleanCode.Common.Security;
using CleanCode.Common.Validation;
using CleanCode.IoC;
using CleanCode.ORM;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.AddDefaultLogging();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.AddBasicHealthChecks();
            
            // Adiciona health checks específicos
            builder.Services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("Database", tags: ["readiness", "database"])
                .AddCheck<MessagingHealthCheck>("Messaging", tags: ["readiness", "messaging"]);
            
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
                { 
                    Title = "Clean Code API", 
                    Version = "v1",
                    Description = "API para controle de fluxo de caixa com autenticação JWT"
                });
                
                // Configuração para autenticação JWT no Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            
            // Adiciona o serviço de mensageria
            builder.Services.AddFileMessaging(builder.Configuration);

            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("CleanCode.ORM")
                )
            );

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.RegisterDependencies();

            builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            var app = builder.Build();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            // Middleware de logging aprimorado (após autenticação)
            app.UseMiddleware<EnhancedRequestLoggingMiddleware>();
            app.UseMiddleware<ValidationExceptionMiddleware>();

            app.UseBasicHealthChecks();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
