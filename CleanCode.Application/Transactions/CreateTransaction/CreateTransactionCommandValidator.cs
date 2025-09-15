using FluentValidation;

namespace CleanCode.Application.Transactions.CreateTransaction
{
    /// <summary>
    /// Validador para CreateTransactionCommand
    /// </summary>
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        /// <summary>
        /// Construtor com regras de validação
        /// </summary>
        public CreateTransactionCommandValidator()
        {
            RuleFor(command => command.Description)
                .NotEmpty()
                .WithMessage("A descrição da transação é obrigatória")
                .MaximumLength(200)
                .WithMessage("A descrição não pode ter mais de 200 caracteres");

            RuleFor(command => command.Amount)
                .GreaterThan(0)
                .WithMessage("O valor da transação deve ser maior que zero")
                .LessThan(1000000)
                .WithMessage("O valor da transação não pode ser maior que 1.000.000");

            RuleFor(command => command.Type)
                .NotEqual(Domain.Enum.TransactionType.None)
                .WithMessage("O tipo da transação deve ser Débito ou Crédito");

            RuleFor(command => command.TransactionDate)
                .NotEqual(default(DateTime))
                .WithMessage("A data da transação é obrigatória")
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                .WithMessage("A data da transação não pode ser no futuro");

            RuleFor(command => command.Category)
                .MaximumLength(100)
                .WithMessage("A categoria não pode ter mais de 100 caracteres")
                .When(command => !string.IsNullOrEmpty(command.Category));

            RuleFor(command => command.Notes)
                .MaximumLength(500)
                .WithMessage("As observações não podem ter mais de 500 caracteres")
                .When(command => !string.IsNullOrEmpty(command.Notes));
        }
    }
}
