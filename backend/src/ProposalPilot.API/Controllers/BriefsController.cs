namespace ProposalPilot.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProposalPilot.Application.Features.Briefs.Commands.AnalyzeBrief;
using ProposalPilot.Application.Features.Briefs.Commands.CreateBrief;
using ProposalPilot.Application.Features.Briefs.Queries.GetBrief;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Shared.DTOs.Brief;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BriefsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<BriefsController> _logger;

    public BriefsController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        ILogger<BriefsController> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new brief
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BriefDto>> CreateBrief([FromBody] CreateBriefRequest request)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        var command = new CreateBriefCommand(
            _currentUserService.UserId.Value,
            request.Title,
            request.RawContent
        );

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBrief), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get a brief by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BriefDto>> GetBrief(Guid id)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        var query = new GetBriefQuery(id, _currentUserService.UserId.Value);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Analyze a brief using Claude AI
    /// </summary>
    [HttpPost("{id}/analyze")]
    public async Task<ActionResult<BriefDto>> AnalyzeBrief(Guid id)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var command = new AnalyzeBriefCommand(id, _currentUserService.UserId.Value);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing brief {BriefId} for user {UserId}", id, _currentUserService.UserId);
            return StatusCode(500, new { message = "An unexpected error occurred while analyzing the brief" });
        }
    }
}
