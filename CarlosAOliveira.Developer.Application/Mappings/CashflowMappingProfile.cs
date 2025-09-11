using AutoMapper;
using CarlosAOliveira.Developer.Application.Commands.Cashflow;
using CarlosAOliveira.Developer.Application.DTOs.Cashflow;
using CarlosAOliveira.Developer.Application.DTOs.DailySummary;
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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount));

            CreateMap<DailyBalance, DailyBalanceResponse>();

            CreateMap<DailySummary, DailySummaryDto>()
                .ForMember(dest => dest.TotalCredits, opt => opt.MapFrom(src => src.TotalCreditsDecimal))
                .ForMember(dest => dest.TotalDebits, opt => opt.MapFrom(src => src.TotalDebitsDecimal))
                .ForMember(dest => dest.NetAmount, opt => opt.MapFrom(src => src.NetAmountDecimal));

            CreateMap<DailySummaryDto, DailySummaryResponse>();

            CreateMap<CreateCashflowTransactionRequest, CreateTransactionCommand>();
        }
    }
}
