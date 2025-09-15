namespace CleanCode.Common.Messaging.Models
{
    /// <summary>
    /// Mensagem enviada quando um saldo diário é consolidado
    /// </summary>
    public class DailyBalanceConsolidatedMessage
    {
        /// <summary>
        /// ID do saldo diário consolidado
        /// </summary>
        public Guid DailyBalanceId { get; set; }

        /// <summary>
        /// Data do saldo
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Saldo de abertura
        /// </summary>
        public decimal OpeningBalance { get; set; }

        /// <summary>
        /// Total de créditos
        /// </summary>
        public decimal TotalCredits { get; set; }

        /// <summary>
        /// Total de débitos
        /// </summary>
        public decimal TotalDebits { get; set; }

        /// <summary>
        /// Saldo de fechamento
        /// </summary>
        public decimal ClosingBalance { get; set; }

        /// <summary>
        /// Quantidade de transações de crédito
        /// </summary>
        public int CreditTransactionCount { get; set; }

        /// <summary>
        /// Quantidade de transações de débito
        /// </summary>
        public int DebitTransactionCount { get; set; }

        /// <summary>
        /// Total de transações
        /// </summary>
        public int TotalTransactionCount { get; set; }

        /// <summary>
        /// Indica se foi criado novo saldo ou atualizado existente
        /// </summary>
        public bool WasCreated { get; set; }

        /// <summary>
        /// Data de consolidação
        /// </summary>
        public DateTime ConsolidatedAt { get; set; }

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
