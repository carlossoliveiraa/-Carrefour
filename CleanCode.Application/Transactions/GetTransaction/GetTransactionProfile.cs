using AutoMapper;
using CleanCode.Domain.Entities;

namespace CleanCode.Application.Transactions.GetTransaction
{
    /// <summary>
    /// Profile do AutoMapper para GetTransaction
    /// </summary>
    public class GetTransactionProfile : Profile
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public GetTransactionProfile()
        {
            CreateMap<Transaction, GetTransactionResult>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
