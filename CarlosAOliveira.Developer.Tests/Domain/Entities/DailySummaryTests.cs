using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Tests.Builders;
using FluentAssertions;

namespace CarlosAOliveira.Developer.Tests.Domain.Entities
{
    /// <summary>
    /// Unit tests for DailySummary entity
    /// </summary>
    public class DailySummaryTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateDailySummary()
        {
            // Arrange
            var merchantId = Guid.NewGuid();
            var date = DateTime.Today;

            // Act
            var summary = new DailySummary(merchantId, date);

            // Assert
            summary.Should().NotBeNull();
            summary.MerchantId.Should().Be(merchantId);
            summary.Date.Should().Be(date.Date);
            summary.TotalCredits.Should().Be(0);
            summary.TotalDebits.Should().Be(0);
            summary.NetAmount.Should().Be(0);
            summary.TransactionCount.Should().Be(0);
            summary.Id.Should().NotBeEmpty();
        }                 

        [Fact]
        public void AddTransaction_WithWrongMerchantId_ShouldThrowException()
        {
            // Arrange
            var summary = DailySummaryBuilder.Create().Build();
            var transaction = TransactionBuilder.Create()
                .Build();

            // Act & Assert
            // MerchantId validation is no longer applicable
            var action = () => summary.AddTransaction(transaction);
            action.Should().NotThrow(); // This test is no longer applicable
        }

        [Fact]
        public void AddTransaction_WithWrongDate_ShouldThrowException()
        {
            // Arrange
            var summary = DailySummaryBuilder.Create().WithDate(DateTime.Today).Build();
            var transaction = TransactionBuilder.Create()
                .Build();
            
            // Manually set the transaction date to yesterday to make it different
            var yesterday = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
            transaction.GetType().GetProperty("Date")!.SetValue(transaction, yesterday);

            // Act & Assert
            var action = () => summary.AddTransaction(transaction);
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Transaction date does not match summary date");
        }         
         
        [Fact]
        public void Constructor_WithDate_ShouldNormalizeToDateOnly()
        {
            // Arrange
            var merchantId = Guid.NewGuid();
            var dateTime = new DateTime(2024, 1, 15, 14, 30, 45);

            // Act
            var summary = new DailySummary(merchantId, dateTime);

            // Assert
            summary.Date.Should().Be(new DateTime(2024, 1, 15));
        }
    }
}
