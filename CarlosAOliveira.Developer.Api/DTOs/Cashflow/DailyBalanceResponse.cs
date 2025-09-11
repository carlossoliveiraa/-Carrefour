namespace CarlosAOliveira.Developer.Api.DTOs.Cashflow
{
    /// <summary>
    /// Response DTO for daily balance operations
    /// </summary>
    public class DailyBalanceResponse
    {
        /// <summary>
        /// Balance date
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Balance amount
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Last update timestamp
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
