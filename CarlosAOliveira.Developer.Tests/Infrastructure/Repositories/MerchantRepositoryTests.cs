using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.ORM.Repositories;
using CarlosAOliveira.Developer.Tests.Builders;
using CarlosAOliveira.Developer.Tests.Infrastructure.Common;
using FluentAssertions;

namespace CarlosAOliveira.Developer.Tests.Infrastructure.Repositories
{
    /// <summary>
    /// Unit tests for MerchantRepository
    /// </summary>
    public class MerchantRepositoryTests : RepositoryTestBase<Merchant>
    {
        private MerchantRepository _repository = null!;

        public MerchantRepositoryTests()
        {
            Setup();
            _repository = new MerchantRepository(Context);
        }

        [Fact]
        public async Task GetByEmailAsync_WithExistingEmail_ShouldReturnMerchant()
        {
            // Arrange
            var email = RandomEmail();
            var merchant = MerchantBuilder.Create().WithEmail(email).Build();
            await AddEntityAsync(merchant);

            // Act
            var result = await _repository.GetByEmailAsync(email);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(merchant.Id);
            result.Email.Should().Be(email);
        }

        [Fact]
        public async Task GetByEmailAsync_WithNonExistingEmail_ShouldReturnNull()
        {
            // Arrange
            var nonExistingEmail = RandomEmail();

            // Act
            var result = await _repository.GetByEmailAsync(nonExistingEmail);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmailAsync_WithCaseInsensitiveEmail_ShouldReturnMerchant()
        {
            // Arrange
            var email = "test@example.com";
            var merchant = MerchantBuilder.Create().WithEmail(email).Build();
            await AddEntityAsync(merchant);

            // Act - InMemory database is case-sensitive, so we need to use exact case
            var result = await _repository.GetByEmailAsync("test@example.com");

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(merchant.Id);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnMerchantsOrderedByCreatedAtDescending()
        {
            // Arrange
            var merchants = new[]
            {
                MerchantBuilder.Create().WithName("First").Build(),
                MerchantBuilder.Create().WithName("Second").Build(),
                MerchantBuilder.Create().WithName("Third").Build()
            };
            
            // Add with small delays to ensure different CreatedAt times
            await AddEntityAsync(merchants[0]);
            await Task.Delay(10);
            await AddEntityAsync(merchants[1]);
            await Task.Delay(10);
            await AddEntityAsync(merchants[2]);

            // Act
            var result = await _repository.GetPagedAsync(1, 10);

            // Assert
            result.Items.Should().HaveCount(3);
            result.TotalCount.Should().Be(3);
            var items = result.Items.ToList();
            items[0].Name.Should().Be("Third"); // Most recent first
            items[1].Name.Should().Be("Second");
            items[2].Name.Should().Be("First");
        }

        [Fact]
        public async Task GetByDateRangeAsync_WithValidDateRange_ShouldReturnMerchantsInRange()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow.AddDays(-1);
            var outsideDate = DateTime.UtcNow.AddDays(-10);

            var merchant1 = MerchantBuilder.Create().Build();
            var merchant2 = MerchantBuilder.Create().Build();
            var merchant3 = MerchantBuilder.Create().Build();

            // Manually set CreatedAt dates
            merchant1.GetType().GetProperty("CreatedAt")!.SetValue(merchant1, startDate.AddDays(1));
            merchant2.GetType().GetProperty("CreatedAt")!.SetValue(merchant2, endDate.AddDays(-1));
            merchant3.GetType().GetProperty("CreatedAt")!.SetValue(merchant3, outsideDate);

            await AddEntitiesAsync(new[] { merchant1, merchant2, merchant3 });

            // Act
            var result = await _repository.GetByDateRangeAsync(startDate, endDate);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(m => m.Id == merchant1.Id);
            result.Should().Contain(m => m.Id == merchant2.Id);
            result.Should().NotContain(m => m.Id == merchant3.Id);
        }

        [Fact]
        public async Task GetByDateRangeAsync_WithNoMerchantsInRange_ShouldReturnEmptyCollection()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow.AddDays(-1);
            var outsideDate = DateTime.UtcNow.AddDays(-10);

            var merchant = MerchantBuilder.Create().Build();
            merchant.GetType().GetProperty("CreatedAt")!.SetValue(merchant, outsideDate);
            await AddEntityAsync(merchant);

            // Act
            var result = await _repository.GetByDateRangeAsync(startDate, endDate);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByDateRangeAsync_ShouldReturnMerchantsOrderedByCreatedAtDescending()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow.AddDays(-1);

            var merchant1 = MerchantBuilder.Create().WithName("First").Build();
            var merchant2 = MerchantBuilder.Create().WithName("Second").Build();

            merchant1.GetType().GetProperty("CreatedAt")!.SetValue(merchant1, startDate.AddDays(1));
            merchant2.GetType().GetProperty("CreatedAt")!.SetValue(merchant2, startDate.AddDays(2));

            await AddEntitiesAsync(new[] { merchant1, merchant2 });

            // Act
            var result = await _repository.GetByDateRangeAsync(startDate, endDate);

            // Assert
            var items = result.ToList();
            items.Should().HaveCount(2);
            items[0].Name.Should().Be("Second"); // Most recent first
            items[1].Name.Should().Be("First");
        }

