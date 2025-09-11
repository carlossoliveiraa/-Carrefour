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

        public TransactionRepositoryTests()
        {
            Setup();
            _repository = new TransactionRepository(Context);
        }

        [Fact]
        public async Task GetByDateAsync_WithValidDate_ShouldReturnTransactions()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.Today);
            var transactions = new[]
            {
                TransactionBuilder.Create().WithDate(date).Build(),
                TransactionBuilder.Create().WithDate(date).Build(),
                TransactionBuilder.Create().WithDate(date).Build()
            };
            await AddEntitiesAsync(transactions);

            // Act
            var result = await _repository.GetByDateAsync(date);

            // Assert
            result.Should().HaveCount(3);
            result.Should().OnlyContain(t => t.Date == date);
        }

        [Fact]
        public async Task GetByDateAsync_WithNonExistingDate_ShouldReturnEmptyCollection()
        {
            // Arrange
            var nonExistingDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-100));

            // Act
            var result = await _repository.GetByDateAsync(nonExistingDate);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByDateRangeAsync_WithValidRange_ShouldReturnTransactions()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5));
            var endDate = DateOnly.FromDateTime(DateTime.Today);
            var transactions = new[]
            {
                TransactionBuilder.Create().WithDate(startDate).Build(),
                TransactionBuilder.Create().WithDate(startDate.AddDays(1)).Build(),
                TransactionBuilder.Create().WithDate(endDate).Build()
            };
            await AddEntitiesAsync(transactions);

            // Act
            var result = await _repository.GetByDateRangeAsync(startDate, endDate);

            // Assert
            result.Should().HaveCount(3);
            result.Should().OnlyContain(t => t.Date >= startDate && t.Date <= endDate);
        }

        [Fact]
        public async Task GetByTypeAsync_WithValidType_ShouldReturnTransactions()
        {
            // Arrange
            var transactions = new[]
            {
                TransactionBuilder.Create().AsCredit().Build(),
                TransactionBuilder.Create().AsCredit().Build(),
                TransactionBuilder.Create().AsDebit().Build()
            };
            await AddEntitiesAsync(transactions);

            // Act
            var result = await _repository.GetByTypeAsync(TransactionType.Credit);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(t => t.Type == TransactionType.Credit);
        }

        [Fact]
        public async Task GetPagedAsync_WithValidParameters_ShouldReturnPagedResults()
        {
            // Arrange
            var transactions = new List<Transaction>();
            for (int i = 0; i < 10; i++)
            {
                transactions.Add(TransactionBuilder.Create().Build());
            }
            await AddEntitiesAsync(transactions);

            // Act
            var result = await _repository.GetPagedAsync(1, 5);

            // Assert
            result.Items.Should().HaveCount(5);
            result.TotalCount.Should().Be(10);
        }

        [Fact]
        public async Task GetTotalAmountByTypeAsync_WithValidType_ShouldReturnCorrectTotal()
        {
            // Arrange
            var transactions = new[]
            {
                TransactionBuilder.Create().AsCredit().WithAmount(100).Build(),
                TransactionBuilder.Create().AsCredit().WithAmount(200).Build(),
                TransactionBuilder.Create().AsDebit().WithAmount(50).Build()
            };
            await AddEntitiesAsync(transactions);

            // Act
            var result = await _repository.GetTotalAmountByTypeAsync(TransactionType.Credit);

            // Assert
            result.Should().Be(300);
        }

        [Fact]
        public async Task GetCountByTypeAsync_WithValidType_ShouldReturnCorrectCount()
        {
            // Arrange
            var transactions = new[]
            {
                TransactionBuilder.Create().AsCredit().Build(),
                TransactionBuilder.Create().AsCredit().Build(),
                TransactionBuilder.Create().AsDebit().Build()
            };
            await AddEntitiesAsync(transactions);

            // Act
            var result = await _repository.GetCountByTypeAsync(TransactionType.Credit);

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public async Task AddAsync_WithValidTransaction_ShouldAddToDatabase()
        {
            // Arrange
            var transaction = TransactionBuilder.Create().Build();

            // Act
            var result = await _repository.AddAsync(transaction);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            
            var savedTransaction = await Context.Transactions.FindAsync(transaction.Id);
            savedTransaction.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldPersistChanges()
        {
            // Arrange
            var transaction = TransactionBuilder.Create().Build();
            Context.Transactions.Add(transaction);

            // Act
            var result = await _repository.SaveChangesAsync();

            // Assert
            result.Should().BeGreaterThan(0);
            
            var savedTransaction = await Context.Transactions.FindAsync(transaction.Id);
            savedTransaction.Should().NotBeNull();
        }

        public void Dispose()
        {
            Cleanup();
        }
    }
}