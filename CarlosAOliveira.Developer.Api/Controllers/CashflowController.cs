using CarlosAOliveira.Developer.Api.DTOs.Cashflow;
using CarlosAOliveira.Developer.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarlosAOliveira.Developer.Api.Controllers
{
    /// <summary>
    /// Cashflow controller for managing financial transactions and daily balances
    /// </summary>
    [Route("api/cashflow")]
    public class CashflowController : BaseController
    {
        private readonly ICashflowApplicationService _cashflowService;
        private readonly ILogger<CashflowController> _logger;

        public CashflowController(ICashflowApplicationService cashflowService, ILogger<CashflowController> logger)
        {
            _cashflowService = cashflowService;
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

                // Map API DTO to Application DTO
                var applicationRequest = new Application.DTOs.Cashflow.CreateCashflowTransactionRequest
                {
                    Date = request.Date,
                    Amount = request.Amount,
                    Type = request.Type,
                    Category = request.Category,
                    Description = request.Description
                };

                var result = await _cashflowService.CreateTransactionAsync(applicationRequest);

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
                var result = await _cashflowService.GetTransactionAsync(id);

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

                var result = await _cashflowService.GetDailyBalanceAsync(parsedDate);

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

        /// <summary>
        /// Gets daily summary by merchant ID and date
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="date">Date</param>
        /// <returns>Daily summary details</returns>
        [HttpGet("merchants/{merchantId}/daily-summary")]
        [ProducesResponseType(typeof(DailySummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDailySummary(Guid merchantId, [FromQuery] string date)
        {
            try
            {
                if (!DateOnly.TryParse(date, out var parsedDate))
                {
                    return BadRequest(new { message = "Invalid date format. Use YYYY-MM-DD" });
                }

                var result = await _cashflowService.GetDailySummaryAsync(merchantId, parsedDate.ToDateTime(TimeOnly.MinValue));

                if (!result.Success)
                {
                    return NotFound(new { message = result.Message });
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily summary for merchant: {MerchantId} on date: {Date}", merchantId, date);
                return StatusCode(500, new { message = "An error occurred while getting the daily summary" });
            }
        }

        /// <summary>
        /// Gets period summary for a merchant
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Period summary</returns>
        [HttpGet("merchants/{merchantId}/period-summary")]
        [ProducesResponseType(typeof(PeriodSummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPeriodSummary(
            Guid merchantId,
            [FromQuery] string startDate,
            [FromQuery] string endDate)
        {
            try
            {
                if (!DateOnly.TryParse(startDate, out var parsedStartDate) || 
                    !DateOnly.TryParse(endDate, out var parsedEndDate))
                {
                    return BadRequest(new { message = "Invalid date format. Use YYYY-MM-DD" });
                }

                if (parsedStartDate >= parsedEndDate)
                {
                    return BadRequest(new { message = "Start date must be before end date" });
                }

                var result = await _cashflowService.GetPeriodSummaryAsync(merchantId, parsedStartDate.ToDateTime(TimeOnly.MinValue), parsedEndDate.ToDateTime(TimeOnly.MinValue));

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting period summary for merchant: {MerchantId}", merchantId);
                return StatusCode(500, new { message = "An error occurred while getting the period summary" });
            }
        }
    }
}
