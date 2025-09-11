using AutoMapper;
using CarlosAOliveira.Developer.Application.Commands.Cashflow;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Cashflow;
using CarlosAOliveira.Developer.Application.Handlers.Cashflow;
using CarlosAOliveira.Developer.Application.Mappings;
using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Domain.Events;
using CarlosAOliveira.Developer.Domain.Repositories;
using Moq;
using Xunit;

namespace CarlosAOliveira.Developer.Tests.Application.Handlers
{
    public class CreateTransactionCommandHandlerTests
    {
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IEventQueue> _eventQueueMock;
        private readonly IMapper _mapper;

        public CreateTransactionCommandHandlerTests()
        {
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _eventQueueMock = new Mock<IEventQueue>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<CashflowMappingProfile>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateTransaction()
        {
            // Arrange
            var command = new CreateTransactionCommand
            {
                Date = DateOnly.FromDateTime(DateTime.Today),
                Amount = 100.50m,
                Type = "Credit",
                Category = "Sales",
                Description = "Test transaction"
            };

            _transactionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Transaction t, CancellationToken ct) => t);

            _transactionRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _eventQueueMock.Setup(x => x.PublishAsync(It.IsAny<TransactionCreatedEvent>()))
                .Returns(Task.CompletedTask);

            var handler = new CreateTransactionCommandHandler(
                _transactionRepositoryMock.Object,
                _eventQueueMock.Object,
                _mapper
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(command.Amount, result.Data.Amount);
            Assert.Equal(command.Type, result.Data.Type);
            Assert.Equal(command.Category, result.Data.Category);
            Assert.Equal(command.Description, result.Data.Description);

            _transactionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
            _transactionRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _eventQueueMock.Verify(x => x.PublishAsync(It.IsAny<TransactionCreatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidTransactionType_ShouldReturnError()
        {
            // Arrange
            var command = new CreateTransactionCommand
            {
                Date = DateOnly.FromDateTime(DateTime.Today),
                Amount = 100.50m,
                Type = "InvalidType",
                Category = "Sales",
                Description = "Test transaction"
            };

            var handler = new CreateTransactionCommandHandler(
                _transactionRepositoryMock.Object,
                _eventQueueMock.Object,
                _mapper
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Invalid transaction type", result.Message);
            _transactionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithDebitTransaction_ShouldCreateTransaction()
        {
            // Arrange
            var command = new CreateTransactionCommand
            {
                Date = DateOnly.FromDateTime(DateTime.Today),
                Amount = 50.25m,
                Type = "Debit",
                Category = "Expenses",
                Description = "Test debit transaction"
            };

            _transactionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Transaction t, CancellationToken ct) => t);

            _transactionRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _eventQueueMock.Setup(x => x.PublishAsync(It.IsAny<TransactionCreatedEvent>()))
                .Returns(Task.CompletedTask);

            var handler = new CreateTransactionCommandHandler(
                _transactionRepositoryMock.Object,
                _eventQueueMock.Object,
                _mapper
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Debit", result.Data.Type);
        }
    }
}
