using AutoMapper;
using CleanCode.Domain.Entities;

namespace CleanCode.Application.DailyBalance.ConsolidateDailyBalance
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
            CreateMap<CleanCode.Domain.Entities.DailyBalance, ConsolidateDailyBalanceResult>();
        }
    }
}
