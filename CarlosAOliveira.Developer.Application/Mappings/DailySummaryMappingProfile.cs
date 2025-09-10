using AutoMapper;
using CarlosAOliveira.Developer.Application.DTOs.DailySummary;

namespace CarlosAOliveira.Developer.Application.Mappings
{
    public class DailySummaryMappingProfile : Profile
    {
        public DailySummaryMappingProfile()
        {
            CreateMap<Domain.Entities.DailySummary, DailySummaryDto>();
        }
    }
}