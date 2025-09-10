using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Validation;
using CarlosAOliveira.Developer.Tests.Builders;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CarlosAOliveira.Developer.Tests.Domain.Validation
{
    /// <summary>
    /// Unit tests for DailySummaryValidator
    /// </summary>
    public class DailySummaryValidatorTests
    {
        private readonly DailySummaryValidator _validator;

        public DailySummaryValidatorTests()
        {
            _validator = new DailySummaryValidator();
        }

        [Fact]
        public void Validate_WithValidDailySummary_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var summary = DailySummaryBuilder.Create()
                .WithDate(DateTime.Today)
                .Build();

            // Act
            var result = _validator.TestValidate(summary);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WithEmptyMerchantId_ShouldHaveValidationError()
        {
            // Arrange
            var summary = DailySummaryBuilder.Create()
                .WithMerchantId(Guid.Empty)
                .Build();

            // Act
            var result = _validator.TestValidate(summary);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MerchantId)
                .WithErrorMessage("Merchant ID is required");
        }

        [Fact]
        public void Validate_WithDefaultDate_ShouldHaveValidationError()
        {
            // Arrange
            var summary = DailySummaryBuilder.Create()
                .WithDate(default(DateTime))
                .Build();

            // Act
            var result = _validator.TestValidate(summary);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("Date is required");
        }

        [Fact]
        public void Validate_WithFutureDate_ShouldHaveValidationError()
        {
            // Arrange
            var futureDate = DateTime.Today.AddDays(1);
            var summary = DailySummaryBuilder.Create()
                .WithDate(futureDate)
                .Build();

            // Act
            var result = _validator.TestValidate(summary);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("Date cannot be in the future");
        }

        [Fact]
        public void Validate_WithTodayDate_ShouldNotHaveValidationError()
        {
            // Arrange
            var summary = DailySummaryBuilder.Create()
                .WithDate(DateTime.Today)
                .Build();

            // Act
            var result = _validator.TestValidate(summary);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Date);
        }

        [Fact]
        public void Validate_WithPastDate_ShouldNotHaveValidationError()
        {
            // Arrange
            var pastDate = DateTime.Today.AddDays(-1);
            var summary = DailySummaryBuilder.Create()
                .WithDate(pastDate)
                .Build();

            // Act
            var result = _validator.TestValidate(summary);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Date);
        }

        [Fact]
        public void Validate_WithNegativeTotalCredits_ShouldHaveValidationError()
        {
            // Arrange
            var summary = DailySummaryBuilder.Create().Build();
            // Manually set negative value using reflection for testing
            var totalCreditsProperty = typeof(DailySummary).GetProperty("TotalCredits");
            totalCreditsProperty?.SetValue(summary, -10m);

            // Act
            var result = _validator.TestValidate(summary);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TotalCredits)
                .WithErrorMessage("Total credits cannot be negative");
        }

        [Fact]
        public void Validate_WithNegativeTotalDebits_ShouldHaveValidationError()
        {
            // Arrange
            var summary = DailySummaryBuilder.Create().Build();
            // Manually set negative value using reflection for testing
            var totalDebitsProperty = typeof(DailySummary).GetProperty("TotalDebits");
            totalDebitsProperty?.SetValue(summary, -10m);

            // Act
            var result = _validator.TestValidate(summary);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TotalDebits)
                .WithErrorMessage("Total debits cannot be negative");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100.50)]
        [InlineData(999999.99)]
        public void Validate_WithValidTotalCredits_ShouldNotHaveValidationError(decimal totalCredits)
        {
            // Arrange
            var summary = DailySummaryBuilder.Create().Build();
            // Manually set value using reflection for testing
            var totalCreditsProperty = typeof(DailySummary).GetProperty("TotalCredits");
            totalCreditsProperty?.SetValue(summary, totalCredits);

            // Act
            var result = _validator.TestValidate(summary);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TotalCredits);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100.50)]
        [InlineData(999999.99)]
        public void Validate_WithValidTotalDebits_ShouldNotHaveValidationError(decimal totalDebits)
        {
            // Arrange
            var summary = DailySummaryBuilder.Create().Build();
            // Manually set value using reflection for testing
            var totalDebitsProperty = typeof(DailySummary).GetProperty("TotalDebits");
            totalDebitsProperty?.SetValue(summary, totalDebits);

            // Act
            var result = _validator.TestValidate(summary);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TotalDebits);
        }

        [Fact]
        public void Validate_WithNegativeTransactionCount_ShouldHaveValidationError()
        {
            // Arrange
            var summary = DailySummaryBuilder.Create().Build();
            // Manually set negative value using reflection for testing
            var transactionCountProperty = typeof(DailySummary).GetProperty("TransactionCount");
            transactionCountProperty?.SetValue(summary, -1);

            // Act
            var result = _validator.TestValidate(summary);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TransactionCount)
                .WithErrorMessage("Transaction count cannot be negative");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void Validate_WithValidTransactionCount_ShouldNotHaveValidationError(int transactionCount)
        {
            // Arrange
            var summary = DailySummaryBuilder.Create().Build();
            // Manually set value using reflection for testing
            var transactionCountProperty = typeof(DailySummary).GetProperty("TransactionCount");
            transactionCountProperty?.SetValue(summary, transactionCount);

            // Act
            var result = _validator.TestValidate(summary);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TransactionCount);
        }
    }
}
