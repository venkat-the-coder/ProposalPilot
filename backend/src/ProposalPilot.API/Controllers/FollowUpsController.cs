namespace ProposalPilot.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Infrastructure.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FollowUpsController : ControllerBase
{
    private readonly IFollowUpService _followUpService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<FollowUpsController> _logger;

    public FollowUpsController(
        IFollowUpService followUpService,
        ICurrentUserService currentUserService,
        ILogger<FollowUpsController> logger)
    {
        _followUpService = followUpService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Get all follow-ups for a proposal
    /// </summary>
    [HttpGet("proposal/{proposalId}")]
    public async Task<ActionResult<List<FollowUpDto>>> GetFollowUps(Guid proposalId)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var followUps = await _followUpService.GetFollowUpsAsync(proposalId, _currentUserService.UserId.Value);
            return Ok(followUps);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting follow-ups for proposal {ProposalId}", proposalId);
            return StatusCode(500, new { message = "An error occurred while getting follow-ups" });
        }
    }

    /// <summary>
    /// Schedule a new follow-up for a proposal
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ScheduleFollowUpResult>> ScheduleFollowUp([FromBody] ScheduleFollowUpApiRequest request)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var serviceRequest = new ScheduleFollowUpRequest(
                request.ProposalId,
                _currentUserService.UserId.Value,
                request.ScheduledFor,
                request.CustomMessage,
                false // Not automatic
            );

            var result = await _followUpService.ScheduleFollowUpAsync(serviceRequest);

            if (result.Success)
            {
                _logger.LogInformation(
                    "Follow-up scheduled for proposal {ProposalId} by user {UserId} at {ScheduledFor}",
                    request.ProposalId, _currentUserService.UserId, request.ScheduledFor);
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling follow-up for proposal {ProposalId}", request.ProposalId);
            return StatusCode(500, new { message = "An error occurred while scheduling the follow-up" });
        }
    }

    /// <summary>
    /// Cancel a scheduled follow-up
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelFollowUp(Guid id)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var success = await _followUpService.CancelFollowUpAsync(id, _currentUserService.UserId.Value);

            if (success)
            {
                _logger.LogInformation("Follow-up {FollowUpId} cancelled by user {UserId}",
                    id, _currentUserService.UserId);
                return NoContent();
            }

            return NotFound(new { message = "Follow-up not found or cannot be cancelled" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling follow-up {FollowUpId}", id);
            return StatusCode(500, new { message = "An error occurred while cancelling the follow-up" });
        }
    }
}

/// <summary>
/// Request model for scheduling a follow-up
/// </summary>
public record ScheduleFollowUpApiRequest(
    Guid ProposalId,
    DateTime ScheduledFor,
    string? CustomMessage = null
);
