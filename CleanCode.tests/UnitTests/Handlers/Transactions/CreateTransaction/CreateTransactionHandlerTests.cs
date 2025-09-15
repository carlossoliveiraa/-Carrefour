using AutoMapper;
using CleanCode.Application.Transactions.CreateTransaction;
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

namespace CleanCode.Tests.UnitTests.Handlers.Transactions.CreateTransaction
{
    public class CreateTransactionHandlerTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<CreateTransactionHandler>> _mockLogger;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly CreateTransactionHandler _handler;
        private readonly Faker<CreateTransactionCommand> _commandFaker;
        private readonly Faker<Transaction> _transactionFaker;

        public CreateTransactionHandlerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<CreateTransactionHandler>>();
            _mockMessageService = new Mock<IMessageService>();
            _handler = new CreateTransactionHandler(_mockTransactionRepository.Object, _mockMapper.Object, _mockLogger.Object, _mockMessageService.Object);

            _commandFaker = new Faker<CreateTransactionCommand>()
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
        public async Task Handle_WithValidCommand_ShouldCreateTransaction()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transaction = _transactionFaker.Generate();
            var result = new CreateTransactionResult
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Amount = transaction.Amount,
                Type = transaction.Type.ToString(),
                TransactionDate = transaction.TransactionDate,
                Category = transaction.Category,
                Notes = transaction.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _mockMapper.Setup(x => x.Map<Transaction>(command)).Returns(transaction);
            _mockTransactionRepository.Setup(x => x.CreateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);
            _mockMapper.Setup(x => x.Map<CreateTransactionResult>(transaction)).Returns(result);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(transaction.Id);
            response.Description.Should().Be(transaction.Description);
            response.Amount.Should().Be(transaction.Amount);
            response.Type.Should().Be(transaction.Type.ToString());

            _mockTransactionRepository.Verify(x => x.CreateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidCommand_ShouldThrowValidationException()
        {
            // Arrange
            var command = new CreateTransactionCommand
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
        public async Task Handle_WithValidCommand_ShouldLogInformation()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transaction = _transactionFaker.Generate();
            var result = new CreateTransactionResult { Id = transaction.Id };

            _mockMapper.Setup(x => x.Map<Transaction>(command)).Returns(transaction);
            _mockTransactionRepository.Setup(x => x.CreateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);
            _mockMapper.Setup(x => x.Map<CreateTransactionResult>(transaction)).Returns(result);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Iniciando criação de transação")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldSendMessageToQueue()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transaction = _transactionFaker.Generate();
            var result = new CreateTransactionResult { Id = transaction.Id };

            _mockMapper.Setup(x => x.Map<Transaction>(command)).Returns(transaction);
            _mockTransactionRepository.Setup(x => x.CreateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);
            _mockMapper.Setup(x => x.Map<CreateTransactionResult>(transaction)).Returns(result);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockMessageService.Verify(x => x.SendMessageAsync(
                It.IsAny<object>(),
                "transaction_created",
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
