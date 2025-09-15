namespace CleanCode.Api.Features.DailyBalance.GetDailyBalance
{
    /// <summary>
    /// Request para buscar saldo diário por data
    /// </summary>
    public class GetDailyBalanceRequest
    {
        /// <summary>
        /// Data do saldo
        /// </summary>
        public DateTime Date { get; set; }
    }
}
