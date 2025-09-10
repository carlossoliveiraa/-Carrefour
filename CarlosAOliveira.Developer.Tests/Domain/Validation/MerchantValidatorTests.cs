using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Validation;
using CarlosAOliveira.Developer.Tests.Builders;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CarlosAOliveira.Developer.Tests.Domain.Validation
{
    /// <summary>
    /// Unit tests for MerchantValidator
    /// </summary>
    public class MerchantValidatorTests
    {
        private readonly MerchantValidator _validator;

        public MerchantValidatorTests()
        {
            _validator = new MerchantValidator();
        }

        [Fact]
        public void Validate_WithValidMerchant_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var merchant = MerchantBuilder.Create()
                .WithName("Valid Merchant")
                .WithEmail("valid@example.com")
                .Build();

            // Act
            var result = _validator.TestValidate(merchant);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_WithInvalidName_ShouldHaveValidationError(string name)
        {
            // Arrange
            var merchant = MerchantBuilder.Create()
                .WithName(name)
                .Build();

            // Act
            var result = _validator.TestValidate(merchant);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Merchant name is required");
        }

        [Fact]
        public void Validate_WithNameTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var longName = new string('A', 101); // 101 characters
            var merchant = MerchantBuilder.Create()
                .WithName(longName)
                .Build();

            // Act
            var result = _validator.TestValidate(merchant);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Merchant name cannot exceed 100 characters");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_WithInvalidEmail_ShouldHaveValidationError(string email)
        {
            // Arrange
            var merchant = MerchantBuilder.Create()
                .WithEmail(email)
                .Build();

            // Act
            var result = _validator.TestValidate(merchant);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Email is required");
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("test@")]
        [InlineData("test.example.com")]
        public void Validate_WithInvalidEmailFormat_ShouldHaveValidationError(string email)
        {
            // Arrange
            var merchant = MerchantBuilder.Create()
                .WithEmail(email)
                .Build();

            // Act
            var result = _validator.TestValidate(merchant);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Invalid email format");
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@domain.co.uk")]
        [InlineData("user+tag@example.org")]
        [InlineData("user123@test-domain.com")]
        public void Validate_WithValidEmailFormat_ShouldNotHaveValidationError(string email)
        {
            // Arrange
            var merchant = MerchantBuilder.Create()
                .WithEmail(email)
                .Build();

            // Act
            var result = _validator.TestValidate(merchant);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Validate_WithNameAtMaxLength_ShouldNotHaveValidationError()
        {
            // Arrange
            var maxLengthName = new string('A', 100); // Exactly 100 characters
            var merchant = MerchantBuilder.Create()
                .WithName(maxLengthName)
                .Build();

            // Act
            var result = _validator.TestValidate(merchant);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Validate_WithMultipleValidationErrors_ShouldHaveAllErrors()
        {
            // Arrange
            var merchant = MerchantBuilder.Create()
                .WithName("") // Invalid name
                .WithEmail("invalid-email") // Invalid email
                .Build();

            // Act
            var result = _validator.TestValidate(merchant);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldHaveValidationErrorFor(x => x.Email);
            result.Errors.Should().HaveCount(2);
        }
    }
}
