using AutoMapper;
using CarlosAOliveira.Developer.Application.DTOs.Merchant;
using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Application.Mappings
{
    /// <summary>
    /// Merchant mapping profile
    /// </summary>
    public class MerchantMappingProfile : Profile
    {
        public MerchantMappingProfile()
        {
            CreateMap<Domain.Entities.Merchant, MerchantDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailString));

            CreateMap<CreateMerchantDto, Domain.Entities.Merchant>()
                .ConstructUsing(src => new Domain.Entities.Merchant(src.Name, src.Email));
        }
    }
}