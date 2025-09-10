using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Tests.Common;

namespace CarlosAOliveira.Developer.Tests.Builders
{
    /// <summary>
    /// Builder for creating test merchant instances
    /// </summary>
    public class MerchantBuilder : TestBase
    {
        private string _name = string.Empty;
        private string _email = string.Empty;

        public MerchantBuilder()
        {
            _name = RandomName();
            _email = RandomEmail();
        }

        public static MerchantBuilder Create() => new();

        public MerchantBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public MerchantBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public MerchantBuilder WithRandomData()
        {
            _name = RandomName();
            _email = RandomEmail();
            return this;
        }

        public Merchant Build()
        {
            return new Merchant(_name, _email);
        }
    }
}
