using CarlosAOliveira.Developer.Domain.Common;
using CarlosAOliveira.Developer.Domain.ValueObjects;

namespace CarlosAOliveira.Developer.Domain.Entities
{
    /// <summary>
    /// Represents a merchant in the cash flow system
    /// </summary>
    public class Merchant : BaseEntity
    {
        /// <summary>
        /// Merchant name
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Merchant email
        /// </summary>
        public Email Email { get; private set; }

        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        private Merchant() 
        { 
            Email = new Email("default@example.com");
        } // EF Core constructor

        /// <summary>
        /// Initializes a new instance of Merchant
        /// </summary>
        /// <param name="name">Merchant name</param>
        /// <param name="email">Merchant email</param>
        public Merchant(string name, string email)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = new Email(email);
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates merchant information
        /// </summary>
        /// <param name="name">New name</param>
        /// <param name="email">New email</param>
        public void UpdateInformation(string name, string email)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = new Email(email);
        }

        /// <summary>
        /// Gets the email as string
        /// </summary>
        public string EmailString => Email.Value;
    }
}