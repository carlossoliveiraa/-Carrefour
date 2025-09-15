using MediatR;

namespace CleanCode.Application.DailyBalance.ConsolidateDailyBalance
{
    /// <summary>
    /// Command para consolidar o saldo diário
    /// </summary>
    public class ConsolidateDailyBalanceCommand : IRequest<ConsolidateDailyBalanceResult>
    {
        /// <summary>
        /// Data para consolidar
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="date">Data para consolidar</param>
        public ConsolidateDailyBalanceCommand(DateTime date)
        {
            Date = date.Date;
        }
    }
}
