using AutoMapper;

namespace CleanCode.Api.Features.DailyBalance.GetDailyBalance
{
    /// <summary>
    /// Profile do AutoMapper para GetDailyBalance
    /// </summary>
    public class GetDailyBalanceProfile : Profile
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public GetDailyBalanceProfile()
        {
            CreateMap<GetDailyBalanceRequest, Application.DailyBalance.GetDailyBalance.GetDailyBalanceCommand>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));
            CreateMap<Application.DailyBalance.GetDailyBalance.GetDailyBalanceResult, GetDailyBalanceResponse>();
        }
    }
}
