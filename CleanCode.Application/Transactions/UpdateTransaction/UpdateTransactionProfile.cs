using AutoMapper;
using CleanCode.Domain.Entities;

namespace CleanCode.Application.Transactions.UpdateTransaction
{
    /// <summary>
    /// Profile do AutoMapper para UpdateTransaction
    /// </summary>
    public class UpdateTransactionProfile : Profile
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public UpdateTransactionProfile()
        {
            CreateMap<Transaction, UpdateTransactionResult>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
