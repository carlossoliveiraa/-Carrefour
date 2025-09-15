using CleanCode.Domain.Enum;

namespace CleanCode.Api.Features.Transactions.CreateTransaction
{
    /// <summary>
    /// Request para criar uma nova transação
    /// </summary>
    public class CreateTransactionRequest
    {
        /// <summary>
        /// Descrição da transação
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Tipo da transação (Débito ou Crédito)
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        /// Data da transação
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Categoria da transação (opcional)
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Observações adicionais (opcional)
        /// </summary>
        public string? Notes { get; set; }
    }
}
