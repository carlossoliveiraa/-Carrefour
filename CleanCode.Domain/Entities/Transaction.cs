using CleanCode.Domain.Common;
using CleanCode.Domain.Enum;

namespace CleanCode.Domain.Entities
{
    /// <summary>
    /// Entidade que representa uma transação no fluxo de caixa
    /// </summary>
    public class Transaction : BaseEntity
    {
        /// <summary>
        /// Descrição da transação
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação (sempre positivo)
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

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Transaction()
        {
            TransactionDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Construtor com parâmetros
        /// </summary>
        /// <param name="description">Descrição da transação</param>
        /// <param name="amount">Valor da transação</param>
        /// <param name="type">Tipo da transação</param>
        /// <param name="category">Categoria (opcional)</param>
        /// <param name="notes">Observações (opcional)</param>
        public Transaction(string description, decimal amount, TransactionType type, string? category = null, string? notes = null)
        {
            Description = description;
            Amount = amount;
            Type = type;
            Category = category;
            Notes = notes;
            TransactionDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Retorna o valor da transação com sinal baseado no tipo
        /// </summary>
        /// <returns>Valor com sinal (negativo para débito, positivo para crédito)</returns>
        public decimal GetSignedAmount()
        {
            return Type == TransactionType.Debit ? -Amount : Amount;
        }

        /// <summary>
        /// Verifica se a transação é válida
        /// </summary>
        /// <returns>True se a transação é válida</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Description) && 
                   Amount > 0 && 
                   Type != TransactionType.None;
        }
    }
}
