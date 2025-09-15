using FluentValidation;

namespace CleanCode.Api.Features.Transactions.GetTransactions
{
    /// <summary>
    /// Validador para GetTransactionsRequest
    /// </summary>
    public class GetTransactionsRequestValidator : AbstractValidator<GetTransactionsRequest>
    {
        /// <summary>
        /// Construtor com regras de validação
        /// </summary>
        public GetTransactionsRequestValidator()
        {
            RuleFor(request => request.StartDate)
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                .WithMessage("A data inicial não pode ser no futuro")
                .When(request => request.StartDate.HasValue);

            RuleFor(request => request.EndDate)
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                .WithMessage("A data final não pode ser no futuro")
                .When(request => request.EndDate.HasValue);

            RuleFor(request => request.StartDate)
                .LessThanOrEqualTo(request => request.EndDate)
                .WithMessage("A data inicial deve ser menor ou igual à data final")
                .When(request => request.StartDate.HasValue && request.EndDate.HasValue);

            RuleFor(request => request.Page)
                .GreaterThan(0)
                .WithMessage("A página deve ser maior que zero");

            RuleFor(request => request.PageSize)
                .GreaterThan(0)
                .WithMessage("O tamanho da página deve ser maior que zero")
                .LessThanOrEqualTo(100)
                .WithMessage("O tamanho da página não pode ser maior que 100");

            RuleFor(request => request.Category)
                .MaximumLength(100)
                .WithMessage("A categoria não pode ter mais de 100 caracteres")
                .When(request => !string.IsNullOrEmpty(request.Category));
        }
    }
}
