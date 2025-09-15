using AutoMapper;
using CleanCode.Domain.Entities;

namespace CleanCode.Application.Users.CreateUser
{
    public class CreateUserProfile : Profile
    {       
        public CreateUserProfile()
        {
            CreateMap<CreateUserCommand, User>();
            CreateMap<User, CreateUserResult>();
        }
    }
}
