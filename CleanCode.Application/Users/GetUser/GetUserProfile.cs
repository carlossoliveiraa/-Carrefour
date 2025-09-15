using AutoMapper;
using CleanCode.Domain.Entities;

namespace CleanCode.Application.Users.GetUser
{
    public class GetUserProfile : Profile
    {        
        public GetUserProfile()
        {
            CreateMap<User, GetUserResult>();
        }
    }
}
