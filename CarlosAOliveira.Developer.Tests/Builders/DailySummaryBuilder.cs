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

        public DailySummaryBuilder()
        {
            _merchantId = RandomGuid();
            _date = RandomDate();
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

        public DailySummaryBuilder WithRandomData()
        {
            _merchantId = RandomGuid();
            _date = RandomDate();
            return this;
        }

        public DailySummary Build()
        {
            return new DailySummary(_merchantId, _date);
        }
    }
}
