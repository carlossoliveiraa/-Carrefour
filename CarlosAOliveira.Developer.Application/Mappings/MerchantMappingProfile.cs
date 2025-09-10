using AutoMapper;
using CarlosAOliveira.Developer.Application.DTOs.Merchant;
using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Application.Mappings
{
    public class MerchantMappingProfile : Profile
    {
        public MerchantMappingProfile()
        {
            CreateMap<Domain.Entities.Merchant, MerchantDto>();
            CreateMap<CreateMerchantDto, Domain.Entities.Merchant>()
                .ConstructUsing(src => new Domain.Entities.Merchant(src.Name, src.Email));
        }
    }
}