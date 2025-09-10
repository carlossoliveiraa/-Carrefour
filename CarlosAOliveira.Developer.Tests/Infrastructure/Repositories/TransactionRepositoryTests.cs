using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.ORM.Repositories;
using CarlosAOliveira.Developer.Tests.Builders;
using CarlosAOliveira.Developer.Tests.Infrastructure.Common;
using FluentAssertions;

namespace CarlosAOliveira.Developer.Tests.Infrastructure.Repositories
{
    /// <summary>
    /// Unit tests for TransactionRepository
    /// </summary>
    public class TransactionRepositoryTests : RepositoryTestBase<Transaction>
    {
        private TransactionRepository _repository = null!;
        private Merchant _merchant = null!;

        public TransactionRepositoryTests()
        {
            Setup();
            _repository = new TransactionRepository(Context);
            _merchant = MerchantBuilder.Create().Build();
            Context.Merchants.Add(_merchant);
            Context.SaveChanges();
        }

        [Fact]
        public async Task GetByMerchantIdAsync_WithValidMerchantId_ShouldReturnTransactions()
        {
            // Arrange
            var transactions = new[]
            {
                TransactionBuilder.Create().WithMerchantId(_merchant.Id).Build(),
                TransactionBuilder.Create().WithMerchantId(_merchant.Id).Build(),
                TransactionBuilder.Create().WithMerchantId(_merchant.Id).Build()
            };
            await AddEntitiesAsync(transactions);

            // Act
            var result = await _repository.GetByMerchantIdAsync(_merchant.Id);

            // Assert
            result.Should().HaveCount(3);
            result.Should().OnlyContain(t => t.MerchantId == _merchant.Id);
        }

