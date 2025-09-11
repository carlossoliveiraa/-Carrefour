using CarlosAOliveira.Developer.Api.Configuration;
using CarlosAOliveira.Developer.Api.Middleware;
using CarlosAOliveira.Developer.Api.Services;
using CarlosAOliveira.Developer.Application;
using CarlosAOliveira.Developer.ORM;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/carrefour-api-.log", rollingInterval: Serilog.RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Carrefour API",
        Version = "v1",
        Description = "A sophisticated API for managing merchants and transactions",
        Contact = new OpenApiContact
        {
            Name = "Carlos Oliveira",
            Email = "carlos@example.com"
        }
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add application services
builder.Services.AddApplicationLayer();
builder.Services.AddOrmLayer(builder.Configuration);

// Add custom services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<CarlosAOliveira.Developer.Domain.Events.IEventQueue, CarlosAOliveira.Developer.Application.Services.FileEventQueue>();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Carrefour API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        c.DocumentTitle = "Carrefour API - Documentação";
        c.DefaultModelsExpandDepth(-1); // Hide models section by default
        c.DisplayRequestDuration();
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<CarlosAOliveira.Developer.ORM.DefaultContext>();
        context.Database.EnsureCreated();
        Log.Information("Database initialized successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error initializing database");
    }
}

Log.Information("Starting Carrefour API...");
app.Run();