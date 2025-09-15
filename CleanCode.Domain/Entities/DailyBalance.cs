using CleanCode.Domain.Common;

namespace CleanCode.Domain.Entities
{
    /// <summary>
    /// Entidade que representa o saldo consolidado diário
    /// </summary>
    public class DailyBalance : BaseEntity
    {
        /// <summary>
        /// Data do saldo consolidado
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

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public DailyBalance()
        {
            Date = DateTime.UtcNow.Date;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Construtor com parâmetros
        /// </summary>
        /// <param name="date">Data do saldo</param>
        /// <param name="openingBalance">Saldo inicial</param>
        public DailyBalance(DateTime date, decimal openingBalance = 0)
        {
            Date = date.Date;
            OpeningBalance = openingBalance;
            ClosingBalance = openingBalance;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Calcula o saldo final baseado nos totais
        /// </summary>
        public void CalculateClosingBalance()
        {
            ClosingBalance = OpeningBalance + TotalCredits - TotalDebits;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Adiciona uma transação de crédito
        /// </summary>
        /// <param name="amount">Valor do crédito</param>
        public void AddCredit(decimal amount)
        {
            TotalCredits += amount;
            CreditTransactionCount++;
            TotalTransactionCount++;
            CalculateClosingBalance();
        }

        /// <summary>
        /// Adiciona uma transação de débito
        /// </summary>
        /// <param name="amount">Valor do débito</param>
        public void AddDebit(decimal amount)
        {
            TotalDebits += amount;
            DebitTransactionCount++;
            TotalTransactionCount++;
            CalculateClosingBalance();
        }

        /// <summary>
        /// Verifica se o saldo diário é válido
        /// </summary>
        /// <returns>True se o saldo é válido</returns>
        public bool IsValid()
        {
            return Date != default &&
                   TotalCredits >= 0 &&
                   TotalDebits >= 0 &&
                   CreditTransactionCount >= 0 &&
                   DebitTransactionCount >= 0 &&
                   TotalTransactionCount >= 0;
        }
    }
}
