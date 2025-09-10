using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.ORM.Repositories;
using CarlosAOliveira.Developer.Tests.Builders;
using CarlosAOliveira.Developer.Tests.Infrastructure.Common;
using FluentAssertions;

namespace CarlosAOliveira.Developer.Tests.Infrastructure.Repositories
{
    /// <summary>
    /// Unit tests for DailySummaryRepository
    /// </summary>
    public class DailySummaryRepositoryTests : RepositoryTestBase<DailySummary>
    {
        private DailySummaryRepository _repository = null!;
        private Merchant _merchant = null!;

        public DailySummaryRepositoryTests()
        {
            Setup();
            _repository = new DailySummaryRepository(Context);
            _merchant = MerchantBuilder.Create().Build();
            Context.Merchants.Add(_merchant);
            Context.SaveChanges();
        }

        [Fact]
        public async Task GetByMerchantIdAndDateAsync_WithValidParameters_ShouldReturnDailySummary()
        {
            // Arrange
            var date = DateTime.Today;
            var dailySummary = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(date)
                .Build();
            await AddEntityAsync(dailySummary);

            // Act
            var result = await _repository.GetByMerchantIdAndDateAsync(_merchant.Id, date);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(dailySummary.Id);
            result.MerchantId.Should().Be(_merchant.Id);
            result.Date.Date.Should().Be(date.Date);
        }

        [Fact]
        public async Task GetByMerchantIdAndDateAsync_WithNonExistingParameters_ShouldReturnNull()
        {
            // Arrange
            var nonExistingMerchantId = RandomGuid();
            var date = DateTime.Today;

            // Act
            var result = await _repository.GetByMerchantIdAndDateAsync(nonExistingMerchantId, date);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByMerchantIdAndDateAsync_WithDifferentTime_ShouldReturnDailySummary()
        {
            // Arrange
            var date = DateTime.Today;
            var dailySummary = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(date)
                .Build();
            await AddEntityAsync(dailySummary);

            // Act - Search with different time but same date
            var result = await _repository.GetByMerchantIdAndDateAsync(_merchant.Id, date.AddHours(10));

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(dailySummary.Id);
        }

        [Fact]
        public async Task GetByMerchantIdAndDateRangeAsync_WithValidParameters_ShouldReturnDailySummariesInRange()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(-5);
            var endDate = DateTime.Today.AddDays(-1);
            var outsideDate = DateTime.Today.AddDays(-10);

            var summary1 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(startDate.AddDays(1))
                .Build();
            var summary2 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(endDate.AddDays(-1))
                .Build();
            var summary3 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(outsideDate)
                .Build();

            await AddEntitiesAsync(new[] { summary1, summary2, summary3 });

            // Act
            var result = await _repository.GetByMerchantIdAndDateRangeAsync(_merchant.Id, startDate, endDate);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(ds => ds.Id == summary1.Id);
            result.Should().Contain(ds => ds.Id == summary2.Id);
            result.Should().NotContain(ds => ds.Id == summary3.Id);
        }

        [Fact]
        public async Task GetByMerchantIdAndDateRangeAsync_WithNoSummariesInRange_ShouldReturnEmptyCollection()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(-5);
            var endDate = DateTime.Today.AddDays(-1);
            var outsideDate = DateTime.Today.AddDays(-10);

            var summary = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(outsideDate)
                .Build();
            await AddEntityAsync(summary);

            // Act
            var result = await _repository.GetByMerchantIdAndDateRangeAsync(_merchant.Id, startDate, endDate);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByMerchantIdAndDateRangeAsync_ShouldReturnSummariesOrderedByDateAscending()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(-5);
            var endDate = DateTime.Today.AddDays(-1);

            var summary1 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(startDate.AddDays(2))
                .WithNetAmount(100)
                .Build();
            var summary2 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(startDate.AddDays(1))
                .WithNetAmount(200)
                .Build();

            await AddEntitiesAsync(new[] { summary1, summary2 });

