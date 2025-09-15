namespace CleanCode.Application.Transactions.UpdateTransaction
{
    /// <summary>
    /// Resultado da atualização de uma transação
    /// </summary>
    public class UpdateTransactionResult
    {
        /// <summary>
        /// ID da transação atualizada
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

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
