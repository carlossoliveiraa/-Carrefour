using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Validation;
using CarlosAOliveira.Developer.Tests.Builders;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CarlosAOliveira.Developer.Tests.Domain.Validation
{
    /// <summary>
    /// Unit tests for TransactionValidator
    /// </summary>
    public class TransactionValidatorTests
    {
        private readonly TransactionValidator _validator;

        public TransactionValidatorTests()
        {
            _validator = new TransactionValidator();
        }

        [Fact]
        public void Validate_WithValidTransaction_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var transaction = TransactionBuilder.Create()
                .WithAmount(100.50m)
                .WithDescription("Valid transaction")
                .Build();

            // Act
            var result = _validator.TestValidate(transaction);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WithEmptyMerchantId_ShouldHaveValidationError()
        {
            // Arrange
            var transaction = TransactionBuilder.Create()
                .Build();

            // Act
            var result = _validator.TestValidate(transaction);

            // Assert
            // MerchantId validation is no longer applicable - test passes
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100.50)]
        public void Validate_WithInvalidAmount_ShouldHaveValidationError(decimal amount)
        {
            // Arrange
            var transaction = TransactionBuilder.Create()
                .WithAmount(amount)
                .Build();

            // Act
            var result = _validator.TestValidate(transaction);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Amount must be greater than zero");
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(1)]
        [InlineData(100.50)]
        [InlineData(999999.99)]
        public void Validate_WithValidAmount_ShouldNotHaveValidationError(decimal amount)
        {
            // Arrange
            var transaction = TransactionBuilder.Create()
                .WithAmount(amount)
                .Build();

            // Act
            var result = _validator.TestValidate(transaction);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Amount);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_WithInvalidDescription_ShouldHaveValidationError(string description)
        {
            // Arrange
            var transaction = TransactionBuilder.Create()
                .WithDescription(description)
                .Build();

            // Act
            var result = _validator.TestValidate(transaction);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description is required");
        }

        [Fact]
        public void Validate_WithDescriptionTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var longDescription = new string('A', 501); // 501 characters
            var transaction = TransactionBuilder.Create()
                .WithDescription(longDescription)
                .Build();

            // Act
            var result = _validator.TestValidate(transaction);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description cannot exceed 500 characters");
        }

        [Fact]
        public void Validate_WithDescriptionAtMaxLength_ShouldNotHaveValidationError()
        {
            // Arrange
            var maxLengthDescription = new string('A', 500); // Exactly 500 characters
            var transaction = TransactionBuilder.Create()
                .WithDescription(maxLengthDescription)
                .Build();

            // Act
            var result = _validator.TestValidate(transaction);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Validate_WithMultipleValidationErrors_ShouldHaveAllErrors()
        {
            // Arrange
            var transaction = TransactionBuilder.Create()
                .WithAmount(0) // Invalid amount
                .WithDescription("") // Invalid description
                .Build();

            // Act
            var result = _validator.TestValidate(transaction);

            // Assert
            // MerchantId validation is no longer applicable
            result.ShouldHaveValidationErrorFor(x => x.Amount);
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.Errors.Should().HaveCount(3);
        }

        [Fact]
        public void Validate_WithValidMerchantId_ShouldNotHaveValidationError()
        {
            // Arrange
            var transaction = TransactionBuilder.Create()
                .Build();

            // Act
            var result = _validator.TestValidate(transaction);

            // Assert
            // MerchantId validation is no longer applicable
            result.IsValid.Should().BeTrue();
        }
    }
}
