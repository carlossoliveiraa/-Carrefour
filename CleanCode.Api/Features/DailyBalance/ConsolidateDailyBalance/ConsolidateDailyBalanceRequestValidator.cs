using FluentValidation;

namespace CleanCode.Api.Features.DailyBalance.ConsolidateDailyBalance
{
    /// <summary>
    /// Validador para ConsolidateDailyBalanceRequest
    /// </summary>
    public class ConsolidateDailyBalanceRequestValidator : AbstractValidator<ConsolidateDailyBalanceRequest>
    {
        /// <summary>
        /// Construtor com regras de validação
        /// </summary>
        public ConsolidateDailyBalanceRequestValidator()
        {
            RuleFor(request => request.Date)
                .NotEqual(default(DateTime))
                .WithMessage("A data para consolidação é obrigatória");
        }
    }
}
