using MediatR;

namespace CleanCode.Application.Transactions.DeleteTransaction
{
    /// <summary>
    /// Command para deletar uma transação
    /// </summary>
    public class DeleteTransactionCommand : IRequest<DeleteTransactionResult>
    {
        /// <summary>
        /// ID da transação a ser deletada
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="id">ID da transação</param>
        public DeleteTransactionCommand(Guid id)
        {
            Id = id;
        }
    }
}
