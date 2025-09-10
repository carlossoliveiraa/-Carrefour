namespace CarlosAOliveira.Developer.Domain.Enums
{
    /// <summary>
    /// Represents the status of a financial transaction
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// Transaction is pending confirmation
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Transaction has been confirmed
        /// </summary>
        Confirmed = 2,

        /// <summary>
        /// Transaction has been cancelled
        /// </summary>
        Cancelled = 3
    }
}
