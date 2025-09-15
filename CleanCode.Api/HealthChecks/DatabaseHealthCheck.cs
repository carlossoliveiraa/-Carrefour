using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using CleanCode.ORM;

namespace CleanCode.Api.HealthChecks
{
    /// <summary>
    /// Health check para verificar a conectividade com o banco de dados
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly DefaultContext _context;
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(DefaultContext context, ILogger<DatabaseHealthCheck> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Checking database health...");

                // Verifica se consegue conectar ao banco
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
                if (!canConnect)
                {
                    _logger.LogWarning("Database health check failed: Cannot connect to database");
                    return HealthCheckResult.Unhealthy("Cannot connect to database");
                }

                // Verifica se consegue executar uma query simples
                var userCount = await _context.Users.CountAsync(cancellationToken);
                
                _logger.LogDebug("Database health check passed. User count: {UserCount}", userCount);

                var data = new Dictionary<string, object>
                {
                    ["user_count"] = userCount,
                    ["database"] = _context.Database.GetConnectionString()?.Split(';').FirstOrDefault() ?? "Unknown",
                    ["timestamp"] = DateTime.UtcNow
                };

                return HealthCheckResult.Healthy("Database is healthy", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed with exception");
                return HealthCheckResult.Unhealthy("Database health check failed", ex);
            }
        }
    }
}
