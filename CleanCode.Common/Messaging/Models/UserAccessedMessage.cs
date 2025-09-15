namespace CleanCode.Common.Messaging.Models
{
    /// <summary>
    /// Mensagem enviada quando um usuário é acessado/consultado
    /// </summary>
    public class UserAccessedMessage
    {
        /// <summary>
        /// ID do usuário acessado
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
        /// Tipo de acesso (GET, UPDATE, DELETE, etc.)
        /// </summary>
        public string AccessType { get; set; } = string.Empty;

        /// <summary>
        /// Data do acesso
        /// </summary>
        public DateTime AccessedAt { get; set; }

        /// <summary>
        /// IP de origem da requisição
        /// </summary>
        public string? SourceIp { get; set; }

        /// <summary>
        /// User Agent da requisição
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// ID do usuário que fez a requisição (se autenticado)
        /// </summary>
        public Guid? RequestedBy { get; set; }
    }
}
