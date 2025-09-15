using AutoMapper;
using CleanCode.Api.Common;
using CleanCode.Api.Features.Transactions;
using CleanCode.Api.Features.Transactions.CreateTransaction;
using CleanCode.Api.Features.Transactions.GetTransaction;
using CleanCode.Api.Features.Transactions.GetTransactions;
using CleanCode.Application.Transactions.CreateTransaction;
using CleanCode.Application.Transactions.GetTransaction;
using CleanCode.Application.Transactions.GetTransactions;
using CleanCode.Domain.Enum;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Bogus;

namespace CleanCode.Tests.UnitTests.Controllers
{
    public class TransactionsControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IMapper> _mockMapper;
        private readonly TransactionsController _controller;
        private readonly Faker<CreateTransactionRequest> _createRequestFaker;
        private readonly Faker<GetTransactionsRequest> _getTransactionsRequestFaker;

        public TransactionsControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockMapper = new Mock<IMapper>();
            _controller = new TransactionsController(_mockMediator.Object, _mockMapper.Object);

            _createRequestFaker = new Faker<CreateTransactionRequest>()
                .RuleFor(r => r.Description, f => f.Commerce.ProductName())
                .RuleFor(r => r.Amount, f => f.Random.Decimal(1, 1000))
                .RuleFor(r => r.Type, f => f.PickRandom(new[] { TransactionType.Credit, TransactionType.Debit }))
                .RuleFor(r => r.TransactionDate, f => f.Date.Recent())
                .RuleFor(r => r.Category, f => f.Commerce.Categories(1).FirstOrDefault())
                .RuleFor(r => r.Notes, f => f.Lorem.Sentence());

            _getTransactionsRequestFaker = new Faker<GetTransactionsRequest>()
                .RuleFor(r => r.StartDate, f => f.Date.Recent())
                .RuleFor(r => r.EndDate, f => f.Date.Recent().AddDays(1))
                .RuleFor(r => r.Type, f => f.PickRandom(new[] { TransactionType.Credit, TransactionType.Debit }))
                .RuleFor(r => r.Category, f => f.Commerce.Categories(1).FirstOrDefault())
                .RuleFor(r => r.Page, f => f.Random.Int(1, 10))
                .RuleFor(r => r.PageSize, f => f.Random.Int(5, 20));
        }

        [Fact]
        public async Task CreateTransaction_WithValidRequest_ShouldReturnCreatedResult()
        {
            // Arrange
            var request = _createRequestFaker.Generate();
            var command = new CreateTransactionCommand();
            var result = new CreateTransactionResult
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                Amount = request.Amount,
                Type = request.Type.ToString(),
                TransactionDate = request.TransactionDate,
                Category = request.Category,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };
            var response = new CreateTransactionResponse
            {
                Id = result.Id,
                Description = result.Description,
                Amount = result.Amount,
                Type = result.Type,
                TransactionDate = result.TransactionDate,
                Category = result.Category,
                Notes = result.Notes,
                CreatedAt = result.CreatedAt
            };

            _mockMapper.Setup(x => x.Map<CreateTransactionCommand>(request)).Returns(command);
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(x => x.Map<CreateTransactionResponse>(result)).Returns(response);

            // Act
            var actionResult = await _controller.CreateTransaction(request, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
            var apiResponse = Assert.IsType<ApiResponseWithData<CreateTransactionResponse>>(createdResult.Value);
            
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data!.Id.Should().Be(response.Id);
            apiResponse.Data.Description.Should().Be(response.Description);
        }

        [Fact]
        public async Task CreateTransaction_WithInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateTransactionRequest
            {
                Description = "", // Invalid
                Amount = -1, // Invalid
                Type = TransactionType.None, // Invalid
                TransactionDate = default
            };

            // Act
            var actionResult = await _controller.CreateTransaction(request, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task GetTransaction_WithValidId_ShouldReturnOkResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new GetTransactionCommand(id);
            var result = new GetTransactionResult
            {
                Id = id,
                Description = "Test Transaction",
                Amount = 100.50m,
                Type = "Credit",
                TransactionDate = DateTime.UtcNow,
                Category = "Test Category",
                Notes = "Test Notes",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var response = new GetTransactionResponse
            {
                Id = result.Id,
                Description = result.Description,
                Amount = result.Amount,
                Type = result.Type,
                TransactionDate = result.TransactionDate,
                Category = result.Category,
                Notes = result.Notes,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };

            _mockMapper.Setup(x => x.Map<GetTransactionCommand>(It.IsAny<GetTransactionRequest>())).Returns(command);
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(x => x.Map<GetTransactionResponse>(result)).Returns(response);

            // Act
            var actionResult = await _controller.GetTransaction(id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var apiResponse = Assert.IsType<ApiResponseWithData<GetTransactionResponse>>(okResult.Value);
            
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data!.Id.Should().Be(response.Id);
            apiResponse.Data.Description.Should().Be(response.Description);
        }

        [Fact]
        public async Task GetTransactions_WithValidRequest_ShouldReturnOkResult()
        {
            // Arrange
            var request = _getTransactionsRequestFaker.Generate();
            var command = new GetTransactionsCommand();
            var result = new GetTransactionsResult
            {
                Transactions = new List<GetTransactionsResult.TransactionItem>
                {
                    new GetTransactionsResult.TransactionItem
                    {
                        Id = Guid.NewGuid(),
                        Description = "Test Transaction 1",
                        Amount = 100.50m,
                        Type = "Credit",
                        TransactionDate = DateTime.UtcNow,
                        Category = "Test Category",
                        CreatedAt = DateTime.UtcNow
                    }
                },
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalCount = 1,
                TotalPages = 1
            };
            var response = new GetTransactionsResponse
            {
                Transactions = new List<GetTransactionsResponse.TransactionItem>
                {
                    new GetTransactionsResponse.TransactionItem
                    {
                        Id = result.Transactions.First().Id,
                        Description = result.Transactions.First().Description,
                        Amount = result.Transactions.First().Amount,
                        Type = result.Transactions.First().Type,
                        TransactionDate = result.Transactions.First().TransactionDate,
                        Category = result.Transactions.First().Category,
                        CreatedAt = result.Transactions.First().CreatedAt
                    }
                },
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            };

            _mockMapper.Setup(x => x.Map<GetTransactionsCommand>(request)).Returns(command);
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(x => x.Map<GetTransactionsResponse>(result)).Returns(response);

            // Act
            var actionResult = await _controller.GetTransactions(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var apiResponse = Assert.IsType<ApiResponseWithData<GetTransactionsResponse>>(okResult.Value);
            
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data!.Transactions.Should().HaveCount(1);
            apiResponse.Data.CurrentPage.Should().Be(request.Page);
        }

        [Fact]
        public async Task GetTransactions_WithInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new GetTransactionsRequest
            {
                StartDate = DateTime.UtcNow.AddDays(1), // Future date - invalid
                EndDate = DateTime.UtcNow.AddDays(-1), // End before start - invalid
                Page = -1, // Invalid
                PageSize = 0 // Invalid
            };

            // Act
            var actionResult = await _controller.GetTransactions(request, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            badRequestResult.Value.Should().NotBeNull();
        }
    }
}
