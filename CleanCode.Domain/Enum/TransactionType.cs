namespace CleanCode.Domain.Enum
{
    /// <summary>
    /// Tipos de transação no fluxo de caixa
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Tipo não definido
        /// </summary>
        None = 0,

        /// <summary>
        /// Débito - saída de dinheiro
        /// </summary>
        Debit = 1,

        /// <summary>
        /// Crédito - entrada de dinheiro
        /// </summary>
        Credit = 2
    }
}
