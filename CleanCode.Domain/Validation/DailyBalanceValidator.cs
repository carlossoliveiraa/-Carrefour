using CleanCode.Domain.Entities;
using FluentValidation;

namespace CleanCode.Domain.Validation
{
    /// <summary>
    /// Validador para entidade DailyBalance
    /// </summary>
    public class DailyBalanceValidator : AbstractValidator<DailyBalance>
    {
        /// <summary>
        /// Construtor com regras de validação
        /// </summary>
        public DailyBalanceValidator()
        {
            RuleFor(balance => balance.Date)
                .NotEqual(default(DateTime))
                .WithMessage("A data do saldo é obrigatória");

            RuleFor(balance => balance.OpeningBalance)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O saldo inicial não pode ser negativo");

            RuleFor(balance => balance.TotalCredits)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O total de créditos não pode ser negativo");

            RuleFor(balance => balance.TotalDebits)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O total de débitos não pode ser negativo");

            RuleFor(balance => balance.CreditTransactionCount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O número de transações de crédito não pode ser negativo");

            RuleFor(balance => balance.DebitTransactionCount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O número de transações de débito não pode ser negativo");

            RuleFor(balance => balance.TotalTransactionCount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O número total de transações não pode ser negativo");

            RuleFor(balance => balance)
                .Must(HaveConsistentTransactionCount)
                .WithMessage("O número total de transações deve ser igual à soma de créditos e débitos");

            RuleFor(balance => balance)
                .Must(HaveConsistentClosingBalance)
                .WithMessage("O saldo final deve ser igual ao saldo inicial + créditos - débitos");
        }

        /// <summary>
        /// Verifica se o número de transações está consistente
        /// </summary>
        /// <param name="balance">Saldo diário</param>
        /// <returns>True se consistente</returns>
        private static bool HaveConsistentTransactionCount(DailyBalance balance)
        {
            return balance.TotalTransactionCount == balance.CreditTransactionCount + balance.DebitTransactionCount;
        }

        /// <summary>
        /// Verifica se o saldo final está consistente
        /// </summary>
        /// <param name="balance">Saldo diário</param>
        /// <returns>True se consistente</returns>
        private static bool HaveConsistentClosingBalance(DailyBalance balance)
        {
            var expectedClosingBalance = balance.OpeningBalance + balance.TotalCredits - balance.TotalDebits;
            return Math.Abs(balance.ClosingBalance - expectedClosingBalance) < 0.01m; // Tolerância para arredondamento
        }
    }
}
