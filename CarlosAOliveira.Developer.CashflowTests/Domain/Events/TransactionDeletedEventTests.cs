using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Domain.Events;
using Xunit;

namespace CarlosAOliveira.Developer.Tests.Domain.Events
{
    public class TransactionDeletedEventTests
    {
        [Fact]
        public void Constructor_WithValidTransaction_ShouldCreateEvent()
        {
            // Arrange
            var transaction = new Transaction(
                DateOnly.FromDateTime(DateTime.Today),
                100.50m,
                TransactionType.Credit,
                "Sales",
                "Test transaction"
            );

            // Act
            var domainEvent = new TransactionDeletedEvent(transaction);

            // Assert
            Assert.Equal(transaction.Id, domainEvent.TransactionId);
            Assert.Equal(transaction.Date, domainEvent.Date);
            Assert.Equal(transaction.Amount, domainEvent.Amount);
            Assert.Equal(transaction.Type.ToString(), domainEvent.TransactionType);
            Assert.True(domainEvent.DeletedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Constructor_WithDebitTransaction_ShouldCreateEventWithCorrectType()
        {
            // Arrange
            var transaction = new Transaction(
                DateOnly.FromDateTime(DateTime.Today),
                75.25m,
                TransactionType.Debit,
                "Expenses",
                "Test debit transaction"
            );

            // Act
            var domainEvent = new TransactionDeletedEvent(transaction);

            // Assert
            Assert.Equal("Debit", domainEvent.TransactionType);
            Assert.Equal(transaction.Amount, domainEvent.Amount);
        }
    }
}
