using AutoMapper;
using CleanCode.Domain.Entities;

namespace CleanCode.Application.DailyBalance.GetDailyBalance
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
            CreateMap<CleanCode.Domain.Entities.DailyBalance, GetDailyBalanceResult>();
        }
    }
}