        [Fact]
        public async Task GetByMerchantIdAsync_WithNonExistingMerchantId_ShouldReturnEmptyCollection()
        {
            // Arrange
            var nonExistingMerchantId = RandomGuid();

            // Act
            var result = await _repository.GetByMerchantIdAsync(nonExistingMerchantId);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByMerchantIdAsync_ShouldReturnTransactionsOrderedByCreatedAtDescending()
        {
            // Arrange
            var transaction1 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithDescription("First").Build();
            var transaction2 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithDescription("Second").Build();
            var transaction3 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithDescription("Third").Build();

            // Add with small delays to ensure different CreatedAt times
            await AddEntityAsync(transaction1);
            await Task.Delay(10);
            await AddEntityAsync(transaction2);
            await Task.Delay(10);
            await AddEntityAsync(transaction3);

            // Act
            var result = await _repository.GetByMerchantIdAsync(_merchant.Id);

            // Assert
            var items = result.ToList();
            items.Should().HaveCount(3);
            items[0].Description.Should().Be("Third"); // Most recent first
            items[1].Description.Should().Be("Second");
            items[2].Description.Should().Be("First");
        }

        [Fact]
        public async Task GetByMerchantIdAndDateRangeAsync_WithValidParameters_ShouldReturnTransactionsInRange()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow.AddDays(-1);
            var outsideDate = DateTime.UtcNow.AddDays(-10);

            var transaction1 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).Build();
            var transaction2 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).Build();
            var transaction3 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).Build();

            // Manually set CreatedAt dates
            transaction1.GetType().GetProperty("CreatedAt")!.SetValue(transaction1, startDate.AddDays(1));
            transaction2.GetType().GetProperty("CreatedAt")!.SetValue(transaction2, endDate.AddDays(-1));
            transaction3.GetType().GetProperty("CreatedAt")!.SetValue(transaction3, outsideDate);

            await AddEntitiesAsync(new[] { transaction1, transaction2, transaction3 });

            // Act
            var result = await _repository.GetByMerchantIdAndDateRangeAsync(_merchant.Id, startDate, endDate);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(t => t.Id == transaction1.Id);
            result.Should().Contain(t => t.Id == transaction2.Id);
            result.Should().NotContain(t => t.Id == transaction3.Id);
        }

        [Fact]
        public async Task GetByMerchantIdAndDateRangeAsync_WithNoTransactionsInRange_ShouldReturnEmptyCollection()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow.AddDays(-1);
            var outsideDate = DateTime.UtcNow.AddDays(-10);

            var transaction = TransactionBuilder.Create().WithMerchantId(_merchant.Id).Build();
            transaction.GetType().GetProperty("CreatedAt")!.SetValue(transaction, outsideDate);
            await AddEntityAsync(transaction);

            // Act
            var result = await _repository.GetByMerchantIdAndDateRangeAsync(_merchant.Id, startDate, endDate);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByMerchantIdAndTypeAsync_WithValidParameters_ShouldReturnTransactionsOfSpecificType()
        {
            // Arrange
            var creditTransaction1 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithType(TransactionType.Credit).Build();
            var creditTransaction2 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithType(TransactionType.Credit).Build();
            var debitTransaction = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithType(TransactionType.Debit).Build();

            await AddEntitiesAsync(new[] { creditTransaction1, creditTransaction2, debitTransaction });

            // Act
            var result = await _repository.GetByMerchantIdAndTypeAsync(_merchant.Id, TransactionType.Credit);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(t => t.Type == TransactionType.Credit);
            result.Should().Contain(t => t.Id == creditTransaction1.Id);
            result.Should().Contain(t => t.Id == creditTransaction2.Id);
        }

        [Fact]
        public async Task GetByMerchantIdAndTypeAsync_WithNoTransactionsOfType_ShouldReturnEmptyCollection()
        {
            // Arrange
            var creditTransaction = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithType(TransactionType.Credit).Build();
            await AddEntityAsync(creditTransaction);

            // Act
            var result = await _repository.GetByMerchantIdAndTypeAsync(_merchant.Id, TransactionType.Debit);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPagedByMerchantIdAsync_WithValidParameters_ShouldReturnPagedResults()
        {
            // Arrange
            var transactions = new List<Transaction>();
            for (int i = 0; i < 10; i++)
            {
                transactions.Add(TransactionBuilder.Create().WithMerchantId(_merchant.Id).Build());
            }
            await AddEntitiesAsync(transactions);

            // Act
            var result = await _repository.GetPagedByMerchantIdAsync(_merchant.Id, 2, 3);

            // Assert
            result.Items.Should().HaveCount(3);
            result.TotalCount.Should().Be(10);
            result.Items.Should().OnlyContain(t => t.MerchantId == _merchant.Id);
        }

        [Fact]
        public async Task GetPagedByMerchantIdAsync_WithPageOutOfRange_ShouldReturnEmptyItems()
        {
            // Arrange
            var transactions = new[]
            {
                TransactionBuilder.Create().WithMerchantId(_merchant.Id).Build(),
                TransactionBuilder.Create().WithMerchantId(_merchant.Id).Build()
            };
            await AddEntitiesAsync(transactions);

            // Act
            var result = await _repository.GetPagedByMerchantIdAsync(_merchant.Id, 5, 10);

            // Assert
            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task GetTotalAmountByMerchantIdAndTypeAsync_WithValidParameters_ShouldReturnCorrectTotal()
        {
            // Arrange
            var creditTransaction1 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithType(TransactionType.Credit).WithAmount(100).Build();
            var creditTransaction2 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithType(TransactionType.Credit).WithAmount(200).Build();
            var debitTransaction = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithType(TransactionType.Debit).WithAmount(50).Build();

            await AddEntitiesAsync(new[] { creditTransaction1, creditTransaction2, debitTransaction });

            // Act
            var result = await _repository.GetTotalAmountByMerchantIdAndTypeAsync(_merchant.Id, TransactionType.Credit);

            // Assert
            result.Should().Be(300);
        }

        [Fact]
        public async Task GetTotalAmountByMerchantIdAndTypeAsync_WithNoTransactions_ShouldReturnZero()
        {
            // Act
            var result = await _repository.GetTotalAmountByMerchantIdAndTypeAsync(_merchant.Id, TransactionType.Credit);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task GetCountByMerchantIdAndTypeAsync_WithValidParameters_ShouldReturnCorrectCount()
        {
            // Arrange
            var creditTransaction1 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithType(TransactionType.Credit).Build();
            var creditTransaction2 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithType(TransactionType.Credit).Build();
            var debitTransaction = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithType(TransactionType.Debit).Build();

            await AddEntitiesAsync(new[] { creditTransaction1, creditTransaction2, debitTransaction });

            // Act
            var result = await _repository.GetCountByMerchantIdAndTypeAsync(_merchant.Id, TransactionType.Credit);

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public async Task GetCountByMerchantIdAndTypeAsync_WithNoTransactions_ShouldReturnZero()
        {
            // Act
            var result = await _repository.GetCountByMerchantIdAndTypeAsync(_merchant.Id, TransactionType.Credit);

            // Assert
            result.Should().Be(0);
        }

        [Theory]
        [InlineData(TransactionType.Credit, 100.50)]
        [InlineData(TransactionType.Debit, 50.25)]
        public async Task GetTotalAmountByMerchantIdAndTypeAsync_WithDifferentTypes_ShouldReturnCorrectAmounts(TransactionType type, decimal amount)
        {
            // Arrange
            var transaction = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithType(type).WithAmount(amount).Build();
            await AddEntityAsync(transaction);

            // Act
            var result = await _repository.GetTotalAmountByMerchantIdAndTypeAsync(_merchant.Id, type);

            // Assert
            result.Should().Be(amount);
        }

        [Fact]
        public async Task GetPagedByMerchantIdAsync_ShouldReturnTransactionsOrderedByCreatedAtDescending()
        {
            // Arrange
            var transaction1 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithDescription("First").Build();
            var transaction2 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithDescription("Second").Build();
            var transaction3 = TransactionBuilder.Create().WithMerchantId(_merchant.Id).WithDescription("Third").Build();

            // Add with small delays to ensure different CreatedAt times
            await AddEntityAsync(transaction1);
            await Task.Delay(10);
            await AddEntityAsync(transaction2);
            await Task.Delay(10);
            await AddEntityAsync(transaction3);

            // Act
            var result = await _repository.GetPagedByMerchantIdAsync(_merchant.Id, 1, 10);

            // Assert
            var items = result.Items.ToList();
            items.Should().HaveCount(3);
            items[0].Description.Should().Be("Third"); // Most recent first
            items[1].Description.Should().Be("Second");
            items[2].Description.Should().Be("First");
        }

        public void Dispose()
        {
            Cleanup();
        }
    }
}
