using Bogus;

namespace CarlosAOliveira.Developer.Tests.Common
{
    /// <summary>
    /// Base class for all unit tests with common setup
    /// </summary>
    public abstract class TestBase
    {
        protected readonly Faker Faker;

        protected TestBase()
        {
            Faker = new Faker("pt_BR");
        }

        /// <summary>
        /// Generates a random GUID
        /// </summary>
        protected Guid RandomGuid() => Faker.Random.Guid();

        /// <summary>
        /// Generates a random decimal amount
        /// </summary>
        protected decimal RandomAmount(decimal min = 1, decimal max = 10000) => 
            Faker.Random.Decimal(min, max);

        /// <summary>
        /// Generates a random date
        /// </summary>
        protected DateTime RandomDate(DateTime? start = null, DateTime? end = null) => 
            Faker.Date.Between(start ?? DateTime.Today.AddDays(-30), end ?? DateTime.Today);

        /// <summary>
        /// Generates a random email
        /// </summary>
        protected string RandomEmail() => Faker.Person.Email;

        /// <summary>
        /// Generates a random name
        /// </summary>
        protected string RandomName() => Faker.Person.FullName;

        /// <summary>
        /// Generates a random description
        /// </summary>
        protected string RandomDescription() => Faker.Lorem.Sentence();
    }
}
