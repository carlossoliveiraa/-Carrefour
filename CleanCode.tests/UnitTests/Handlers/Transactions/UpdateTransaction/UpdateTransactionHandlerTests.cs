using AutoMapper;
using CleanCode.Application.Transactions.UpdateTransaction;
using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;
using CleanCode.Domain.Repositories.Interfaces;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Bogus;

namespace CleanCode.Tests.UnitTests.Handlers.Transactions.UpdateTransaction
{
    public class UpdateTransactionHandlerTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UpdateTransactionHandler>> _mockLogger;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly UpdateTransactionHandler _handler;
        private readonly Faker<UpdateTransactionCommand> _commandFaker;
        private readonly Faker<Transaction> _transactionFaker;

        public UpdateTransactionHandlerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UpdateTransactionHandler>>();
            _mockMessageService = new Mock<IMessageService>();
            _handler = new UpdateTransactionHandler(_mockTransactionRepository.Object, _mockMapper.Object, _mockLogger.Object, _mockMessageService.Object);

            _commandFaker = new Faker<UpdateTransactionCommand>()
                .CustomInstantiator(f => new UpdateTransactionCommand(f.Random.Guid()))
                .RuleFor(c => c.Description, f => f.Commerce.ProductName())
                .RuleFor(c => c.Amount, f => f.Random.Decimal(1, 10000))
                .RuleFor(c => c.Type, f => f.PickRandom(new[] { TransactionType.Credit, TransactionType.Debit }))
                .RuleFor(c => c.TransactionDate, f => f.Date.Recent())
                .RuleFor(c => c.Category, f => f.Commerce.Categories(1).FirstOrDefault())
                .RuleFor(c => c.Notes, f => f.Lorem.Sentence());

            _transactionFaker = new Faker<Transaction>()
                .RuleFor(t => t.Id, f => f.Random.Guid())
                .RuleFor(t => t.Description, f => f.Commerce.ProductName())
                .RuleFor(t => t.Amount, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.Type, f => f.PickRandom(new[] { TransactionType.Credit, TransactionType.Debit }))
                .RuleFor(t => t.TransactionDate, f => f.Date.Recent())
                .RuleFor(t => t.Category, f => f.Commerce.Categories(1).FirstOrDefault())
                .RuleFor(t => t.Notes, f => f.Lorem.Sentence());
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldReturnUpdatedTransaction()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var existingTransaction = _transactionFaker.Generate();
            var updatedTransaction = _transactionFaker.Generate();
            var result = new UpdateTransactionResult { Id = updatedTransaction.Id };

            _mockTransactionRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTransaction);
            _mockTransactionRepository.Setup(x => x.UpdateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedTransaction);
            _mockMapper.Setup(x => x.Map<UpdateTransactionResult>(updatedTransaction)).Returns(result);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(updatedTransaction.Id);

            _mockTransactionRepository.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
            _mockTransactionRepository.Verify(x => x.UpdateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentTransaction_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var command = _commandFaker.Generate();

            _mockTransactionRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Transaction?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain($"Transação com ID {command.Id} não encontrada");
        }

        [Fact]
        public async Task Handle_WithInvalidCommand_ShouldThrowValidationException()
        {
            // Arrange
            var command = new UpdateTransactionCommand(Guid.NewGuid())
            {
                Description = "", // Invalid
                Amount = -1, // Invalid
                Type = TransactionType.None, // Invalid
                TransactionDate = default
            };

            // Act & Assert
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldSendMessageToQueue()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var existingTransaction = _transactionFaker.Generate();
            var updatedTransaction = _transactionFaker.Generate();
            var result = new UpdateTransactionResult { Id = updatedTransaction.Id };

            _mockTransactionRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTransaction);
            _mockTransactionRepository.Setup(x => x.UpdateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedTransaction);
            _mockMapper.Setup(x => x.Map<UpdateTransactionResult>(updatedTransaction)).Returns(result);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockMessageService.Verify(x => x.SendMessageAsync(
                It.IsAny<object>(),
                "transaction_updated",
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
