using CleanCode.Application.Transactions.DeleteTransaction;
using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;
using CleanCode.Domain.Repositories.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Bogus;

namespace CleanCode.Tests.UnitTests.Handlers.Transactions.DeleteTransaction
{
    public class DeleteTransactionHandlerTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<ILogger<DeleteTransactionHandler>> _mockLogger;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly DeleteTransactionHandler _handler;
        private readonly Faker<DeleteTransactionCommand> _commandFaker;
        private readonly Faker<Transaction> _transactionFaker;

        public DeleteTransactionHandlerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockLogger = new Mock<ILogger<DeleteTransactionHandler>>();
            _mockMessageService = new Mock<IMessageService>();
            _handler = new DeleteTransactionHandler(_mockTransactionRepository.Object, _mockLogger.Object, _mockMessageService.Object);

            _commandFaker = new Faker<DeleteTransactionCommand>()
                .CustomInstantiator(f => new DeleteTransactionCommand(f.Random.Guid()));

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
        public async Task Handle_WithValidCommand_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transaction = _transactionFaker.Generate();

            _mockTransactionRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);
            _mockTransactionRepository.Setup(x => x.DeleteAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(command.Id);
            response.Success.Should().BeTrue();
            response.Message.Should().Be("Transação deletada com sucesso");

            _mockTransactionRepository.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
            _mockTransactionRepository.Verify(x => x.DeleteAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentTransaction_ShouldReturnFailureResult()
        {
            // Arrange
            var command = _commandFaker.Generate();

            _mockTransactionRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Transaction?)null);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(command.Id);
            response.Success.Should().BeFalse();
            response.Message.Should().Contain($"Transação com ID {command.Id} não encontrada");

            _mockTransactionRepository.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
            _mockTransactionRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldSendMessageToQueue()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transaction = _transactionFaker.Generate();

            _mockTransactionRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);
            _mockTransactionRepository.Setup(x => x.DeleteAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockMessageService.Verify(x => x.SendMessageAsync(
                It.IsAny<object>(),
                "transaction_deleted",
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
