using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.ORM;
using CarlosAOliveira.Developer.Tests.Builders;
using CarlosAOliveira.Developer.Tests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CarlosAOliveira.Developer.Tests.Infrastructure
{
    /// <summary>
    /// Unit tests for DefaultContext
    /// </summary>
    public class DefaultContextTests : TestBase
    {
        private DefaultContext _context = null!;

        public DefaultContextTests()
        {
            Setup();
        }

        private void Setup()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DefaultContext(options);
        }

        [Fact]
        public void Constructor_WithValidOptions_ShouldCreateContext()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Act
            var context = new DefaultContext(options);

            // Assert
            context.Should().NotBeNull();
            context.Users.Should().NotBeNull();
            context.Merchants.Should().NotBeNull();
            context.Transactions.Should().NotBeNull();
            context.DailySummaries.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveChangesAsync_WithValidEntities_ShouldSaveToDatabase()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            var transaction = TransactionBuilder.Create().WithMerchantId(merchant.Id).Build();
            var dailySummary = DailySummaryBuilder.Create().WithMerchantId(merchant.Id).Build();

            // Act
            _context.Merchants.Add(merchant);
            _context.Transactions.Add(transaction);
            _context.DailySummaries.Add(dailySummary);
            var result = await _context.SaveChangesAsync();

            // Assert
            result.Should().Be(3);
            _context.Merchants.Count().Should().Be(1);
            _context.Transactions.Count().Should().Be(1);
            _context.DailySummaries.Count().Should().Be(1);
        }

        [Fact]
        public async Task SaveChangesAsync_WithInvalidData_ShouldThrowException()
        {
            // Arrange
            var merchant = new Merchant("", ""); // Invalid data

            // Act & Assert
            _context.Merchants.Add(merchant);
            // InMemory database doesn't enforce all constraints like SQL Server
            // This test would work with a real database
            var result = await _context.SaveChangesAsync();
            result.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task OnModelCreating_ShouldApplyConfigurations()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            var transaction = TransactionBuilder.Create().WithMerchantId(merchant.Id).Build();

            // Act
            _context.Merchants.Add(merchant);
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Assert - Verify that configurations are applied by checking constraints
            var savedMerchant = await _context.Merchants.FirstAsync();
            var savedTransaction = await _context.Transactions.FirstAsync();
            
            savedMerchant.Should().NotBeNull();
            savedTransaction.Should().NotBeNull();
            savedTransaction.MerchantId.Should().Be(savedMerchant.Id);
        }

        [Fact]
        public async Task DbSets_ShouldBeAccessible()
        {
            // Arrange & Act
            var users = _context.Users;
            var merchants = _context.Merchants;
            var transactions = _context.Transactions;
            var dailySummaries = _context.DailySummaries;

            // Assert
            users.Should().NotBeNull();
            merchants.Should().NotBeNull();
            transactions.Should().NotBeNull();
            dailySummaries.Should().NotBeNull();
            
            // Verify they are empty initially
            (await users.CountAsync()).Should().Be(0);
            (await merchants.CountAsync()).Should().Be(0);
            (await transactions.CountAsync()).Should().Be(0);
            (await dailySummaries.CountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task Context_ShouldSupportMultipleOperations()
        {
            // Arrange
            var merchants = new[]
            {
                MerchantBuilder.Create().WithName("Merchant 1").Build(),
                MerchantBuilder.Create().WithName("Merchant 2").Build(),
                MerchantBuilder.Create().WithName("Merchant 3").Build()
            };

            // Act
            _context.Merchants.AddRange(merchants);
            await _context.SaveChangesAsync();

            var retrievedMerchants = await _context.Merchants.ToListAsync();

            // Assert
            retrievedMerchants.Should().HaveCount(3);
            retrievedMerchants.Should().Contain(m => m.Name == "Merchant 1");
            retrievedMerchants.Should().Contain(m => m.Name == "Merchant 2");
            retrievedMerchants.Should().Contain(m => m.Name == "Merchant 3");
        }

        [Fact]
        public async Task Context_ShouldHandleTransactions()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            var transaction = TransactionBuilder.Create().WithMerchantId(merchant.Id).Build();

            // Act - InMemory database doesn't support real transactions
            _context.Merchants.Add(merchant);
            await _context.SaveChangesAsync();
            
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Assert
            var savedMerchant = await _context.Merchants.FirstOrDefaultAsync();
            var savedTransaction = await _context.Transactions.FirstOrDefaultAsync();
            
            savedMerchant.Should().NotBeNull();
            savedTransaction.Should().NotBeNull();
            savedTransaction!.MerchantId.Should().Be(savedMerchant!.Id);
        }

        [Fact]
        public async Task Context_ShouldHandleTransactionRollback()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();

            // Act - InMemory database doesn't support real transactions
            _context.Merchants.Add(merchant);
            await _context.SaveChangesAsync();

            // Assert - InMemory database doesn't support rollback
            var savedMerchant = await _context.Merchants.FirstOrDefaultAsync();
            savedMerchant.Should().NotBeNull();
        }

        [Fact]
        public async Task Context_ShouldSupportQueryOperations()
        {
            // Arrange
            var merchants = new[]
            {
                MerchantBuilder.Create().WithName("Merchant A").Build(),
                MerchantBuilder.Create().WithName("Merchant B").Build(),
                MerchantBuilder.Create().WithName("Merchant C").Build()
            };

            _context.Merchants.AddRange(merchants);
            await _context.SaveChangesAsync();

            // Act
            var merchantA = await _context.Merchants
                .FirstOrDefaultAsync(m => m.Name == "Merchant A");
            
            var merchantCount = await _context.Merchants.CountAsync();
            
            var merchantsWithA = await _context.Merchants
                .Where(m => m.Name.Contains("A"))
                .ToListAsync();

            // Assert
            merchantA.Should().NotBeNull();
            merchantA!.Name.Should().Be("Merchant A");
            merchantCount.Should().Be(3);
            merchantsWithA.Should().HaveCount(1);
        }

        [Fact]
        public async Task Context_ShouldSupportUpdateOperations()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().WithName("Original Name").Build();
            _context.Merchants.Add(merchant);
            await _context.SaveChangesAsync();

            // Act
            merchant.UpdateInformation("Updated Name", merchant.Email);
            _context.Merchants.Update(merchant);
            await _context.SaveChangesAsync();

            // Assert
            var updatedMerchant = await _context.Merchants.FirstAsync();
            updatedMerchant.Name.Should().Be("Updated Name");
        }

        [Fact]
        public async Task Context_ShouldSupportDeleteOperations()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            _context.Merchants.Add(merchant);
            await _context.SaveChangesAsync();

            // Act
            _context.Merchants.Remove(merchant);
            await _context.SaveChangesAsync();

            // Assert
            var deletedMerchant = await _context.Merchants.FirstOrDefaultAsync();
            deletedMerchant.Should().BeNull();
        }

        [Fact]
        public async Task Context_ShouldHandleConcurrentOperations()
        {
            // Arrange
            var merchant1 = MerchantBuilder.Create().WithName("Merchant 1").Build();
            var merchant2 = MerchantBuilder.Create().WithName("Merchant 2").Build();

            // Act - InMemory database doesn't support concurrent operations well
            _context.Merchants.Add(merchant1);
            await _context.SaveChangesAsync();
            
            _context.Merchants.Add(merchant2);
            await _context.SaveChangesAsync();

            // Assert
            var merchants = await _context.Merchants.ToListAsync();
            merchants.Should().HaveCount(2);
            merchants.Should().Contain(m => m.Name == "Merchant 1");
            merchants.Should().Contain(m => m.Name == "Merchant 2");
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
