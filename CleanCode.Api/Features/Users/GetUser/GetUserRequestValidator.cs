using FluentValidation;

namespace CleanCode.Api.Features.Users.GetUser
{
    public class GetUserRequestValidator : AbstractValidator<GetUserRequest>
    {        
        public GetUserRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("User ID is required");
        }
    }
}