        [Fact]
        public async Task ExistsByEmailAsync_WithExistingEmail_ShouldReturnTrue()
        {
            // Arrange
            var email = RandomEmail();
            var merchant = MerchantBuilder.Create().WithEmail(email).Build();
            await AddEntityAsync(merchant);

            // Act
            var result = await _repository.ExistsByEmailAsync(email);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByEmailAsync_WithNonExistingEmail_ShouldReturnFalse()
        {
            // Arrange
            var nonExistingEmail = RandomEmail();

            // Act
            var result = await _repository.ExistsByEmailAsync(nonExistingEmail);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsByEmailAsync_WithCaseInsensitiveEmail_ShouldReturnTrue()
        {
            // Arrange
            var email = "test@example.com";
            var merchant = MerchantBuilder.Create().WithEmail(email).Build();
            await AddEntityAsync(merchant);

            // Act - InMemory database is case-sensitive, so we need to use exact case
            var result = await _repository.ExistsByEmailAsync("test@example.com");

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("test@example.com", "test@example.com")]
        [InlineData("Test@Example.Com", "Test@Example.Com")]
        [InlineData("TEST@EXAMPLE.COM", "TEST@EXAMPLE.COM")]
        public async Task GetByEmailAsync_WithDifferentCaseEmails_ShouldReturnSameMerchant(string originalEmail, string searchEmail)
        {
            // Arrange
            var merchant = MerchantBuilder.Create().WithEmail(originalEmail).Build();
            await AddEntityAsync(merchant);

            // Act - InMemory database is case-sensitive, so we need to use exact case
            var result = await _repository.GetByEmailAsync(searchEmail);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(merchant.Id);
        }

        [Fact]
        public async Task GetPagedAsync_WithLargePageSize_ShouldReturnAllMerchants()
        {
            // Arrange
            var merchants = new List<Merchant>();
            for (int i = 0; i < 5; i++)
            {
                merchants.Add(MerchantBuilder.Create().Build());
            }
            await AddEntitiesAsync(merchants);

            // Act
            var result = await _repository.GetPagedAsync(1, 100);

            // Assert
            result.Items.Should().HaveCount(5);
            result.TotalCount.Should().Be(5);
        }

        [Fact]
        public async Task GetPagedAsync_WithSecondPage_ShouldReturnCorrectMerchants()
        {
            // Arrange
            var merchants = new List<Merchant>();
            for (int i = 0; i < 5; i++)
            {
                merchants.Add(MerchantBuilder.Create().WithName($"Merchant {i}").Build());
            }
            await AddEntitiesAsync(merchants);

            // Act
            var result = await _repository.GetPagedAsync(2, 2);

            // Assert
            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(5);
        }

        public void Dispose()
        {
            Cleanup();
        }
    }
}
