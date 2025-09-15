namespace CleanCode.Common.Messaging.Models
{
    /// <summary>
    /// Representa uma mensagem na fila
    /// </summary>
    public class QueueMessage
    {
        /// <summary>
        /// Identificador único da mensagem
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Conteúdo da mensagem
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Nome da fila
        /// </summary>
        public string QueueName { get; set; } = string.Empty;

        /// <summary>
        /// Data de criação da mensagem
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Tipo da mensagem
        /// </summary>
        public string MessageType { get; set; } = string.Empty;

        /// <summary>
        /// Dados adicionais em formato JSON
        /// </summary>
        public string? AdditionalData { get; set; }
    }
}