            // Act
            var result = await _repository.GetByMerchantIdAndDateRangeAsync(_merchant.Id, startDate, endDate);

            // Assert
            var items = result.ToList();
            items.Should().HaveCount(2);
            items[0].NetAmount.Should().Be(200); // Earlier date first
            items[1].NetAmount.Should().Be(100);
        }

        [Fact]
        public async Task GetPagedByMerchantIdAsync_WithValidParameters_ShouldReturnPagedResults()
        {
            // Arrange
            var summaries = new List<DailySummary>();
            for (int i = 0; i < 10; i++)
            {
                summaries.Add(DailySummaryBuilder.Create()
                    .WithMerchantId(_merchant.Id)
                    .WithDate(DateTime.Today.AddDays(-i))
                    .Build());
            }
            await AddEntitiesAsync(summaries);

            // Act
            var result = await _repository.GetPagedByMerchantIdAsync(_merchant.Id, 2, 3);

            // Assert
            result.Items.Should().HaveCount(3);
            result.TotalCount.Should().Be(10);
            result.Items.Should().OnlyContain(ds => ds.MerchantId == _merchant.Id);
        }

        [Fact]
        public async Task GetPagedByMerchantIdAsync_WithPageOutOfRange_ShouldReturnEmptyItems()
        {
            // Arrange
            var summaries = new[]
            {
                DailySummaryBuilder.Create().WithMerchantId(_merchant.Id).Build(),
                DailySummaryBuilder.Create().WithMerchantId(_merchant.Id).Build()
            };
            await AddEntitiesAsync(summaries);

            // Act
            var result = await _repository.GetPagedByMerchantIdAsync(_merchant.Id, 5, 10);

            // Assert
            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task GetTotalNetAmountByMerchantIdAndDateRangeAsync_WithValidParameters_ShouldReturnCorrectTotal()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(-5);
            var endDate = DateTime.Today.AddDays(-1);

            var summary1 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(startDate.AddDays(1))
                .WithNetAmount(100.50m)
                .Build();
            var summary2 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(endDate.AddDays(-1))
                .WithNetAmount(200.25m)
                .Build();
            var summary3 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(DateTime.Today.AddDays(-10)) // Outside range
                .WithNetAmount(50.00m)
                .Build();

            await AddEntitiesAsync(new[] { summary1, summary2, summary3 });

            // Act
            var result = await _repository.GetTotalNetAmountByMerchantIdAndDateRangeAsync(_merchant.Id, startDate, endDate);

            // Assert
            result.Should().Be(300.75m);
        }

        [Fact]
        public async Task GetTotalNetAmountByMerchantIdAndDateRangeAsync_WithNoSummaries_ShouldReturnZero()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(-5);
            var endDate = DateTime.Today.AddDays(-1);

            // Act
            var result = await _repository.GetTotalNetAmountByMerchantIdAndDateRangeAsync(_merchant.Id, startDate, endDate);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task GetTotalTransactionCountByMerchantIdAndDateRangeAsync_WithValidParameters_ShouldReturnCorrectTotal()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(-5);
            var endDate = DateTime.Today.AddDays(-1);

            var summary1 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(startDate.AddDays(1))
                .WithTransactionCount(5)
                .Build();
            var summary2 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(endDate.AddDays(-1))
                .WithTransactionCount(3)
                .Build();
            var summary3 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(DateTime.Today.AddDays(-10)) // Outside range
                .WithTransactionCount(2)
                .Build();

            await AddEntitiesAsync(new[] { summary1, summary2, summary3 });

            // Act
            var result = await _repository.GetTotalTransactionCountByMerchantIdAndDateRangeAsync(_merchant.Id, startDate, endDate);

