using CarlosAOliveira.Developer.Application.Queries.DailySummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarlosAOliveira.Developer.Api.Controllers
{
    /// <summary>
    /// Daily summaries management controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class DailySummariesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DailySummariesController> _logger;

        public DailySummariesController(IMediator mediator, ILogger<DailySummariesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets daily summary by merchant ID and date
        /// </summary>
        /// <param name="merchantId">Merchant ID</param>
        /// <param name="date">Date</param>
        /// <returns>Daily summary details</returns>
        [HttpGet("merchant/{merchantId}/date/{date}")]
        [ProducesResponseType(typeof(Application.DTOs.DailySummary.DailySummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDailySummary(Guid merchantId, DateTime date)
        {
            try
            {
                _logger.LogInformation("Getting daily summary for merchant: {MerchantId} on date: {Date}", merchantId, date);

                var query = new GetDailySummaryQuery(merchantId, date);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new { message = "Daily summary not found" });
                }

                return Ok(result);
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
        [HttpGet("merchant/{merchantId}/period")]
        [ProducesResponseType(typeof(Application.DTOs.DailySummary.PeriodSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPeriodSummary(
            Guid merchantId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                _logger.LogInformation("Getting period summary for merchant: {MerchantId} from {StartDate} to {EndDate}", 
                    merchantId, startDate, endDate);

                if (startDate >= endDate)
                {
                    return BadRequest(new { message = "Start date must be before end date" });
                }

                var query = new GetPeriodSummaryQuery(merchantId, startDate, endDate);
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting period summary for merchant: {MerchantId}", merchantId);
                return StatusCode(500, new { message = "An error occurred while getting the period summary" });
            }
        }
    }
}
