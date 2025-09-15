namespace CleanCode.Common.Messaging.Models
{
    /// <summary>
    /// Mensagem enviada quando um usuário é criado
    /// </summary>
    public class UserCreatedMessage
    {
        /// <summary>
        /// ID do usuário criado
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Nome de usuário
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Papel do usuário
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Status do usuário
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// IP de origem da requisição
        /// </summary>
        public string? SourceIp { get; set; }

        /// <summary>
        /// User Agent da requisição
        /// </summary>
        public string? UserAgent { get; set; }
    }
}
