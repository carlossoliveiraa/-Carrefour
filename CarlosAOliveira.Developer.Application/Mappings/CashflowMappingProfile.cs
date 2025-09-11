using AutoMapper;
using CarlosAOliveira.Developer.Application.DTOs.Cashflow;
using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Application.Mappings
{
    /// <summary>
    /// Cashflow mapping profile
    /// </summary>
    public class CashflowMappingProfile : Profile
    {
        public CashflowMappingProfile()
        {
            CreateMap<Transaction, TransactionResponse>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<DailyBalance, DailyBalanceResponse>();
        }
    }
}
