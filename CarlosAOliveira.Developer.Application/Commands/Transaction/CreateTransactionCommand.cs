using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Transaction;
using CarlosAOliveira.Developer.Domain.Enums;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Commands.Transaction
{
    public class CreateTransactionCommand : IRequest<BaseResponse<TransactionDto>>
    {
        public Guid MerchantId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}