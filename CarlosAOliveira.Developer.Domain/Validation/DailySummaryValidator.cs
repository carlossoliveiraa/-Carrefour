using FluentValidation;
using CarlosAOliveira.Developer.Domain.Entities;

namespace CarlosAOliveira.Developer.Domain.Validation
{
    /// <summary>
    /// Validator for daily summary entity using FluentValidation
    /// </summary>
    public class DailySummaryValidator : AbstractValidator<DailySummary>
    {
        public DailySummaryValidator()
        {
            RuleFor(x => x.MerchantId)
                .NotEmpty().WithMessage("Merchant ID is required");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Date cannot be in the future");

            RuleFor(x => x.TotalCredits)
                .GreaterThanOrEqualTo(0).WithMessage("Total credits cannot be negative");

            RuleFor(x => x.TotalDebits)
                .GreaterThanOrEqualTo(0).WithMessage("Total debits cannot be negative");

            RuleFor(x => x.TransactionCount)
                .GreaterThanOrEqualTo(0).WithMessage("Transaction count cannot be negative");
        }
    }
}