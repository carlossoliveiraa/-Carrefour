using FluentValidation;

namespace CleanCode.Api.Features.Transactions.CreateTransaction
{
    /// <summary>
    /// Validador para CreateTransactionRequest
    /// </summary>
    public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
    {
        /// <summary>
        /// Construtor com regras de validação
        /// </summary>
        public CreateTransactionRequestValidator()
        {
            RuleFor(request => request.Description)
                .NotEmpty()
                .WithMessage("A descrição da transação é obrigatória")
                .MaximumLength(200)
                .WithMessage("A descrição não pode ter mais de 200 caracteres");

            RuleFor(request => request.Amount)
                .GreaterThan(0)
                .WithMessage("O valor da transação deve ser maior que zero")
                .LessThan(1000000)
                .WithMessage("O valor da transação não pode ser maior que 1.000.000");

            RuleFor(request => request.Type)
                .NotEqual(Domain.Enum.TransactionType.None)
                .WithMessage("O tipo da transação deve ser Débito ou Crédito");

            RuleFor(request => request.TransactionDate)
                .NotEqual(default(DateTime))
                .WithMessage("A data da transação é obrigatória")
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                .WithMessage("A data da transação não pode ser no futuro");

            RuleFor(request => request.Category)
                .MaximumLength(100)
                .WithMessage("A categoria não pode ter mais de 100 caracteres")
                .When(request => !string.IsNullOrEmpty(request.Category));

            RuleFor(request => request.Notes)
                .MaximumLength(500)
                .WithMessage("As observações não podem ter mais de 500 caracteres")
                .When(request => !string.IsNullOrEmpty(request.Notes));
        }
    }
}
