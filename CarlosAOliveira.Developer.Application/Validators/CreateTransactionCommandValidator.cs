using CarlosAOliveira.Developer.Application.Commands.Transaction;
using FluentValidation;

namespace CarlosAOliveira.Developer.Application.Validators
{   
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {
            RuleFor(x => x.MerchantId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Type).IsInEnum();
        }
    }
}