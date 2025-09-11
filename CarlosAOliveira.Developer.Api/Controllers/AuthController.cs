using CarlosAOliveira.Developer.Api.DTOs.Auth;
using CarlosAOliveira.Developer.Api.Services;
using CarlosAOliveira.Developer.Application.Commands.Merchant;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarlosAOliveira.Developer.Api.Controllers
{
    /// <summary>
    /// Authentication controller for JWT token generation
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IJwtService jwtService,
            IMediator mediator,
            ILogger<AuthController> logger)
        {
            _jwtService = jwtService;
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a merchant and returns JWT token
        /// </summary>
        /// <param name="request">Login request</param>
        /// <returns>JWT token</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid request data", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                _logger.LogInformation("Login attempt for merchant: {MerchantName}", request.MerchantName);

                // For demo purposes, we'll create a merchant if it doesn't exist
                // In a real application, you'd validate against existing merchants
                var createMerchantCommand = new CreateMerchantCommand
                {
                    Name = request.MerchantName,
                    Email = request.Email
                };

                var result = await _mediator.Send(createMerchantCommand);

                if (result.Success && result.Data != null)
                {
                    var token = _jwtService.GenerateAccessToken(result.Data.Id, result.Data.Name);
                    var refreshToken = _jwtService.GenerateRefreshToken();

                    _logger.LogInformation("Login successful for merchant: {MerchantName}", request.MerchantName);

                    return Ok(new LoginResponse
                    {
                        AccessToken = token,
                        RefreshToken = refreshToken,
                        ExpiresIn = 60 * 60, // 1 hour in seconds
                        TokenType = "Bearer",
                        MerchantId = result.Data.Id,
                        MerchantName = result.Data.Name
                    });
                }

                return Unauthorized(new { message = "Invalid merchant credentials" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for merchant: {MerchantName}", request.MerchantName);
                return StatusCode(500, new { message = "An error occurred during authentication" });
            }
        }

        /// <summary>
        /// Generates a new JWT token for testing purposes (no authentication required)
        /// </summary>
        /// <param name="request">Test token request</param>
        /// <returns>JWT token</returns>
        [HttpPost("test-token")]
        [AllowAnonymous]
        public IActionResult GenerateTestToken([FromBody] TestTokenRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid request data", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                _logger.LogInformation("Generating test token for merchant: {MerchantName}", request.MerchantName);

                var merchantId = request.MerchantId ?? Guid.NewGuid();
                var token = _jwtService.GenerateAccessToken(merchantId, request.MerchantName);
                var refreshToken = _jwtService.GenerateRefreshToken();

                return Ok(new LoginResponse
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    ExpiresIn = 60 * 60, // 1 hour in seconds
                    TokenType = "Bearer",
                    MerchantId = merchantId,
                    MerchantName = request.MerchantName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating test token for merchant: {MerchantName}", request.MerchantName);
                return StatusCode(500, new { message = "An error occurred while generating token" });
            }
        }

        /// <summary>
        /// Validates a JWT token
        /// </summary>
        /// <returns>Token validation result</returns>
        [HttpPost("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            try
            {
                var merchantId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var merchantName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(merchantId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                return Ok(new
                {
                    message = "Token is valid",
                    merchantId = Guid.Parse(merchantId),
                    merchantName = merchantName,
                    claims = User.Claims.Select(c => new { c.Type, c.Value })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return StatusCode(500, new { message = "An error occurred while validating token" });
            }
        }
    }
}
