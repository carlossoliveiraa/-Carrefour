using FluentValidation;

namespace CleanCode.Api.Features.DailyBalance.GetDailyBalance
{
    /// <summary>
    /// Validador para GetDailyBalanceRequest
    /// </summary>
    public class GetDailyBalanceRequestValidator : AbstractValidator<GetDailyBalanceRequest>
    {
        /// <summary>
        /// Construtor com regras de validação
        /// </summary>
        public GetDailyBalanceRequestValidator()
        {
            RuleFor(request => request.Date)
                .NotEqual(default(DateTime))
                .WithMessage("A data do saldo é obrigatória");
        }
    }
}
