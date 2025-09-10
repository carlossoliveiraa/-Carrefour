using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Transaction;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Queries.Transaction
{
    /// <summary>
    /// Query to get a transaction by ID
    /// </summary>
    public class GetTransactionByIdQuery : IRequest<BaseResponse<TransactionDto>>
    {
        public Guid Id { get; set; }

        public GetTransactionByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
