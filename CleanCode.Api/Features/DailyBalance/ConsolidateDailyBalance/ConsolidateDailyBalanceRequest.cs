namespace CleanCode.Api.Features.DailyBalance.ConsolidateDailyBalance
{
    /// <summary>
    /// Request para consolidar saldo diário
    /// </summary>
    public class ConsolidateDailyBalanceRequest
    {
        /// <summary>
        /// Data para consolidar
        /// </summary>
        public DateTime Date { get; set; }
    }
}
