using CarlosAOliveira.Developer.Application.Commands.Cashflow;
using FluentValidation;

namespace CarlosAOliveira.Developer.Application.Validators
{   
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {
            RuleFor(x => x.Date).NotEmpty().WithMessage("Date is required");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero");
            RuleFor(x => x.Type).NotEmpty().WithMessage("Transaction type is required");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required").MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required").MaximumLength(500);
        }
    }
}