            // Assert
            result.Should().Be(8);
        }

        [Fact]
        public async Task GetTotalTransactionCountByMerchantIdAndDateRangeAsync_WithNoSummaries_ShouldReturnZero()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(-5);
            var endDate = DateTime.Today.AddDays(-1);

            // Act
            var result = await _repository.GetTotalTransactionCountByMerchantIdAndDateRangeAsync(_merchant.Id, startDate, endDate);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task GetPositiveBalanceByMerchantIdAsync_WithValidParameters_ShouldReturnPositiveBalanceSummaries()
        {
            // Arrange
            var positiveSummary1 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithNetAmount(100.50m)
                .Build();
            var positiveSummary2 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithNetAmount(200.25m)
                .Build();
            var negativeSummary = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithNetAmount(-50.00m)
                .Build();
            var zeroSummary = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithNetAmount(0)
                .Build();

            await AddEntitiesAsync(new[] { positiveSummary1, positiveSummary2, negativeSummary, zeroSummary });

            // Act
            var result = await _repository.GetPositiveBalanceByMerchantIdAsync(_merchant.Id);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(ds => ds.NetAmount > 0);
            result.Should().Contain(ds => ds.Id == positiveSummary1.Id);
            result.Should().Contain(ds => ds.Id == positiveSummary2.Id);
        }

        [Fact]
        public async Task GetPositiveBalanceByMerchantIdAsync_WithNoPositiveBalances_ShouldReturnEmptyCollection()
        {
            // Arrange
            var negativeSummary = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithNetAmount(-50.00m)
                .Build();
            var zeroSummary = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithNetAmount(0)
                .Build();

            await AddEntitiesAsync(new[] { negativeSummary, zeroSummary });

            // Act
            var result = await _repository.GetPositiveBalanceByMerchantIdAsync(_merchant.Id);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetNegativeBalanceByMerchantIdAsync_WithValidParameters_ShouldReturnNegativeBalanceSummaries()
        {
            // Arrange
            var negativeSummary1 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithNetAmount(-100.50m)
                .Build();
            var negativeSummary2 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithNetAmount(-200.25m)
                .Build();
            var positiveSummary = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithNetAmount(50.00m)
                .Build();
            var zeroSummary = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithNetAmount(0)
                .Build();

            await AddEntitiesAsync(new[] { negativeSummary1, negativeSummary2, positiveSummary, zeroSummary });

            // Act
            var result = await _repository.GetNegativeBalanceByMerchantIdAsync(_merchant.Id);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(ds => ds.NetAmount < 0);
            result.Should().Contain(ds => ds.Id == negativeSummary1.Id);
            result.Should().Contain(ds => ds.Id == negativeSummary2.Id);
        }

        [Fact]
        public async Task GetNegativeBalanceByMerchantIdAsync_WithNoNegativeBalances_ShouldReturnEmptyCollection()
        {
            // Arrange
            var positiveSummary = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithNetAmount(50.00m)
                .Build();
            var zeroSummary = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithNetAmount(0)
                .Build();

            await AddEntitiesAsync(new[] { positiveSummary, zeroSummary });

            // Act
            var result = await _repository.GetNegativeBalanceByMerchantIdAsync(_merchant.Id);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPagedByMerchantIdAsync_ShouldReturnSummariesOrderedByDateDescending()
        {
            // Arrange
            var summary1 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(DateTime.Today.AddDays(-2))
                .WithNetAmount(100)
                .Build();
            var summary2 = DailySummaryBuilder.Create()
                .WithMerchantId(_merchant.Id)
                .WithDate(DateTime.Today.AddDays(-1))
                .WithNetAmount(200)
                .Build();

            await AddEntitiesAsync(new[] { summary1, summary2 });

            // Act
            var result = await _repository.GetPagedByMerchantIdAsync(_merchant.Id, 1, 10);

            // Assert
            var items = result.Items.ToList();
            items.Should().HaveCount(2);
            items[0].NetAmount.Should().Be(200); // Most recent first
            items[1].NetAmount.Should().Be(100);
        }

        public void Dispose()
        {
            Cleanup();
        }
    }
}
