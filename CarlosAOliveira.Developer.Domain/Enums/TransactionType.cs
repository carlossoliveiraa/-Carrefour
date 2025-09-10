namespace CarlosAOliveira.Developer.Domain.Enums
{
    /// <summary>
    /// Represents the type of a financial transaction
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Credit transaction - money coming in
        /// </summary>
        Credit = 1,

        /// <summary>
        /// Debit transaction - money going out
        /// </summary>
        Debit = 2
    }
}
