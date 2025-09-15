using MediatR;

namespace CleanCode.Application.Auth.AuthenticateUser
{
    public class AuthenticateUserCommand : IRequest<AuthenticateUserResult>
    {       
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

}
