namespace CleanCode.Api.Features.Transactions.CreateTransaction
{
    /// <summary>
    /// Response da criação de uma transação
    /// </summary>
    public class CreateTransactionResponse
    {
        /// <summary>
        /// ID da transação criada
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Descrição da transação
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Tipo da transação
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
        /// Observações
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
