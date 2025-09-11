using System.Text.RegularExpressions;

namespace CarlosAOliveira.Developer.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing an email address
    /// </summary>
    public record Email
    {
        /// <summary>
        /// Email address value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of Email
        /// </summary>
        /// <param name="value">Email address</param>
        /// <exception cref="ArgumentException">Thrown when email is invalid</exception>
        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be null or empty", nameof(value));

            if (!IsValidEmail(value))
                throw new ArgumentException("Invalid email format", nameof(value));

            Value = value.ToLowerInvariant().Trim();
        }

        /// <summary>
        /// Implicit conversion from string to Email
        /// </summary>
        /// <param name="value">Email string</param>
        /// <returns>Email instance</returns>
        public static implicit operator Email(string value) => new(value);

        /// <summary>
        /// Implicit conversion from Email to string
        /// </summary>
        /// <param name="email">Email instance</param>
        /// <returns>Email string</returns>
        public static implicit operator string(Email email) => email.Value;

        /// <summary>
        /// Validates email format
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            if (email.Length > 255)
                return false;

            // More strict email validation
            var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
            return regex.IsMatch(email);
        }

        /// <summary>
        /// Gets the domain part of the email
        /// </summary>
        public string Domain => Value.Split('@')[1];

        /// <summary>
        /// Gets the local part of the email
        /// </summary>
        public string LocalPart => Value.Split('@')[0];

        /// <summary>
        /// Returns string representation of the email
        /// </summary>
        /// <returns>Email string</returns>
        public override string ToString() => Value;
    }
}
