using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProposalPilot.Application.Features.Users.Commands.SendEmailVerification;
using ProposalPilot.Application.Features.Users.Commands.VerifyEmail;
using ProposalPilot.Application.Interfaces;

namespace ProposalPilot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<EmailController> _logger;

    public EmailController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        ILogger<EmailController> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Send email verification link to current user
    /// </summary>
    [Authorize]
    [HttpPost("send-verification")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendVerification()
    {
        try
        {
            if (!_currentUserService.UserId.HasValue)
            {
                _logger.LogWarning("Unauthorized access to send verification");
                return Unauthorized(new { message = "User not authenticated" });
            }

            var command = new SendEmailVerificationCommand(_currentUserService.UserId.Value);
            var success = await _mediator.Send(command);

            if (!success)
            {
                _logger.LogWarning("Failed to send verification email for user: {UserId}", _currentUserService.UserId);
                return BadRequest(new { message = "Failed to send verification email" });
            }

            _logger.LogInformation("Verification email sent to user: {UserId}", _currentUserService.UserId);
            return Ok(new { message = "Verification email sent. Please check your email." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending verification email for user: {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An error occurred while sending verification email" });
        }
    }

    /// <summary>
    /// Verify email with token
    /// </summary>
    [HttpPost("verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        try
        {
            var command = new VerifyEmailCommand(request.UserId, request.Token);
            var success = await _mediator.Send(command);

            if (!success)
            {
                _logger.LogWarning("Failed to verify email for user: {UserId}", request.UserId);
                return BadRequest(new { message = "Invalid or expired verification token" });
            }

            _logger.LogInformation("Email verified successfully for user: {UserId}", request.UserId);
            return Ok(new { message = "Email verified successfully!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying email for user: {UserId}", request.UserId);
            return StatusCode(500, new { message = "An error occurred during email verification" });
        }
    }
}

public record VerifyEmailRequest(Guid UserId, string Token);
