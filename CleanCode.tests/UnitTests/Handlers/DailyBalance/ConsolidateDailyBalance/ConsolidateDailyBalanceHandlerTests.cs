using AutoMapper;
using CleanCode.Application.DailyBalance.ConsolidateDailyBalance;
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

namespace CleanCode.Tests.UnitTests.Handlers.DailyBalance.ConsolidateDailyBalance
{
    public class ConsolidateDailyBalanceHandlerTests
    {
        private readonly Mock<IDailyBalanceRepository> _mockDailyBalanceRepository;
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ConsolidateDailyBalanceHandler>> _mockLogger;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly ConsolidateDailyBalanceHandler _handler;
        private readonly Faker<ConsolidateDailyBalanceCommand> _commandFaker;
        private readonly Faker<Transaction> _transactionFaker;
        private readonly Faker<CleanCode.Domain.Entities.DailyBalance> _dailyBalanceFaker;

        public ConsolidateDailyBalanceHandlerTests()
        {
            _mockDailyBalanceRepository = new Mock<IDailyBalanceRepository>();
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ConsolidateDailyBalanceHandler>>();
            _mockMessageService = new Mock<IMessageService>();
            _handler = new ConsolidateDailyBalanceHandler(
                _mockDailyBalanceRepository.Object,
                _mockTransactionRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockMessageService.Object);

            _commandFaker = new Faker<ConsolidateDailyBalanceCommand>()
                .CustomInstantiator(f => new ConsolidateDailyBalanceCommand(f.Date.Recent()));

            _transactionFaker = new Faker<Transaction>()
                .RuleFor(t => t.Id, f => f.Random.Guid())
                .RuleFor(t => t.Description, f => f.Commerce.ProductName())
                .RuleFor(t => t.Amount, f => f.Random.Decimal(1, 1000))
                .RuleFor(t => t.Type, f => f.PickRandom(new[] { TransactionType.Credit, TransactionType.Debit }))
                .RuleFor(t => t.TransactionDate, f => f.Date.Recent())
                .RuleFor(t => t.Category, f => f.Commerce.Categories(1).FirstOrDefault())
                .RuleFor(t => t.Notes, f => f.Lorem.Sentence());

            _dailyBalanceFaker = new Faker<CleanCode.Domain.Entities.DailyBalance>()
                .RuleFor(db => db.Id, f => f.Random.Guid())
                .RuleFor(db => db.Date, f => f.Date.Recent())
                .RuleFor(db => db.OpeningBalance, f => f.Random.Decimal(0, 10000))
                .RuleFor(db => db.TotalCredits, f => f.Random.Decimal(0, 5000))
                .RuleFor(db => db.TotalDebits, f => f.Random.Decimal(0, 3000))
                .RuleFor(db => db.ClosingBalance, f => f.Random.Decimal(0, 10000))
                .RuleFor(db => db.CreditTransactionCount, f => f.Random.Int(0, 10))
                .RuleFor(db => db.DebitTransactionCount, f => f.Random.Int(0, 10))
                .RuleFor(db => db.TotalTransactionCount, f => f.Random.Int(0, 20))
                .RuleFor(db => db.LastUpdated, f => f.Date.Recent());
        }

        [Fact]
        public async Task Handle_WithNewDailyBalance_ShouldCreateNewBalance()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transactions = _transactionFaker.Generate(3);
            var dailyBalance = _dailyBalanceFaker.Generate();
            var result = new ConsolidateDailyBalanceResult
            {
                Id = dailyBalance.Id,
                Date = dailyBalance.Date,
                OpeningBalance = dailyBalance.OpeningBalance,
                TotalCredits = dailyBalance.TotalCredits,
                TotalDebits = dailyBalance.TotalDebits,
                ClosingBalance = dailyBalance.ClosingBalance,
                WasCreated = true
            };

