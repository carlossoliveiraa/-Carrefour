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
        private Guid _merchantId = Guid.Empty;
        private decimal _amount = 0;
        private TransactionType _type = TransactionType.Credit;
        private string _description = string.Empty;
        private TransactionStatus _status = TransactionStatus.Pending;

        public TransactionBuilder()
        {
            _merchantId = RandomGuid();
            _amount = RandomAmount();
            _type = TransactionType.Credit;
            _description = RandomDescription();
            _status = TransactionStatus.Pending;
        }

        public static TransactionBuilder Create() => new();

        public TransactionBuilder WithMerchantId(Guid merchantId)
        {
            _merchantId = merchantId;
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

        public TransactionBuilder WithStatus(TransactionStatus status)
        {
            _status = status;
            return this;
        }

        public TransactionBuilder WithRandomData()
        {
            _merchantId = RandomGuid();
            _amount = RandomAmount();
            _type = Faker.PickRandom<TransactionType>();
            _description = RandomDescription();
            _status = Faker.PickRandom<TransactionStatus>();
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
            return new Transaction(_merchantId, _amount, _type, _description);
        }
    }
}