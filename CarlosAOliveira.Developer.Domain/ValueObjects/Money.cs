namespace CarlosAOliveira.Developer.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing monetary amount
    /// </summary>
    public record Money
    {
        /// <summary>
        /// Amount value
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Currency code (default: BRL)
        /// </summary>
        public string Currency { get; }

        /// <summary>
        /// Initializes a new instance of Money
        /// </summary>
        /// <param name="amount">Amount value</param>
        /// <param name="currency">Currency code</param>
        /// <exception cref="ArgumentException">Thrown when amount is negative</exception>
        public Money(decimal amount, string currency = "BRL")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        /// <summary>
        /// Creates a Money instance from a decimal amount
        /// </summary>
        /// <param name="amount">Amount value</param>
        /// <returns>Money instance</returns>
        public static Money FromDecimal(decimal amount) => new(amount);

        /// <summary>
        /// Implicit conversion from decimal to Money
        /// </summary>
        /// <param name="amount">Amount value</param>
        /// <returns>Money instance</returns>
        public static implicit operator Money(decimal amount) => new(amount);

        /// <summary>
        /// Implicit conversion from Money to decimal
        /// </summary>
        /// <param name="money">Money instance</param>
        /// <returns>Amount value</returns>
        public static implicit operator decimal(Money money) => money.Amount;

        /// <summary>
        /// Adds two Money instances
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>Sum of the amounts</returns>
        /// <exception cref="InvalidOperationException">Thrown when currencies don't match</exception>
        public static Money operator +(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot add amounts with different currencies");

            return new Money(left.Amount + right.Amount, left.Currency);
        }

        /// <summary>
        /// Subtracts two Money instances
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>Difference of the amounts</returns>
        /// <exception cref="InvalidOperationException">Thrown when currencies don't match</exception>
        public static Money operator -(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot subtract amounts with different currencies");

            return new Money(left.Amount - right.Amount, left.Currency);
        }

        /// <summary>
        /// Multiplies Money by a factor
        /// </summary>
        /// <param name="money">Money instance</param>
        /// <param name="factor">Multiplication factor</param>
        /// <returns>Multiplied amount</returns>
        public static Money operator *(Money money, decimal factor) => new(money.Amount * factor, money.Currency);

        /// <summary>
        /// Divides Money by a factor
        /// </summary>
        /// <param name="money">Money instance</param>
        /// <param name="factor">Division factor</param>
        /// <returns>Divided amount</returns>
        /// <exception cref="ArgumentException">Thrown when factor is zero</exception>
        public static Money operator /(Money money, decimal factor)
        {
            if (factor == 0)
                throw new ArgumentException("Cannot divide by zero", nameof(factor));

            return new Money(money.Amount / factor, money.Currency);
        }

        /// <summary>
        /// Checks if amount is zero
        /// </summary>
        public bool IsZero => Amount == 0;

        /// <summary>
        /// Checks if amount is positive
        /// </summary>
        public bool IsPositive => Amount > 0;

        /// <summary>
        /// Checks if amount is negative
        /// </summary>
        public bool IsNegative => Amount < 0;

        /// <summary>
        /// Returns string representation of the money
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString() => $"{Amount:C} {Currency}";
    }
}
