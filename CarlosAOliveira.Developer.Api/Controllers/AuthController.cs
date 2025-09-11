using CarlosAOliveira.Developer.Api.DTOs.Auth;
using CarlosAOliveira.Developer.Api.Services;
using CarlosAOliveira.Developer.Application.Commands.User;
using CarlosAOliveira.Developer.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarlosAOliveira.Developer.Api.Controllers
{
    /// <summary>
    /// Authentication controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Domain.Entities.User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<Domain.Entities.User> userManager,
            IJwtService jwtService,
            IMediator mediator,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user and returns JWT tokens
        /// </summary>
        /// <param name="request">Login request</param>
        /// <returns>JWT tokens and user information</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", request.Email);

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null || user.Status != UserStatus.Active)
                {
                    _logger.LogWarning("Login failed for email: {Email} - User not found or inactive", request.Email);
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                // In a real application, you would verify the password hash here
                // For now, we'll assume the password is correct
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var response = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60), // Should come from JWT settings
                    User = new UserInfo
                    {
                        Id = user.Id,
                        Name = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        Role = user.Role.ToString(),
                        Status = user.Status.ToString()
                    }
                };

                _logger.LogInformation("Login successful for user: {UserId}", user.Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Refreshes an access token using a refresh token
        /// </summary>
        /// <param name="request">Refresh token request</param>
        /// <returns>New JWT tokens</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                _logger.LogInformation("Refresh token request received");

                // In a real application, you would validate the refresh token against a database
                // For now, we'll extract the user ID from the current token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Invalid refresh token" });
                }

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null || user.Status != UserStatus.Active)
                {
                    return Unauthorized(new { message = "User not found or inactive" });
                }

                var accessToken = _jwtService.GenerateAccessToken(user);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                var response = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                    User = new UserInfo
                    {
                        Id = user.Id,
                        Name = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        Role = user.Role.ToString(),
                        Status = user.Status.ToString()
                    }
                };

                _logger.LogInformation("Token refresh successful for user: {UserId}", user.Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new { message = "An error occurred during token refresh" });
            }
        }

        /// <summary>
        /// Gets the current user's information
        /// </summary>
        /// <returns>Current user information</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var userInfo = new UserInfo
                {
                    Id = user.Id,
                    Name = user.UserName,  
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Status = user.Status.ToString()
                };

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, new { message = "An error occurred while getting user information" });
            }
        }

        /// <summary>
        /// Creates a new user account
        /// </summary>
        /// <param name="request">Create user request</param>
        /// <returns>Created user and JWT tokens</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
        {
            try
            {
                _logger.LogInformation("Registering new user with email: {Email}", request.Email);

                var command = new CreateUserCommand
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    Phone = request.Phone
                };

                var result = await _mediator.Send(command);

                if (!result.Success)
                {
                    if (result.Message?.Contains("already exists") == true)
                    {
                        return Conflict(new { message = result.Message });
                    }
                    return BadRequest(new { message = result.Message, errors = result.Errors });
                }

                // Get the created user
                var user = await _userManager.FindByIdAsync(result.Data!.Id.ToString());
                if (user == null)
                {
                    return StatusCode(500, new { message = "User created but could not be retrieved" });
                }

                // Generate JWT tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var response = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                    User = new UserInfo
                    {
                        Id = user.Id,
                        Name = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        Role = user.Role.ToString(),
                        Status = user.Status.ToString()
                    }
                };

                _logger.LogInformation("User registered successfully: {UserId}", user.Id);
                return CreatedAtAction(nameof(GetCurrentUser), new { }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for email: {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }
    }
}
