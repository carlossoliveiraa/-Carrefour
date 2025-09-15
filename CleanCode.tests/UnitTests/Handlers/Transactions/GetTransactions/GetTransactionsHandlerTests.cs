using AutoMapper;
using CleanCode.Application.Transactions.GetTransactions;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;
using CleanCode.Domain.Repositories.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Bogus;

namespace CleanCode.Tests.UnitTests.Handlers.Transactions.GetTransactions
{
    public class GetTransactionsHandlerTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<GetTransactionsHandler>> _mockLogger;
        private readonly GetTransactionsHandler _handler;
        private readonly Faker<GetTransactionsCommand> _commandFaker;
        private readonly Faker<Transaction> _transactionFaker;

        public GetTransactionsHandlerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<GetTransactionsHandler>>();
            _handler = new GetTransactionsHandler(_mockTransactionRepository.Object, _mockMapper.Object, _mockLogger.Object);

            _commandFaker = new Faker<GetTransactionsCommand>()
                .RuleFor(c => c.StartDate, f => f.Date.Recent())
                .RuleFor(c => c.EndDate, f => f.Date.Future())
                .RuleFor(c => c.Type, f => f.PickRandom(new[] { TransactionType.Credit, TransactionType.Debit }))
                .RuleFor(c => c.Category, f => f.Commerce.Categories(1).FirstOrDefault())
                .RuleFor(c => c.Page, f => f.Random.Int(1, 10))
                .RuleFor(c => c.PageSize, f => f.Random.Int(5, 20));

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
        public async Task Handle_WithValidCommand_ShouldReturnTransactions()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transactions = _transactionFaker.Generate(5);
            
            // Ajusta as datas das transações para estarem dentro do intervalo do comando
            for (int i = 0; i < transactions.Count; i++)
            {
                transactions[i].TransactionDate = command.StartDate!.Value.AddDays(i);
                transactions[i].Type = command.Type!.Value;
                transactions[i].Category = command.Category;
            }
            var transactionItems = transactions.Select(t => new GetTransactionsResult.TransactionItem
            {
                Id = t.Id,
                Description = t.Description,
                Amount = t.Amount,
                Type = t.Type.ToString(),
                TransactionDate = t.TransactionDate,
                Category = t.Category,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            var result = new GetTransactionsResult
            {
                Transactions = transactionItems,
                CurrentPage = command.Page,
                PageSize = command.PageSize,
                TotalCount = 5,
                TotalPages = 1
            };

            _mockTransactionRepository.Setup(x => x.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);
            _mockMapper.Setup(x => x.Map<List<GetTransactionsResult.TransactionItem>>(It.IsAny<List<Transaction>>()))
                .Returns(transactionItems);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Transactions.Should().HaveCount(5);
            response.CurrentPage.Should().Be(command.Page);
            response.PageSize.Should().Be(command.PageSize);
            response.TotalCount.Should().Be(5);
            response.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task Handle_WithNoTransactions_ShouldReturnEmptyList()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var emptyTransactions = new List<Transaction>();

            _mockTransactionRepository.Setup(x => x.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyTransactions);
            _mockMapper.Setup(x => x.Map<List<GetTransactionsResult.TransactionItem>>(It.IsAny<List<Transaction>>()))
                .Returns(new List<GetTransactionsResult.TransactionItem>());

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Transactions.Should().BeEmpty();
            response.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task Handle_WithDateRange_ShouldCallCorrectRepositoryMethod()
        {
            // Arrange
            var command = new GetTransactionsCommand(
                startDate: DateTime.UtcNow.AddDays(-7),
                endDate: DateTime.UtcNow,
                page: 1,
                pageSize: 10);

            var transactions = _transactionFaker.Generate(3);

            _mockTransactionRepository.Setup(x => x.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockTransactionRepository.Verify(x => x.GetByDateRangeAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldLogInformation()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transactions = _transactionFaker.Generate(2);

            _mockTransactionRepository.Setup(x => x.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Buscando transações com filtros")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