            _mockTransactionRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);
            _mockDailyBalanceRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CleanCode.Domain.Entities.DailyBalance?)null);
            _mockDailyBalanceRepository.Setup(x => x.GetLastAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((CleanCode.Domain.Entities.DailyBalance?)null);
            _mockDailyBalanceRepository.Setup(x => x.CreateOrUpdateAsync(It.IsAny<CleanCode.Domain.Entities.DailyBalance>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dailyBalance);
            _mockMapper.Setup(x => x.Map<ConsolidateDailyBalanceResult>(dailyBalance)).Returns(result);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(dailyBalance.Id);
            response.WasCreated.Should().BeTrue();

            _mockTransactionRepository.Verify(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()), Times.Once);
            _mockDailyBalanceRepository.Verify(x => x.CreateOrUpdateAsync(It.IsAny<CleanCode.Domain.Entities.DailyBalance>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithExistingDailyBalance_ShouldUpdateExistingBalance()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transactions = _transactionFaker.Generate(2);
            var existingBalance = _dailyBalanceFaker.Generate();
            var result = new ConsolidateDailyBalanceResult
            {
                Id = existingBalance.Id,
                Date = existingBalance.Date,
                WasCreated = false
            };

            _mockTransactionRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);
            _mockDailyBalanceRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBalance);
            _mockDailyBalanceRepository.Setup(x => x.CreateOrUpdateAsync(It.IsAny<CleanCode.Domain.Entities.DailyBalance>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBalance);
            _mockMapper.Setup(x => x.Map<ConsolidateDailyBalanceResult>(existingBalance)).Returns(result);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(existingBalance.Id);
            response.WasCreated.Should().BeFalse();

            _mockDailyBalanceRepository.Verify(x => x.CreateOrUpdateAsync(It.IsAny<CleanCode.Domain.Entities.DailyBalance>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNoTransactions_ShouldCreateBalanceWithZeroValues()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var emptyTransactions = new List<Transaction>();
            var dailyBalance = _dailyBalanceFaker.Generate();
            var result = new ConsolidateDailyBalanceResult
            {
                Id = dailyBalance.Id,
                Date = dailyBalance.Date,
                TotalCredits = 0,
                TotalDebits = 0,
                CreditTransactionCount = 0,
                DebitTransactionCount = 0,
                TotalTransactionCount = 0,
                WasCreated = true
            };

            _mockTransactionRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyTransactions);
            _mockDailyBalanceRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CleanCode.Domain.Entities.DailyBalance?)null);
            _mockDailyBalanceRepository.Setup(x => x.GetLastAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((CleanCode.Domain.Entities.DailyBalance?)null);
            _mockDailyBalanceRepository.Setup(x => x.CreateOrUpdateAsync(It.IsAny<CleanCode.Domain.Entities.DailyBalance>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dailyBalance);
            _mockMapper.Setup(x => x.Map<ConsolidateDailyBalanceResult>(dailyBalance)).Returns(result);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.TotalCredits.Should().Be(0);
            response.TotalDebits.Should().Be(0);
            response.CreditTransactionCount.Should().Be(0);
            response.DebitTransactionCount.Should().Be(0);
            response.TotalTransactionCount.Should().Be(0);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldLogInformation()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transactions = _transactionFaker.Generate(1);
            var dailyBalance = _dailyBalanceFaker.Generate();
            var result = new ConsolidateDailyBalanceResult { Id = dailyBalance.Id };

            _mockTransactionRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);
            _mockDailyBalanceRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CleanCode.Domain.Entities.DailyBalance?)null);
            _mockDailyBalanceRepository.Setup(x => x.GetLastAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((CleanCode.Domain.Entities.DailyBalance?)null);
            _mockDailyBalanceRepository.Setup(x => x.CreateOrUpdateAsync(It.IsAny<CleanCode.Domain.Entities.DailyBalance>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dailyBalance);
            _mockMapper.Setup(x => x.Map<ConsolidateDailyBalanceResult>(dailyBalance)).Returns(result);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Iniciando consolidação do saldo diário")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldSendMessageToQueue()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var transactions = _transactionFaker.Generate(1);
            var dailyBalance = _dailyBalanceFaker.Generate();
            var result = new ConsolidateDailyBalanceResult { Id = dailyBalance.Id };

            _mockTransactionRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);
            _mockDailyBalanceRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CleanCode.Domain.Entities.DailyBalance?)null);
            _mockDailyBalanceRepository.Setup(x => x.GetLastAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((CleanCode.Domain.Entities.DailyBalance?)null);
            _mockDailyBalanceRepository.Setup(x => x.CreateOrUpdateAsync(It.IsAny<CleanCode.Domain.Entities.DailyBalance>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dailyBalance);
            _mockMapper.Setup(x => x.Map<ConsolidateDailyBalanceResult>(dailyBalance)).Returns(result);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockMessageService.Verify(x => x.SendMessageAsync(
                It.IsAny<object>(),
                "daily_balance_consolidated",
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
