using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Domain.Events;
using CarlosAOliveira.Developer.Tests.Builders;
using FluentAssertions;

namespace CarlosAOliveira.Developer.Tests.Domain.Entities
{
    /// <summary>
    /// Unit tests for Merchant entity
    /// </summary>
    public class MerchantTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateMerchant()
        {
            // Arrange
            var name = "Test Merchant";
            var email = "test@example.com";

            // Act
            var merchant = new Merchant(name, email);

            // Assert
            merchant.Should().NotBeNull();
            merchant.Name.Should().Be(name);
            merchant.Email.Should().Be(email);
            merchant.Id.Should().NotBeEmpty();
            merchant.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        // CreateTransaction method was removed from Merchant entity

        // CreateTransaction method was removed from Merchant entity

        [Fact]
        public void UpdateInformation_WithValidData_ShouldUpdateMerchant()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            var newName = "Updated Merchant";
            var newEmail = "updated@example.com";

            // Act
            merchant.UpdateInformation(newName, newEmail);

            // Assert
            merchant.Name.Should().Be(newName);
            merchant.Email.Should().Be(newEmail);
        }

        // CreateTransaction method was removed from Merchant entity

        [Fact]
        public void Constructor_ShouldSetCreatedAtToCurrentTime()
        {
            // Arrange & Act
            var merchant = MerchantBuilder.Create().Build();

            // Assert
            merchant.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Id_ShouldBeUniqueForEachMerchant()
        {
            // Arrange & Act
            var merchant1 = MerchantBuilder.Create().Build();
            var merchant2 = MerchantBuilder.Create().Build();

            // Assert
            merchant1.Id.Should().NotBe(merchant2.Id);
        }
    }
}
