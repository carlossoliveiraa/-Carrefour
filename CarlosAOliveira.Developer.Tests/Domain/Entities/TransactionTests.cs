using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using Xunit;

namespace CarlosAOliveira.Developer.Tests.Domain.Entities
{
    public class TransactionTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateTransaction()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.Today);
            var amount = 100.50m;
            var type = TransactionType.Credit;
            var category = "Sales";
            var description = "Test transaction";

            // Act
            var transaction = new Transaction(date, amount, type, category, description);

            // Assert
            Assert.Equal(date, transaction.Date);
            Assert.Equal(amount, transaction.Amount.Amount);
            Assert.Equal(type, transaction.Type);
            Assert.Equal(category, transaction.Category);
            Assert.Equal(description, transaction.Description);
            Assert.NotEqual(Guid.Empty, transaction.Id);
            Assert.True(transaction.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void IsCredit_WhenTypeIsCredit_ShouldReturnTrue()
        {
            // Arrange
            var transaction = new Transaction(
                DateOnly.FromDateTime(DateTime.Today),
                100m,
                TransactionType.Credit,
                "Sales",
                "Test"
            );

            // Act & Assert
            Assert.True(transaction.IsCredit);
            Assert.False(transaction.IsDebit);
        }

        [Fact]
        public void IsDebit_WhenTypeIsDebit_ShouldReturnTrue()
        {
            // Arrange
            var transaction = new Transaction(
                DateOnly.FromDateTime(DateTime.Today),
                100m,
                TransactionType.Debit,
                "Expenses",
                "Test"
            );

            // Act & Assert
            Assert.True(transaction.IsDebit);
            Assert.False(transaction.IsCredit);
        }

        [Fact]
        public void SignedAmount_WhenCredit_ShouldReturnPositiveAmount()
        {
            // Arrange
            var amount = 100.50m;
            var transaction = new Transaction(
                DateOnly.FromDateTime(DateTime.Today),
                amount,
                TransactionType.Credit,
                "Sales",
                "Test"
            );

            // Act
            var signedAmount = transaction.SignedAmount;

            // Assert
            Assert.Equal(amount, signedAmount);
        }

        [Fact]
        public void SignedAmount_WhenDebit_ShouldReturnNegativeAmount()
        {
            // Arrange
            var amount = 100.50m;
            var transaction = new Transaction(
                DateOnly.FromDateTime(DateTime.Today),
                amount,
                TransactionType.Debit,
                "Expenses",
                "Test"
            );

            // Act
            var signedAmount = transaction.SignedAmount;

            // Assert
            Assert.Equal(-amount, signedAmount);
        }
    }
}