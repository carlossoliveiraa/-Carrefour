using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using FluentValidation;

namespace CarlosAOliveira.Developer.Domain.Validation
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .MaximumLength(100)
                .WithMessage("Email cannot be longer than 100 characters.")
                .EmailAddress()
                .WithMessage("The provided email address is not valid.");

            RuleFor(user => user.UserName)
                .NotEmpty()
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Username cannot be longer than 50 characters.");

            RuleFor(user => user.PhoneNumber)
                .Matches(@"^\+[1-9]\d{10,14}$")
                .WithMessage("Phone number must start with '+' followed by 11-15 digits.")
                .When(user => !string.IsNullOrEmpty(user.PhoneNumber));

            RuleFor(user => user.Status)
                .NotEqual(UserStatus.Unknown)
                .WithMessage("User status cannot be Unknown.");

            RuleFor(user => user.Role)
                .NotEqual(UserRole.None)
                .WithMessage("User role cannot be None.");
        }
    }

}
