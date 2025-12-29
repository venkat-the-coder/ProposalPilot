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
}
