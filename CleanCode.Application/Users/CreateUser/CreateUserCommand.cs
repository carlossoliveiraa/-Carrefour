using CleanCode.Common.Validation;
using CleanCode.Domain.Enum;
using MediatR;

namespace CleanCode.Application.Users.CreateUser
{
    public class CreateUserCommand : IRequest<CreateUserResult>
    {
        /// <summary>
        /// Gets or sets the username of the user to be created.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for the user.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the phone number for the user.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address for the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status of the user.
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the role of the user.
        /// </summary>
        public UserRole Role { get; set; }


        public ValidationResultDetail Validate()
        {
            var validator = new CreateUserCommandValidator();
            var result = validator.Validate(this);
            return new ValidationResultDetail
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
            };
        }
    }
}
