using MediatR;

namespace CleanCode.Application.Transactions.GetTransaction
{
    /// <summary>
    /// Command para buscar uma transação por ID
    /// </summary>
    public class GetTransactionCommand : IRequest<GetTransactionResult>
    {
        /// <summary>
        /// ID da transação
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="id">ID da transação</param>
        public GetTransactionCommand(Guid id)
        {
            Id = id;
        }
    }
}
