namespace CleanCode.Common.Messaging.Models
{
    /// <summary>
    /// Mensagem enviada quando uma transação é atualizada
    /// </summary>
    public class TransactionUpdatedMessage
    {
        /// <summary>
        /// ID da transação atualizada
        /// </summary>
        public Guid TransactionId { get; set; }

        /// <summary>
        /// Descrição da transação
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Tipo da transação (Credit/Debit)
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Data da transação
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Categoria da transação
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Observações da transação
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime UpdatedAt { get; set; }

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
