namespace CleanCode.Application.Transactions.DeleteTransaction
{
    /// <summary>
    /// Resultado da exclusão de uma transação
    /// </summary>
    public class DeleteTransactionResult
    {
        /// <summary>
        /// ID da transação deletada
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Indica se a transação foi deletada com sucesso
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem de resultado
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
