using Microsoft.Extensions.Diagnostics.HealthChecks;
using CleanCode.Common.Messaging.Interfaces;

namespace CleanCode.Api.HealthChecks
{
    /// <summary>
    /// Health check para verificar o serviço de mensageria
    /// </summary>
    public class MessagingHealthCheck : IHealthCheck
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessagingHealthCheck> _logger;

        public MessagingHealthCheck(IMessageService messageService, ILogger<MessagingHealthCheck> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Checking messaging service health...");

                // Testa o serviço de mensageria enviando uma mensagem de teste
                var testMessage = $"Health check test - {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
                var testQueue = "health_check_test";

                await _messageService.SendMessageAsync(testMessage, testQueue, cancellationToken);

                // Tenta ler mensagens para verificar se o serviço está funcionando
                var messages = await _messageService.ReadMessagesAsync(testQueue, cancellationToken);

                _logger.LogDebug("Messaging service health check passed");

                var data = new Dictionary<string, object>
                {
                    ["test_queue"] = testQueue,
                    ["messages_found"] = messages.Count(),
                    ["timestamp"] = DateTime.UtcNow
                };

                return HealthCheckResult.Healthy("Messaging service is healthy", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Messaging service health check failed with exception");
                return HealthCheckResult.Unhealthy("Messaging service health check failed", ex);
            }
        }
    }
}
