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

        [Fact]
        public void CreateTransaction_WithValidData_ShouldCreateTransaction()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            var amount = 100.50m;
            var type = TransactionType.Credit;
            var description = "Test transaction";

            // Act
            var transaction = merchant.CreateTransaction(DateOnly.FromDateTime(DateTime.Today), amount, type, "Test Category", description);

            // Assert
            transaction.Should().NotBeNull();
            // MerchantId is no longer part of Transaction entity
            transaction.Amount.Should().Be(amount);
            transaction.Type.Should().Be(type);
            transaction.Description.Should().Be(description);
            transaction.Id.Should().NotBeEmpty();
            transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void CreateTransaction_ShouldRaiseTransactionCreatedEvent()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            var amount = 100.50m;
            var type = TransactionType.Credit;
            var description = "Test transaction";

            // Act
            var transaction = merchant.CreateTransaction(DateOnly.FromDateTime(DateTime.Today), amount, type, "Test Category", description);

            // Assert
            merchant.DomainEvents.Should().HaveCount(1);
            var domainEvent = merchant.DomainEvents.First() as TransactionCreatedEvent;
            domainEvent.Should().NotBeNull();
            domainEvent!.TransactionId.Should().Be(transaction.Id);
            // MerchantId is no longer part of TransactionCreatedEvent
            domainEvent.Amount.Should().Be(amount);
            domainEvent.TransactionType.Should().Be(type.ToString());
        }

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

        [Theory]
        [InlineData(TransactionType.Credit, 100.50)]
        [InlineData(TransactionType.Debit, 50.25)]
        public void CreateTransaction_WithDifferentTypes_ShouldCreateCorrectTransaction(TransactionType type, decimal amount)
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            var description = "Test transaction";

            // Act
            var transaction = merchant.CreateTransaction(DateOnly.FromDateTime(DateTime.Today), amount, type, "Test Category", description);

            // Assert
            transaction.Type.Should().Be(type);
            transaction.Amount.Should().Be(amount);
        }

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
