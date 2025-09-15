using CleanCode.Domain.Common;

namespace CleanCode.Domain.Entities
{
    /// <summary>
    /// Entidade para armazenar logs da aplicação
    /// </summary>
    public class LogEntry : BaseEntity
    {
        /// <summary>
        /// Timestamp do log
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Nível do log (Information, Warning, Error, etc.)
        /// </summary>
        public string Level { get; set; } = string.Empty;

        /// <summary>
        /// Mensagem do log
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Exceção (se houver)
        /// </summary>
        public string? Exception { get; set; }

        /// <summary>
        /// Propriedades adicionais do log (JSON)
        /// </summary>
        public string? Properties { get; set; }

        /// <summary>
        /// Nome da máquina
        /// </summary>
        public string? MachineName { get; set; }

        /// <summary>
        /// ID do processo
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// ID da thread
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Contexto da fonte do log
        /// </summary>
        public string? SourceContext { get; set; }

        /// <summary>
        /// ID da requisição (se aplicável)
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Método HTTP (se aplicável)
        /// </summary>
        public string? HttpMethod { get; set; }

        /// <summary>
        /// Path da requisição (se aplicável)
        /// </summary>
        public string? RequestPath { get; set; }

        /// <summary>
        /// Status code da resposta (se aplicável)
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// IP do cliente (se aplicável)
        /// </summary>
        public string? ClientIp { get; set; }

        /// <summary>
        /// User Agent (se aplicável)
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Duração da requisição em milissegundos (se aplicável)
        /// </summary>
        public long? Duration { get; set; }
    }
}
