namespace ProposalPilot.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProposalPilot.Application.Features.Proposals.Commands.GenerateProposal;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Shared.DTOs.Proposal;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProposalsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ApplicationDbContext _context;

    public ProposalsController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        ApplicationDbContext context)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _context = context;
    }

    /// <summary>
    /// Generate a new proposal from a brief
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<Guid>> GenerateProposal([FromBody] GenerateProposalRequest request)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var command = new GenerateProposalCommand(
                _currentUserService.UserId.Value,
                request.BriefId,
                request.ClientId,
                request.PreferredTone,
                request.ProposalLength
            );

            var proposalId = await _mediator.Send(command);

            return Ok(new { proposalId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error generating proposal", error = ex.Message });
        }
    }

    /// <summary>
    /// Get a shared proposal by token (public, no auth required)
    /// </summary>
    [HttpGet("share/{token}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetSharedProposal(string token)
    {
        var proposal = await _context.Proposals
            .Include(p => p.Client)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.ShareToken == token && p.IsPublic);

        if (proposal == null)
            return NotFound(new { message = "Proposal not found or not publicly shared" });

        // Track view
        proposal.ViewCount++;
        if (proposal.FirstViewedAt == null)
            proposal.FirstViewedAt = DateTime.UtcNow;
        proposal.LastViewedAt = DateTime.UtcNow;

        // Create analytics record
        var analytics = new ProposalPilot.Domain.Entities.ProposalAnalytics
        {
            ProposalId = proposal.Id,
            ViewedAt = DateTime.UtcNow,
            ViewerUserAgent = Request.Headers["User-Agent"].ToString(),
            ViewerIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        };
        _context.ProposalAnalytics.Add(analytics);

        await _context.SaveChangesAsync();

        return Ok(proposal);
    }

    /// <summary>
    /// Get a proposal by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetProposal(Guid id)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        var proposal = await _context.Proposals
            .Include(p => p.Client)
            .Include(p => p.Brief)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == _currentUserService.UserId.Value);

        if (proposal == null)
            return NotFound();

        return Ok(proposal);
    }

    /// <summary>
    /// Get all proposals for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetProposals()
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        var proposals = await _context.Proposals
            .Include(p => p.Client)
            .Include(p => p.Brief)
            .Where(p => p.UserId == _currentUserService.UserId.Value)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(proposals);
    }

    /// <summary>
    /// Update a proposal
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProposal(Guid id, [FromBody] UpdateProposalRequest request)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var command = new ProposalPilot.Application.Features.Proposals.Commands.UpdateProposal.UpdateProposalCommand(
                id,
                _currentUserService.UserId.Value,
                request.Title,
                request.Description,
                request.Sections
            );

            var success = await _mediator.Send(command);

            if (!success)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating proposal", error = ex.Message });
        }
    }

    /// <summary>
    /// Toggle proposal public sharing
    /// </summary>
    [HttpPost("{id}/share/toggle")]
    public async Task<ActionResult> ToggleSharing(Guid id)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        var proposal = await _context.Proposals
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == _currentUserService.UserId.Value);

        if (proposal == null)
            return NotFound();

        proposal.IsPublic = !proposal.IsPublic;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            isPublic = proposal.IsPublic,
            shareToken = proposal.ShareToken,
            shareUrl = $"{Request.Scheme}://{Request.Host}/share/{proposal.ShareToken}"
        });
    }

    /// <summary>
    /// Get proposal analytics
    /// </summary>
    [HttpGet("{id}/analytics")]
    public async Task<ActionResult> GetProposalAnalytics(Guid id)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        var proposal = await _context.Proposals
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == _currentUserService.UserId.Value);

        if (proposal == null)
            return NotFound();

        var analytics = await _context.ProposalAnalytics
            .Where(a => a.ProposalId == id)
            .OrderByDescending(a => a.ViewedAt)
            .ToListAsync();

        return Ok(new
        {
            totalViews = proposal.ViewCount,
            firstViewedAt = proposal.FirstViewedAt,
            lastViewedAt = proposal.LastViewedAt,
            isPublic = proposal.IsPublic,
            shareToken = proposal.ShareToken,
            viewHistory = analytics
        });
    }
}
