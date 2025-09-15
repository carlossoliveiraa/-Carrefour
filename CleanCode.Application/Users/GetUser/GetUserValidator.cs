using FluentValidation;

namespace CleanCode.Application.Users.GetUser
{
    public class GetUserValidator : AbstractValidator<GetUserCommand>
    {         
        public GetUserValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("User ID is required");
        }
    }
}
