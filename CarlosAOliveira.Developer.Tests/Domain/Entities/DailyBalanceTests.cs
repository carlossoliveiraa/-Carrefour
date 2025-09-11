using CarlosAOliveira.Developer.Domain.Entities;
using Xunit;

namespace CarlosAOliveira.Developer.Tests.Domain.Entities
{
    public class DailyBalanceTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateDailyBalance()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.Today);
            var balance = 1000.50m;

            // Act
            var dailyBalance = new DailyBalance(date, balance);

            // Assert
            Assert.Equal(date, dailyBalance.Date);
            Assert.Equal(balance, dailyBalance.Balance);
            Assert.NotEqual(Guid.Empty, dailyBalance.Id);
            Assert.True(dailyBalance.LastUpdated <= DateTime.UtcNow);
        }

        [Fact]
        public void UpdateBalance_WithNewBalance_ShouldUpdateBalance()
        {
            // Arrange
            var dailyBalance = new DailyBalance(DateOnly.FromDateTime(DateTime.Today), 100m);
            var newBalance = 200m;

            // Act
            dailyBalance.UpdateBalance(newBalance);

            // Assert
            Assert.Equal(newBalance, dailyBalance.Balance);
            Assert.True(dailyBalance.LastUpdated <= DateTime.UtcNow);
        }

        [Fact]
        public void AddAmount_WithPositiveAmount_ShouldIncreaseBalance()
        {
            // Arrange
            var initialBalance = 100m;
            var dailyBalance = new DailyBalance(DateOnly.FromDateTime(DateTime.Today), initialBalance);
            var amountToAdd = 50m;

            // Act
            dailyBalance.AddAmount(amountToAdd);

            // Assert
            Assert.Equal(initialBalance + amountToAdd, dailyBalance.Balance);
        }

        [Fact]
        public void AddAmount_WithNegativeAmount_ShouldDecreaseBalance()
        {
            // Arrange
            var initialBalance = 100m;
            var dailyBalance = new DailyBalance(DateOnly.FromDateTime(DateTime.Today), initialBalance);
            var amountToSubtract = 30m;

            // Act
            dailyBalance.AddAmount(-amountToSubtract);

            // Assert
            Assert.Equal(initialBalance - amountToSubtract, dailyBalance.Balance);
        }

        [Fact]
        public void AddAmount_WithZeroAmount_ShouldNotChangeBalance()
        {
            // Arrange
            var initialBalance = 100m;
            var dailyBalance = new DailyBalance(DateOnly.FromDateTime(DateTime.Today), initialBalance);

            // Act
            dailyBalance.AddAmount(0);

            // Assert
            Assert.Equal(initialBalance, dailyBalance.Balance);
        }
    }
}
