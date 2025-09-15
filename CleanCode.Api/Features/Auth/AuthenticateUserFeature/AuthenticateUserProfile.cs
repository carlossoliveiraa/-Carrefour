using AutoMapper;
using CleanCode.Application.Auth.AuthenticateUser;
using CleanCode.Domain.Entities;

namespace CleanCode.Api.Features.Auth.AuthenticateUserFeature
{   
    public sealed class AuthenticateUserProfile : Profile
    {       
        public AuthenticateUserProfile()
        {
            CreateMap<AuthenticateUserRequest, AuthenticateUserCommand>();
            CreateMap<AuthenticateUserResult, AuthenticateUserResponse>();
            CreateMap<User, AuthenticateUserResponse>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        }
    }
}
