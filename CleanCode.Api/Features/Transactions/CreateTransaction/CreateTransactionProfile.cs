using AutoMapper;

namespace CleanCode.Api.Features.Transactions.CreateTransaction
{
    /// <summary>
    /// Profile do AutoMapper para CreateTransaction
    /// </summary>
    public class CreateTransactionProfile : Profile
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public CreateTransactionProfile()
        {
            CreateMap<CreateTransactionRequest, Application.Transactions.CreateTransaction.CreateTransactionCommand>();
            CreateMap<Application.Transactions.CreateTransaction.CreateTransactionResult, CreateTransactionResponse>();
        }
    }
}
