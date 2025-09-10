using FluentValidation;
using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Domain.Validation
{
    /// <summary>
    /// Validator for transaction entity using FluentValidation
    /// </summary>
    public class TransactionValidator : AbstractValidator<Transaction>
    {
        public TransactionValidator()
        {
            RuleFor(x => x.MerchantId)
                .NotEmpty().WithMessage("Merchant ID is required");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}