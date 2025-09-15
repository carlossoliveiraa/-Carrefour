namespace CleanCode.Api.Features.Transactions.GetTransaction
{
    /// <summary>
    /// Request para buscar uma transação por ID
    /// </summary>
    public class GetTransactionRequest
    {
        /// <summary>
        /// ID da transação
        /// </summary>
        public Guid Id { get; set; }
    }
}
