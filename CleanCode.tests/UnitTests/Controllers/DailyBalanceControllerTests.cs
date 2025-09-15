using AutoMapper;
using CleanCode.Api.Common;
using CleanCode.Api.Features.DailyBalance;
using CleanCode.Api.Features.DailyBalance.ConsolidateDailyBalance;
using CleanCode.Api.Features.DailyBalance.GetDailyBalance;
using CleanCode.Application.DailyBalance.ConsolidateDailyBalance;
using CleanCode.Application.DailyBalance.GetDailyBalance;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Bogus;

namespace CleanCode.Tests.UnitTests.Controllers
{
    public class DailyBalanceControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DailyBalanceController _controller;
        private readonly Faker<ConsolidateDailyBalanceRequest> _consolidateRequestFaker;

        public DailyBalanceControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockMapper = new Mock<IMapper>();
            _controller = new DailyBalanceController(_mockMediator.Object, _mockMapper.Object);

            _consolidateRequestFaker = new Faker<ConsolidateDailyBalanceRequest>()
                .RuleFor(r => r.Date, f => f.Date.Recent());
        }

        [Fact]
        public async Task GetDailyBalance_WithValidDate_ShouldReturnOkResult()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var command = new GetDailyBalanceCommand(date);
            var result = new GetDailyBalanceResult
            {
                Id = Guid.NewGuid(),
                Date = date,
                OpeningBalance = 1000.00m,
                TotalCredits = 500.00m,
                TotalDebits = 200.00m,
                ClosingBalance = 1300.00m,
                CreditTransactionCount = 2,
                DebitTransactionCount = 1,
                TotalTransactionCount = 3,
                LastUpdated = DateTime.UtcNow
            };
            var response = new GetDailyBalanceResponse
            {
                Id = result.Id,
                Date = result.Date,
                OpeningBalance = result.OpeningBalance,
                TotalCredits = result.TotalCredits,
                TotalDebits = result.TotalDebits,
                ClosingBalance = result.ClosingBalance,
                CreditTransactionCount = result.CreditTransactionCount,
                DebitTransactionCount = result.DebitTransactionCount,
                TotalTransactionCount = result.TotalTransactionCount,
                LastUpdated = result.LastUpdated
            };

            _mockMapper.Setup(x => x.Map<GetDailyBalanceCommand>(It.IsAny<GetDailyBalanceRequest>())).Returns(command);
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(x => x.Map<GetDailyBalanceResponse>(result)).Returns(response);

            // Act
            var actionResult = await _controller.GetDailyBalance(date, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var apiResponse = Assert.IsType<ApiResponseWithData<GetDailyBalanceResponse>>(okResult.Value);
            
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data!.Id.Should().Be(response.Id);
            apiResponse.Data.Date.Should().Be(response.Date);
            apiResponse.Data.OpeningBalance.Should().Be(response.OpeningBalance);
            apiResponse.Data.TotalCredits.Should().Be(response.TotalCredits);
            apiResponse.Data.TotalDebits.Should().Be(response.TotalDebits);
            apiResponse.Data.ClosingBalance.Should().Be(response.ClosingBalance);
        }

        [Fact]
        public async Task GetDailyBalance_WithInvalidDate_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidDate = default(DateTime);

            // Act
            var actionResult = await _controller.GetDailyBalance(invalidDate, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task ConsolidateDailyBalance_WithValidRequest_ShouldReturnOkResult()
        {
            // Arrange
            var request = _consolidateRequestFaker.Generate();
            var command = new ConsolidateDailyBalanceCommand(request.Date);
            var result = new ConsolidateDailyBalanceResult
            {
                Id = Guid.NewGuid(),
                Date = request.Date,
                OpeningBalance = 1000.00m,
                TotalCredits = 500.00m,
                TotalDebits = 200.00m,
                ClosingBalance = 1300.00m,
                CreditTransactionCount = 2,
                DebitTransactionCount = 1,
                TotalTransactionCount = 3,
                WasCreated = true
            };
            var response = new ConsolidateDailyBalanceResponse
            {
                Id = result.Id,
                Date = result.Date,
                OpeningBalance = result.OpeningBalance,
                TotalCredits = result.TotalCredits,
                TotalDebits = result.TotalDebits,
                ClosingBalance = result.ClosingBalance,
                CreditTransactionCount = result.CreditTransactionCount,
                DebitTransactionCount = result.DebitTransactionCount,
                TotalTransactionCount = result.TotalTransactionCount,
                WasCreated = result.WasCreated
            };

            _mockMapper.Setup(x => x.Map<ConsolidateDailyBalanceCommand>(request)).Returns(command);
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(x => x.Map<ConsolidateDailyBalanceResponse>(result)).Returns(response);

            // Act
            var actionResult = await _controller.ConsolidateDailyBalance(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var apiResponse = Assert.IsType<ApiResponseWithData<ConsolidateDailyBalanceResponse>>(okResult.Value);
            
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data!.Id.Should().Be(response.Id);
            apiResponse.Data.Date.Should().Be(response.Date);
            apiResponse.Data.WasCreated.Should().BeTrue();
        }

        [Fact]
        public async Task ConsolidateDailyBalance_WithInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new ConsolidateDailyBalanceRequest
            {
                Date = default(DateTime) // Invalid
            };

            // Act
            var actionResult = await _controller.ConsolidateDailyBalance(request, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task ConsolidateDailyBalance_WithExistingBalance_ShouldReturnUpdatedBalance()
        {
            // Arrange
            var request = _consolidateRequestFaker.Generate();
            var command = new ConsolidateDailyBalanceCommand(request.Date);
            var result = new ConsolidateDailyBalanceResult
            {
                Id = Guid.NewGuid(),
                Date = request.Date,
                OpeningBalance = 1000.00m,
                TotalCredits = 300.00m,
                TotalDebits = 150.00m,
                ClosingBalance = 1150.00m,
                CreditTransactionCount = 1,
                DebitTransactionCount = 1,
                TotalTransactionCount = 2,
                WasCreated = false // Existing balance was updated
            };
            var response = new ConsolidateDailyBalanceResponse
            {
                Id = result.Id,
                Date = result.Date,
                OpeningBalance = result.OpeningBalance,
                TotalCredits = result.TotalCredits,
                TotalDebits = result.TotalDebits,
                ClosingBalance = result.ClosingBalance,
                CreditTransactionCount = result.CreditTransactionCount,
                DebitTransactionCount = result.DebitTransactionCount,
                TotalTransactionCount = result.TotalTransactionCount,
                WasCreated = result.WasCreated
            };

            _mockMapper.Setup(x => x.Map<ConsolidateDailyBalanceCommand>(request)).Returns(command);
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(x => x.Map<ConsolidateDailyBalanceResponse>(result)).Returns(response);

            // Act
            var actionResult = await _controller.ConsolidateDailyBalance(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var apiResponse = Assert.IsType<ApiResponseWithData<ConsolidateDailyBalanceResponse>>(okResult.Value);
            
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data!.WasCreated.Should().BeFalse();
        }
    }
}
