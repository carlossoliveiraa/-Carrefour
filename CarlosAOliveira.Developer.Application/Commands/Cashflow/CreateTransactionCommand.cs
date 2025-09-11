using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Cashflow;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Commands.Cashflow
{
    /// <summary>
    /// Command to create a new transaction
    /// </summary>
    public class CreateTransactionCommand : IRequest<BaseResponse<TransactionResponse>>
    {
        /// <summary>
        /// Transaction date
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Transaction amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Transaction category
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Transaction description
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
