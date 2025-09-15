using CleanCode.Common.Security.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CleanCode.Common.Security
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            var secretKey = configuration["Jwt:SecretKey"]?.ToString();
            ArgumentException.ThrowIfNullOrWhiteSpace(secretKey);

            var key = Encoding.ASCII.GetBytes(secretKey);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
                
                // Configuração para aceitar token com ou sem "Bearer"
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                        
                        if (!string.IsNullOrEmpty(token))
                        {
                            // Se o token não começar com "Bearer ", adiciona automaticamente
                            if (!context.Request.Headers["Authorization"].ToString().StartsWith("Bearer "))
                            {
                                context.Request.Headers["Authorization"] = $"Bearer {token}";
                            }
                        }
                        
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
