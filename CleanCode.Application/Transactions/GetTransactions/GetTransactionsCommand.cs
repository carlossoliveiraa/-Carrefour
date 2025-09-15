using MediatR;

namespace CleanCode.Application.Transactions.GetTransactions
{
    /// <summary>
    /// Command para buscar transações com filtros
    /// </summary>
    public class GetTransactionsCommand : IRequest<GetTransactionsResult>
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
        public Domain.Enum.TransactionType? Type { get; set; }

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

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public GetTransactionsCommand()
        {
        }

        /// <summary>
        /// Construtor com parâmetros
        /// </summary>
        /// <param name="startDate">Data inicial</param>
        /// <param name="endDate">Data final</param>
        /// <param name="type">Tipo da transação</param>
        /// <param name="category">Categoria</param>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        public GetTransactionsCommand(
            DateTime? startDate = null,
            DateTime? endDate = null,
            Domain.Enum.TransactionType? type = null,
            string? category = null,
            int page = 1,
            int pageSize = 10)
        {
            StartDate = startDate;
            EndDate = endDate;
            Type = type;
            Category = category;
            Page = page;
            PageSize = pageSize;
        }
    }
}
