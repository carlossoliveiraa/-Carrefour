using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.User;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Commands.User
{
    /// <summary>
    /// Command to create a new user
    /// </summary>
    public class CreateUserCommand : IRequest<BaseResponse<UserDto>>
    {
        /// <summary>
        /// User username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// User phone (optional)
        /// </summary>
        public string? Phone { get; set; }
    }
}
