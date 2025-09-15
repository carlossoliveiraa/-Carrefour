namespace CleanCode.Api.Features.DailyBalance.GetDailyBalance
{
    /// <summary>
    /// Response da busca de saldo diário
    /// </summary>
    public class GetDailyBalanceResponse
    {
        /// <summary>
        /// ID do saldo
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Data do saldo
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Saldo inicial do dia
        /// </summary>
        public decimal OpeningBalance { get; set; }

        /// <summary>
        /// Total de créditos do dia
        /// </summary>
        public decimal TotalCredits { get; set; }

        /// <summary>
        /// Total de débitos do dia
        /// </summary>
        public decimal TotalDebits { get; set; }

        /// <summary>
        /// Saldo final do dia
        /// </summary>
        public decimal ClosingBalance { get; set; }

        /// <summary>
        /// Número de transações de crédito
        /// </summary>
        public int CreditTransactionCount { get; set; }

        /// <summary>
        /// Número de transações de débito
        /// </summary>
        public int DebitTransactionCount { get; set; }

        /// <summary>
        /// Total de transações do dia
        /// </summary>
        public int TotalTransactionCount { get; set; }

        /// <summary>
        /// Data de última atualização
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
