using AutoMapper;
using CarlosAOliveira.Developer.Application.DTOs.User;
using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Application.Mappings
{
    /// <summary>
    /// User mapping profile
    /// </summary>
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<Domain.Entities.User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
