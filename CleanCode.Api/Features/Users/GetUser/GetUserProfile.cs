using AutoMapper;
using CleanCode.Application.Users.GetUser;

namespace CleanCode.Api.Features.Users.GetUser
{
    public class GetUserProfile : Profile
    {       
        public GetUserProfile()
        {
            CreateMap<Guid, GetUserCommand>()
                .ConstructUsing(id => new GetUserCommand(id));
            CreateMap<GetUserResult, GetUserResponse>();
        }
    }
}
