using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Events;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Tests.Builders;
using FluentAssertions;

namespace CarlosAOliveira.Developer.Tests.Domain.Events
{
    /// <summary>
    /// Unit tests for TransactionCreatedEvent
    /// </summary>
    public class TransactionCreatedEventTests
    {
        [Fact]
        public void Constructor_WithValidTransaction_ShouldCreateEvent()
        {
            // Arrange
            var transaction = TransactionBuilder.Create()
                .WithAmount(100.50m)
                .AsCredit()
                .WithDescription("Test transaction")
                .Build();

            // Act
            var domainEvent = new TransactionCreatedEvent(transaction);

            // Assert
            domainEvent.Should().NotBeNull();
            domainEvent.TransactionId.Should().Be(transaction.Id);
            domainEvent.MerchantId.Should().Be(transaction.MerchantId);
            domainEvent.Amount.Should().Be(transaction.Amount);
            domainEvent.TransactionType.Should().Be(transaction.Type.ToString());
            domainEvent.CreatedAt.Should().Be(transaction.CreatedAt);
        }

        [Fact]
        public void Constructor_WithCreditTransaction_ShouldSetCorrectType()
        {
            // Arrange
            var transaction = TransactionBuilder.Create()
                .AsCredit()
                .Build();

            // Act
            var domainEvent = new TransactionCreatedEvent(transaction);

            // Assert
            domainEvent.TransactionType.Should().Be("Credit");
        }

        [Fact]
        public void Constructor_WithDebitTransaction_ShouldSetCorrectType()
        {
            // Arrange
            var transaction = TransactionBuilder.Create()
                .AsDebit()
                .Build();

            // Act
            var domainEvent = new TransactionCreatedEvent(transaction);

            // Assert
            domainEvent.TransactionType.Should().Be("Debit");
        }

        [Fact]
        public void Constructor_WithZeroAmount_ShouldCreateEvent()
        {
            // Arrange
            var transaction = TransactionBuilder.Create()
                .WithAmount(0)
                .Build();

            // Act
            var domainEvent = new TransactionCreatedEvent(transaction);

            // Assert
            domainEvent.Amount.Should().Be(0);
        }

        [Fact]
        public void Constructor_WithLargeAmount_ShouldCreateEvent()
        {
            // Arrange
            var largeAmount = 999999.99m;
            var transaction = TransactionBuilder.Create()
                .WithAmount(largeAmount)
                .Build();

            // Act
            var domainEvent = new TransactionCreatedEvent(transaction);

            // Assert
            domainEvent.Amount.Should().Be(largeAmount);
        }

        [Fact]
        public void Constructor_ShouldCopyAllTransactionProperties()
        {
            // Arrange
            var merchantId = Guid.NewGuid();
            var amount = 250.75m;
            var type = TransactionType.Debit;
            var description = "Test transaction description";
            
            var transaction = TransactionBuilder.Create()
                .WithMerchantId(merchantId)
                .WithAmount(amount)
                .WithType(type)
                .WithDescription(description)
                .Build();

            // Act
            var domainEvent = new TransactionCreatedEvent(transaction);

            // Assert
            domainEvent.TransactionId.Should().Be(transaction.Id);
            domainEvent.MerchantId.Should().Be(merchantId);
            domainEvent.Amount.Should().Be(amount);
            domainEvent.TransactionType.Should().Be(type.ToString());
            domainEvent.CreatedAt.Should().Be(transaction.CreatedAt);
        }       
    }
}
