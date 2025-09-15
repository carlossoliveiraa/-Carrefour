using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Common.Messaging.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CleanCode.Common.Messaging
{
    /// <summary>
    /// Implementação do serviço de mensageria usando arquivos TXT
    /// </summary>
    public class FileMessageService : IMessageService
    {
        private readonly string _basePath;
        private readonly ILogger<FileMessageService> _logger;

        public FileMessageService(ILogger<FileMessageService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _basePath = configuration["Messaging:BasePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "queues");
            
            // Garante que o diretório base existe
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
                _logger.LogInformation("Created messaging directory: {BasePath}", _basePath);
            }
        }

        public async Task SendMessageAsync(string message, string queueName, CancellationToken cancellationToken = default)
        {
            try
            {
                var queueMessage = new QueueMessage
                {
                    Content = message,
                    QueueName = queueName,
                    MessageType = "text"
                };

                await SendMessageAsync(queueMessage, queueName, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending text message to queue {QueueName}", queueName);
                throw;
            }
        }

        public async Task SendMessageAsync<T>(T message, string queueName, CancellationToken cancellationToken = default)
        {
            try
            {
                var queueMessage = new QueueMessage
                {
                    Content = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true }),
                    QueueName = queueName,
                    MessageType = typeof(T).Name,
                    AdditionalData = JsonSerializer.Serialize(new { Type = typeof(T).FullName })
                };

                var queueDirectory = Path.Combine(_basePath, queueName);
                if (!Directory.Exists(queueDirectory))
                {
                    Directory.CreateDirectory(queueDirectory);
                    _logger.LogInformation("Created queue directory: {QueueDirectory}", queueDirectory);
                }

                var fileName = $"{queueMessage.Id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt";
                var filePath = Path.Combine(queueDirectory, fileName);

                var fileContent = $"""
                    Message ID: {queueMessage.Id}
                    Queue: {queueMessage.QueueName}
                    Type: {queueMessage.MessageType}
                    Created At: {queueMessage.CreatedAt:yyyy-MM-dd HH:mm:ss UTC}
                    
                    Content:
                    {queueMessage.Content}
                    
                    Additional Data:
                    {queueMessage.AdditionalData}
                    """;

                await File.WriteAllTextAsync(filePath, fileContent, cancellationToken);

                _logger.LogInformation("Message sent to queue {QueueName} with ID {MessageId}", queueName, queueMessage.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to queue {QueueName}", queueName);
                throw;
            }
        }

        public async Task<IEnumerable<string>> ReadMessagesAsync(string queueName, CancellationToken cancellationToken = default)
        {
            try
            {
                var queueDirectory = Path.Combine(_basePath, queueName);
                if (!Directory.Exists(queueDirectory))
                {
                    _logger.LogWarning("Queue directory not found: {QueueDirectory}", queueDirectory);
                    return Enumerable.Empty<string>();
                }

                var files = Directory.GetFiles(queueDirectory, "*.txt")
                    .OrderBy(f => File.GetCreationTime(f))
                    .ToArray();

                var messages = new List<string>();

                foreach (var file in files)
                {
                    try
                    {
                        var content = await File.ReadAllTextAsync(file, cancellationToken);
                        messages.Add(content);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error reading message file: {FilePath}", file);
                    }
                }

                _logger.LogInformation("Read {MessageCount} messages from queue {QueueName}", messages.Count, queueName);
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading messages from queue {QueueName}", queueName);
                throw;
            }
        }
    }
}
