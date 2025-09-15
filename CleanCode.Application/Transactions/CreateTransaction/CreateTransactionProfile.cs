using AutoMapper;
using CleanCode.Domain.Entities;

namespace CleanCode.Application.Transactions.CreateTransaction
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
            CreateMap<CreateTransactionCommand, Transaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Transaction, CreateTransactionResult>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
