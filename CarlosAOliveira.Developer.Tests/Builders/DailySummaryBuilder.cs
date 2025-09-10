using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Tests.Common;

namespace CarlosAOliveira.Developer.Tests.Builders
{
    /// <summary>
    /// Builder for creating test daily summary instances
    /// </summary>
    public class DailySummaryBuilder : TestBase
    {
        private Guid _merchantId = Guid.Empty;
        private DateTime _date = DateTime.Today;
        private decimal _netAmount = 0;
        private int _transactionCount = 0;

        public DailySummaryBuilder()
        {
            _merchantId = RandomGuid();
            _date = RandomDate();
            _netAmount = RandomAmount(-1000, 1000);
            _transactionCount = Faker.Random.Int(1, 100);
        }

        public static DailySummaryBuilder Create() => new();

        public DailySummaryBuilder WithMerchantId(Guid merchantId)
        {
            _merchantId = merchantId;
            return this;
        }

        public DailySummaryBuilder WithDate(DateTime date)
        {
            _date = date;
            return this;
        }

        public DailySummaryBuilder WithNetAmount(decimal netAmount)
        {
            _netAmount = netAmount;
            return this;
        }

        public DailySummaryBuilder WithTransactionCount(int transactionCount)
        {
            _transactionCount = transactionCount;
            return this;
        }

        public DailySummaryBuilder WithRandomData()
        {
            _merchantId = RandomGuid();
            _date = RandomDate();
            _netAmount = RandomAmount(-1000, 1000);
            _transactionCount = Faker.Random.Int(1, 100);
            return this;
        }

        public DailySummary Build()
        {
            return new DailySummary(_merchantId, _date, _netAmount, _transactionCount);
        }
    }
}