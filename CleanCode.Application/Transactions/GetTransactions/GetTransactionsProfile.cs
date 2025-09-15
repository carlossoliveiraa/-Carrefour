using AutoMapper;
using CleanCode.Domain.Entities;

namespace CleanCode.Application.Transactions.GetTransactions
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
            CreateMap<Transaction, GetTransactionsResult.TransactionItem>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
