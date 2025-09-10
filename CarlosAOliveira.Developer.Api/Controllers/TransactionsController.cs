using CarlosAOliveira.Developer.Api.DTOs.Transactions;
using CarlosAOliveira.Developer.Application.Commands.Transaction;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.Queries.Transaction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarlosAOliveira.Developer.Api.Controllers
{
    /// <summary>
    /// Transactions management controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(IMediator mediator, ILogger<TransactionsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a transaction by ID
        /// </summary>
        /// <param name="id">Transaction ID</param>
        /// <returns>Transaction details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Application.DTOs.Transaction.TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTransaction(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting transaction with ID: {TransactionId}", id);

                var query = new GetTransactionByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new { message = "Transaction not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction with ID: {TransactionId}", id);
                return StatusCode(500, new { message = "An error occurred while getting the transaction" });
            }
        }

        /// <summary>
        /// Gets transactions by merchant ID
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of transactions</returns>
        [HttpGet("merchant/{merchantId}")]
        [ProducesResponseType(typeof(PagedResult<Application.DTOs.Transaction.TransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTransactionsByMerchant(
            Guid merchantId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting transactions for merchant: {MerchantId} - Page: {PageNumber}, Size: {PageSize}", 
                    merchantId, pageNumber, pageSize);

                var query = new GetTransactionsByMerchantQuery(merchantId, null, null, pageNumber, pageSize);
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transactions for merchant: {MerchantId}", merchantId);
                return StatusCode(500, new { message = "An error occurred while getting transactions" });
            }
        }

        /// <summary>
        /// Creates a new transaction
        /// </summary>
        /// <param name="request">Create transaction request</param>
        /// <returns>Created transaction</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Application.DTOs.Transaction.TransactionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
        {
            try
            {
                _logger.LogInformation("Creating transaction for merchant: {MerchantId}", request.MerchantId);

                var command = new CreateTransactionCommand
                {
                    MerchantId = request.MerchantId,
                    Amount = request.Amount,
                    Type = request.Type,
                    Description = request.Description
                };

                var result = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetTransaction), new { id = result.Data?.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction for merchant: {MerchantId}", request.MerchantId);
                return StatusCode(500, new { message = "An error occurred while creating the transaction" });
            }
        }
    }
}
