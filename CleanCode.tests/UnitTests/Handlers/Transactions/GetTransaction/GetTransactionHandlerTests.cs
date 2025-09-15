using AutoMapper;
using CleanCode.Application.Transactions.GetTransaction;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;
using CleanCode.Domain.Repositories.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Bogus;

namespace CleanCode.Tests.UnitTests.Handlers.Transactions.GetTransaction
{
    public class GetTransactionHandlerTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<GetTransactionHandler>> _mockLogger;
        private readonly GetTransactionHandler _handler;
        private readonly Faker<GetTransactionCommand> _commandFaker;
        private readonly Faker<Transaction> _transactionFaker;

        public GetTransactionHandlerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<GetTransactionHandler>>();
            _handler = new GetTransactionHandler(_mockTransactionRepository.Object, _mockMapper.Object, _mockLogger.Object);

            _commandFaker = new Faker<GetTransactionCommand>()
                .CustomInstantiator(f => new GetTransactionCommand(f.Random.Guid()));

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
        public async Task Handle_WithValidId_ShouldReturnTransaction()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transaction = _transactionFaker.Generate();
            var result = new GetTransactionResult
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Amount = transaction.Amount,
                Type = transaction.Type.ToString(),
                TransactionDate = transaction.TransactionDate,
                Category = transaction.Category,
                Notes = transaction.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockTransactionRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);
            _mockMapper.Setup(x => x.Map<GetTransactionResult>(transaction)).Returns(result);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(transaction.Id);
            response.Description.Should().Be(transaction.Description);
            response.Amount.Should().Be(transaction.Amount);
            response.Type.Should().Be(transaction.Type.ToString());

            _mockTransactionRepository.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentId_ShouldThrowKeyNotFoundException()
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
        public async Task Handle_WithValidId_ShouldLogInformation()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transaction = _transactionFaker.Generate();
            var result = new GetTransactionResult { Id = transaction.Id };

            _mockTransactionRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);
            _mockMapper.Setup(x => x.Map<GetTransactionResult>(transaction)).Returns(result);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Buscando transação")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
