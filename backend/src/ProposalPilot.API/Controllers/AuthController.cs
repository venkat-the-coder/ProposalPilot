using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Shared.DTOs.Auth;

namespace ProposalPilot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ICurrentUserService currentUserService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("Attempting to register user with email: {Email}", request.Email);

            var (user, accessToken, refreshToken) = await _authService.RegisterAsync(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                request.CompanyName);

            _logger.LogInformation("User registered successfully: {UserId}", user.Id);

            return Ok(new AuthResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.CompanyName,
                accessToken,
                refreshToken,
                DateTime.UtcNow.AddMinutes(15)
            ));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed for {Email}: {Message}", request.Email, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var result = await _authService.LoginAsync(request.Email, request.Password);

            if (result == null)
            {
                _logger.LogWarning("Failed login attempt for {Email}", request.Email);
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var (user, accessToken, refreshToken) = result.Value;

            _logger.LogInformation("User logged in successfully: {UserId}", user.Id);

            return Ok(new AuthResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.CompanyName,
                accessToken,
                refreshToken,
                DateTime.UtcNow.AddMinutes(15)
            ));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Login failed for {Email}: {Message}", request.Email, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (result == null)
            {
                _logger.LogWarning("Invalid or expired refresh token");
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            var (accessToken, refreshToken) = result.Value;

            _logger.LogInformation("Token refreshed successfully");

            return Ok(new
            {
                accessToken,
                refreshToken,
                expiresAt = DateTime.UtcNow.AddMinutes(15)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during token refresh");
            return StatusCode(500, new { message = "An error occurred during token refresh" });
        }
    }

    /// <summary>
    /// Logout current user (revoke refresh token)
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            if (_currentUserService.UserId.HasValue)
            {
                await _authService.RevokeTokenAsync(_currentUserService.UserId.Value);
                _logger.LogInformation("User logged out successfully: {UserId}", _currentUserService.UserId);
            }

            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An error occurred during logout" });
        }
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            userId = _currentUserService.UserId,
            email = _currentUserService.Email,
            isAuthenticated = _currentUserService.IsAuthenticated
        });
    }
}
