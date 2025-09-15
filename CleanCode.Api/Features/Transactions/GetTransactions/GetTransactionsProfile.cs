using AutoMapper;

namespace CleanCode.Api.Features.Transactions.GetTransactions
{
    /// <summary>
    /// Profile do AutoMapper para GetTransactions
    /// </summary>
    public class GetTransactionsProfile : Profile
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public GetTransactionsProfile()
        {
            CreateMap<GetTransactionsRequest, Application.Transactions.GetTransactions.GetTransactionsCommand>();
            CreateMap<Application.Transactions.GetTransactions.GetTransactionsResult, GetTransactionsResponse>();
            CreateMap<Application.Transactions.GetTransactions.GetTransactionsResult.TransactionItem, GetTransactionsResponse.TransactionItem>();
        }
    }
}
