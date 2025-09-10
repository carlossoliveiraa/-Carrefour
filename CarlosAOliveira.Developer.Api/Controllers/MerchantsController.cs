using CarlosAOliveira.Developer.Api.DTOs.Merchants;
using CarlosAOliveira.Developer.Application.Commands.Merchant;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.Queries.Merchant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarlosAOliveira.Developer.Api.Controllers
{
    /// <summary>
    /// Merchants management controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class MerchantsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MerchantsController> _logger;

        public MerchantsController(IMediator mediator, ILogger<MerchantsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets all merchants with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of merchants</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<Application.DTOs.Merchant.MerchantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMerchants(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting merchants - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

                var query = new GetMerchantsQuery { PageNumber = pageNumber, PageSize = pageSize };
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting merchants");
                return StatusCode(500, new { message = "An error occurred while getting merchants" });
            }
        }

        /// <summary>
        /// Gets a merchant by ID
        /// </summary>
        /// <param name="id">Merchant ID</param>
        /// <returns>Merchant details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Application.DTOs.Merchant.MerchantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMerchant(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting merchant with ID: {MerchantId}", id);

                var query = new GetMerchantByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new { message = "Merchant not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting merchant with ID: {MerchantId}", id);
                return StatusCode(500, new { message = "An error occurred while getting the merchant" });
            }
        }

        /// <summary>
        /// Creates a new merchant
        /// </summary>
        /// <param name="request">Create merchant request</param>
        /// <returns>Created merchant</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Application.DTOs.Merchant.MerchantDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateMerchant([FromBody] CreateMerchantRequest request)
        {
            try
            {
                _logger.LogInformation("Creating merchant with email: {Email}", request.Email);

                var command = new CreateMerchantCommand
                {
                    Name = request.Name,
                    Email = request.Email
                };

                var result = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetMerchant), new { id = result.Data?.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating merchant with email: {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred while creating the merchant" });
            }
        }

        /// <summary>
        /// Updates an existing merchant
        /// </summary>
        /// <param name="id">Merchant ID</param>
        /// <param name="request">Update merchant request</param>
        /// <returns>Updated merchant</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Application.DTOs.Merchant.MerchantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateMerchant(Guid id, [FromBody] UpdateMerchantRequest request)
        {
            try
            {
                _logger.LogInformation("Updating merchant with ID: {MerchantId}", id);

                var command = new UpdateMerchantCommand
                {
                    Id = id,
                    Name = request.Name,
                    Email = request.Email
                };

                var result = await _mediator.Send(command);

                if (result == null)
                {
                    return NotFound(new { message = "Merchant not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating merchant with ID: {MerchantId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the merchant" });
            }
        }

        /// <summary>
        /// Deletes a merchant
        /// </summary>
        /// <param name="id">Merchant ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteMerchant(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting merchant with ID: {MerchantId}", id);

                var command = new DeleteMerchantCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new { message = "Merchant not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting merchant with ID: {MerchantId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the merchant" });
            }
        }
    }
}
