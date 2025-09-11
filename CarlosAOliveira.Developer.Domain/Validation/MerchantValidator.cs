using FluentValidation;
using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Domain.Validation
{
    /// <summary>
    /// Validator for merchant entity using FluentValidation
    /// </summary>
    public class MerchantValidator : AbstractValidator<Merchant>
    {
        public MerchantValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Merchant name is required")
                .MaximumLength(100).WithMessage("Merchant name cannot exceed 100 characters");

            RuleFor(x => x.Email.Value)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
        }
    }
}