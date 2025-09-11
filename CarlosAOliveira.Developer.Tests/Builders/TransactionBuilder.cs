using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Tests.Common;

namespace CarlosAOliveira.Developer.Tests.Builders
{
    /// <summary>
    /// Builder for creating test transaction instances
    /// </summary>
    public class TransactionBuilder : TestBase
    {
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Today);
        private decimal _amount = 0;
        private TransactionType _type = TransactionType.Credit;
        private string _category = string.Empty;
        private string _description = string.Empty;

        public TransactionBuilder()
        {
            _date = DateOnly.FromDateTime(DateTime.Today);
            _amount = RandomAmount();
            _type = TransactionType.Credit;
            _category = "Test Category";
            _description = RandomDescription();
        }

        public static TransactionBuilder Create() => new();

        public TransactionBuilder WithDate(DateOnly date)
        {
            _date = date;
            return this;
        }

        public TransactionBuilder WithCategory(string category)
        {
            _category = category;
            return this;
        }

        public TransactionBuilder WithAmount(decimal amount)
        {
            _amount = amount;
            return this;
        }

        public TransactionBuilder WithType(TransactionType type)
        {
            _type = type;
            return this;
        }

        public TransactionBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public TransactionBuilder WithRandomData()
        {
            _date = DateOnly.FromDateTime(DateTime.Today.AddDays(Random.Shared.Next(-30, 30)));
            _amount = RandomAmount();
            _type = Faker.PickRandom<TransactionType>();
            _category = Faker.Commerce.Categories(1).First();
            _description = RandomDescription();
            return this;
        }

        public TransactionBuilder AsCredit()
        {
            _type = TransactionType.Credit;
            return this;
        }

        public TransactionBuilder AsDebit()
        {
            _type = TransactionType.Debit;
            return this;
        }

        public Transaction Build()
        {
            return new Transaction(_date, _amount, _type, _category, _description);
        }
    }
}