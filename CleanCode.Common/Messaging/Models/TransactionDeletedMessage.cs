namespace CleanCode.Common.Messaging.Models
{
    /// <summary>
    /// Mensagem enviada quando uma transação é deletada
    /// </summary>
    public class TransactionDeletedMessage
    {
        /// <summary>
        /// ID da transação deletada
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
        /// Data de exclusão
        /// </summary>
        public DateTime DeletedAt { get; set; }

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
