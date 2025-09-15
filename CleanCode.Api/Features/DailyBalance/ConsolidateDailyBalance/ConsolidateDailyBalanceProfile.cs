using AutoMapper;

namespace CleanCode.Api.Features.DailyBalance.ConsolidateDailyBalance
{
    /// <summary>
    /// Profile do AutoMapper para ConsolidateDailyBalance
    /// </summary>
    public class ConsolidateDailyBalanceProfile : Profile
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public ConsolidateDailyBalanceProfile()
        {
            CreateMap<ConsolidateDailyBalanceRequest, Application.DailyBalance.ConsolidateDailyBalance.ConsolidateDailyBalanceCommand>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));
            CreateMap<Application.DailyBalance.ConsolidateDailyBalance.ConsolidateDailyBalanceResult, ConsolidateDailyBalanceResponse>();
        }
    }
}
