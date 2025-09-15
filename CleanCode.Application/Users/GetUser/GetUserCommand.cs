using MediatR;

namespace CleanCode.Application.Users.GetUser
{
    public record GetUserCommand : IRequest<GetUserResult>
    {       
        public Guid Id { get; }
               
        public GetUserCommand(Guid id)
        {
            Id = id;
        }
    }
}
