using AutoMapper;
using CleanCode.Application.DailyBalance.GetDailyBalance;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Repositories.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Bogus;

namespace CleanCode.Tests.UnitTests.Handlers.DailyBalance.GetDailyBalance
{
    public class GetDailyBalanceHandlerTests
    {
        private readonly Mock<IDailyBalanceRepository> _mockDailyBalanceRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<GetDailyBalanceHandler>> _mockLogger;
        private readonly GetDailyBalanceHandler _handler;
        private readonly Faker<GetDailyBalanceCommand> _commandFaker;
        private readonly Faker<CleanCode.Domain.Entities.DailyBalance> _dailyBalanceFaker;

        public GetDailyBalanceHandlerTests()
        {
            _mockDailyBalanceRepository = new Mock<IDailyBalanceRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<GetDailyBalanceHandler>>();
            _handler = new GetDailyBalanceHandler(_mockDailyBalanceRepository.Object, _mockMapper.Object, _mockLogger.Object);

            _commandFaker = new Faker<GetDailyBalanceCommand>()
                .CustomInstantiator(f => new GetDailyBalanceCommand(f.Date.Recent()));

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
        public async Task Handle_WithValidDate_ShouldReturnDailyBalance()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var dailyBalance = _dailyBalanceFaker.Generate();
            var result = new GetDailyBalanceResult
            {
                Id = dailyBalance.Id,
                Date = dailyBalance.Date,
                OpeningBalance = dailyBalance.OpeningBalance,
                TotalCredits = dailyBalance.TotalCredits,
                TotalDebits = dailyBalance.TotalDebits,
                ClosingBalance = dailyBalance.ClosingBalance,
                CreditTransactionCount = dailyBalance.CreditTransactionCount,
                DebitTransactionCount = dailyBalance.DebitTransactionCount,
                TotalTransactionCount = dailyBalance.TotalTransactionCount,
                LastUpdated = dailyBalance.LastUpdated
            };

            _mockDailyBalanceRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dailyBalance);
            _mockMapper.Setup(x => x.Map<GetDailyBalanceResult>(dailyBalance)).Returns(result);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(dailyBalance.Id);
            response.Date.Should().Be(dailyBalance.Date);
            response.OpeningBalance.Should().Be(dailyBalance.OpeningBalance);
            response.TotalCredits.Should().Be(dailyBalance.TotalCredits);
            response.TotalDebits.Should().Be(dailyBalance.TotalDebits);
            response.ClosingBalance.Should().Be(dailyBalance.ClosingBalance);

            _mockDailyBalanceRepository.Verify(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentDate_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var command = _commandFaker.Generate();

            _mockDailyBalanceRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CleanCode.Domain.Entities.DailyBalance?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain($"Saldo diário para data {command.Date:yyyy-MM-dd} não encontrado");
        }

        [Fact]
        public async Task Handle_WithValidDate_ShouldLogInformation()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var dailyBalance = _dailyBalanceFaker.Generate();
            var result = new GetDailyBalanceResult { Id = dailyBalance.Id };

            _mockDailyBalanceRepository.Setup(x => x.GetByDateAsync(command.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dailyBalance);
            _mockMapper.Setup(x => x.Map<GetDailyBalanceResult>(dailyBalance)).Returns(result);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Buscando saldo diário para data")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
