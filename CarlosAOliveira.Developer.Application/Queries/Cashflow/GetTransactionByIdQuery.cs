using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Cashflow;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Queries.Cashflow
{
    /// <summary>
    /// Query to get a transaction by ID
    /// </summary>
    public class GetTransactionByIdQuery : IRequest<BaseResponse<TransactionResponse>>
    {
        /// <summary>
        /// Transaction ID
        /// </summary>
        public Guid Id { get; set; }
    }
}
