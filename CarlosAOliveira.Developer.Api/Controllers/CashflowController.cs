using CarlosAOliveira.Developer.Api.DTOs.Cashflow;
using CarlosAOliveira.Developer.Application.Commands.Cashflow;
using CarlosAOliveira.Developer.Application.Queries.Cashflow;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarlosAOliveira.Developer.Api.Controllers
{
    /// <summary>
    /// Cashflow controller for managing financial transactions and balances
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class CashflowController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CashflowController> _logger;

        public CashflowController(IMediator mediator, ILogger<CashflowController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new transaction
        /// </summary>
        /// <param name="request">Transaction creation request</param>
        /// <returns>Created transaction</returns>
        [HttpPost("transactions")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateCashflowTransactionRequest request)
        {
            try
            {
                // Check for idempotency key
                var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
                if (string.IsNullOrEmpty(idempotencyKey))
                {
                    return BadRequest(new { message = "Idempotency-Key header is required" });
                }

                _logger.LogInformation("Creating transaction for date: {Date}, amount: {Amount}, type: {Type}", 
                    request.Date, request.Amount, request.Type);

                var command = new CreateTransactionCommand
                {
                    Date = request.Date,
                    Amount = request.Amount,
                    Type = request.Type,
                    Category = request.Category,
                    Description = request.Description
                };

                var result = await _mediator.Send(command);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message, errors = result.Errors });
                }

                _logger.LogInformation("Transaction created successfully: {TransactionId}", result.Data!.Id);
                return CreatedAtAction(nameof(GetTransaction), new { id = result.Data.Id }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
                return StatusCode(500, new { message = "An error occurred while creating the transaction" });
            }
        }

        /// <summary>
        /// Gets a transaction by ID
        /// </summary>
        /// <param name="id">Transaction ID</param>
        /// <returns>Transaction details</returns>
        [HttpGet("transactions/{id}")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTransaction(Guid id)
        {
            try
            {
                var query = new GetTransactionByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.Success)
                {
                    return NotFound(new { message = result.Message });
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction {TransactionId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the transaction" });
            }
        }

        /// <summary>
        /// Gets the consolidated daily balance for a specific date
        /// </summary>
        /// <param name="date">Date to get balance for (YYYY-MM-DD format)</param>
        /// <returns>Daily balance</returns>
        [HttpGet("consolidated/daily")]
        [ProducesResponseType(typeof(DailyBalanceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDailyBalance([FromQuery] string date)
        {
            try
            {
                if (!DateOnly.TryParse(date, out var parsedDate))
                {
                    return BadRequest(new { message = "Invalid date format. Use YYYY-MM-DD" });
                }

                var query = new GetDailyBalanceQuery { Date = parsedDate };
                var result = await _mediator.Send(query);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily balance for date {Date}", date);
                return StatusCode(500, new { message = "An error occurred while retrieving the daily balance" });
            }
        }
    }
}
