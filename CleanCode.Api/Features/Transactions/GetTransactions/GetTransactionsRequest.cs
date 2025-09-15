using CleanCode.Domain.Enum;

namespace CleanCode.Api.Features.Transactions.GetTransactions
{
    /// <summary>
    /// Request para buscar transações com filtros
    /// </summary>
    public class GetTransactionsRequest
    {
        /// <summary>
        /// Data inicial (opcional)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Data final (opcional)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Tipo da transação (opcional)
        /// </summary>
        public TransactionType? Type { get; set; }

        /// <summary>
        /// Categoria (opcional)
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Página atual (padrão: 1)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Tamanho da página (padrão: 10)
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
