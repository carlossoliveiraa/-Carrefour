using CarlosAOliveira.Developer.Application.Commands.Merchant;
using FluentValidation;

namespace CarlosAOliveira.Developer.Application.Validators
{
    public class CreateMerchantCommandValidator : AbstractValidator<CreateMerchantCommand>
    {
        public CreateMerchantCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        }
    }
}