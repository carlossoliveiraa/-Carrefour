using MediatR;

namespace CleanCode.Application.DailyBalance.GetDailyBalance
{
    /// <summary>
    /// Command para buscar saldo diário por data
    /// </summary>
    public class GetDailyBalanceCommand : IRequest<GetDailyBalanceResult>
    {
        /// <summary>
        /// Data do saldo
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="date">Data do saldo</param>
        public GetDailyBalanceCommand(DateTime date)
        {
            Date = date.Date;
        }
    }
}
