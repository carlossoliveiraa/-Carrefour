namespace CleanCode.Api.Features.Transactions.GetTransactions
{
    /// <summary>
    /// Response da busca de transações
    /// </summary>
    public class GetTransactionsResponse
    {
        /// <summary>
        /// Lista de transações
        /// </summary>
        public IEnumerable<TransactionItem> Transactions { get; set; } = new List<TransactionItem>();

        /// <summary>
        /// Página atual
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Total de páginas
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Total de itens
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Tamanho da página
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Item de transação
        /// </summary>
        public class TransactionItem
        {
            /// <summary>
            /// ID da transação
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
            /// Data de criação
            /// </summary>
            public DateTime CreatedAt { get; set; }
        }
    }
}
