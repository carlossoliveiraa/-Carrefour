using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Tests.Builders;
using FluentAssertions;

namespace CarlosAOliveira.Developer.Tests.Domain.Entities
{
    /// <summary>
    /// Unit tests for Transaction entity
    /// </summary>
    public class TransactionTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateTransaction()
        {
            // Arrange
            var merchantId = Guid.NewGuid();
            var amount = 100.50m;
            var type = TransactionType.Credit;
            var description = "Test transaction";

            // Act
            var transaction = new Transaction(merchantId, amount, type, description);

            // Assert
            transaction.Should().NotBeNull();
            transaction.MerchantId.Should().Be(merchantId);
            transaction.Amount.Should().Be(amount);
            transaction.Type.Should().Be(type);
            transaction.Description.Should().Be(description);
            transaction.Id.Should().NotBeEmpty();
            transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void IsCredit_WhenTypeIsCredit_ShouldReturnTrue()
        {
            // Arrange
            var transaction = TransactionBuilder.Create().AsCredit().Build();

            // Act & Assert
            transaction.IsCredit.Should().BeTrue();
            transaction.IsDebit.Should().BeFalse();
        }

        [Fact]
        public void IsDebit_WhenTypeIsDebit_ShouldReturnTrue()
        {
            // Arrange
            var transaction = TransactionBuilder.Create().AsDebit().Build();

            // Act & Assert
            transaction.IsDebit.Should().BeTrue();
            transaction.IsCredit.Should().BeFalse();
        }

        [Fact]
        public void SignedAmount_WhenCredit_ShouldReturnPositiveAmount()
        {
            // Arrange
            var amount = 100.50m;
            var transaction = TransactionBuilder.Create()
                .AsCredit()
                .WithAmount(amount)
                .Build();

            // Act & Assert
            transaction.SignedAmount.Should().Be(amount);
        }

        [Fact]
        public void SignedAmount_WhenDebit_ShouldReturnNegativeAmount()
        {
            // Arrange
            var amount = 100.50m;
            var transaction = TransactionBuilder.Create()
                .AsDebit()
                .WithAmount(amount)
                .Build();

            // Act & Assert
            transaction.SignedAmount.Should().Be(-amount);
        }

        [Theory]
        [InlineData(TransactionType.Credit, 100.50, 100.50)]
        [InlineData(TransactionType.Debit, 100.50, -100.50)]
        [InlineData(TransactionType.Credit, 0, 0)]
        [InlineData(TransactionType.Debit, 0, 0)]
        public void SignedAmount_WithDifferentTypes_ShouldReturnCorrectValue(
            TransactionType type, decimal amount, decimal expectedSignedAmount)
        {
            // Arrange
            var transaction = TransactionBuilder.Create()
                .WithType(type)
                .WithAmount(amount)
                .Build();

            // Act & Assert
            transaction.SignedAmount.Should().Be(expectedSignedAmount);
        }

        [Fact]
        public void Constructor_ShouldSetCreatedAtToCurrentTime()
        {
            // Arrange & Act
            var transaction = TransactionBuilder.Create().Build();

            // Assert
            transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Id_ShouldBeUniqueForEachTransaction()
        {
            // Arrange & Act
            var transaction1 = TransactionBuilder.Create().Build();
            var transaction2 = TransactionBuilder.Create().Build();

            // Assert
            transaction1.Id.Should().NotBe(transaction2.Id);
        }

        [Fact]
        public void Constructor_WithZeroAmount_ShouldCreateTransaction()
        {
            // Arrange
            var merchantId = Guid.NewGuid();
            var amount = 0m;
            var type = TransactionType.Credit;
            var description = "Zero amount transaction";

            // Act
            var transaction = new Transaction(merchantId, amount, type, description);

            // Assert
            transaction.Amount.Should().Be(0);
            transaction.SignedAmount.Should().Be(0);
        }

        [Fact]
        public void Constructor_WithEmptyDescription_ShouldCreateTransaction()
        {
            // Arrange
            var merchantId = Guid.NewGuid();
            var amount = 100m;
            var type = TransactionType.Credit;
            var description = "";

            // Act
            var transaction = new Transaction(merchantId, amount, type, description);

            // Assert
            transaction.Description.Should().Be("");
        }
    }
}
