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
    private readonly ProposalPilot.Infrastructure.Services.IPdfExportService _pdfExportService;
    private readonly ProposalPilot.Infrastructure.Services.IDocxExportService _docxExportService;
    private readonly ILogger<ProposalsController> _logger;

    public ProposalsController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        ApplicationDbContext context,
        ProposalPilot.Infrastructure.Services.IPdfExportService pdfExportService,
        ProposalPilot.Infrastructure.Services.IDocxExportService docxExportService,
        ILogger<ProposalsController> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _context = context;
        _pdfExportService = pdfExportService;
        _docxExportService = docxExportService;
        _logger = logger;
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
            _logger.LogError(ex, "Error generating proposal for user {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An unexpected error occurred while generating the proposal" });
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
    /// Get all proposals for the current user with pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetProposals(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] bool sortDescending = true)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        // Validate pagination parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 100) pageSize = 100; // Max page size limit

        var query = _context.Proposals
            .Include(p => p.Client)
            .Include(p => p.Brief)
            .Where(p => p.UserId == _currentUserService.UserId.Value);

        // Apply status filter if provided
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ProposalPilot.Domain.Enums.ProposalStatus>(status, true, out var statusEnum))
        {
            query = query.Where(p => p.Status == statusEnum);
        }

        // Apply sorting
        query = sortBy?.ToLowerInvariant() switch
        {
            "title" => sortDescending
                ? query.OrderByDescending(p => p.Title)
                : query.OrderBy(p => p.Title),
            "status" => sortDescending
                ? query.OrderByDescending(p => p.Status)
                : query.OrderBy(p => p.Status),
            "viewcount" => sortDescending
                ? query.OrderByDescending(p => p.ViewCount)
                : query.OrderBy(p => p.ViewCount),
            _ => sortDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt)
        };

        // Get total count for pagination metadata
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Apply pagination
        var proposals = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            data = proposals,
            pagination = new
            {
                currentPage = page,
                pageSize,
                totalCount,
                totalPages,
                hasNextPage = page < totalPages,
                hasPreviousPage = page > 1
            }
        });
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
            _logger.LogError(ex, "Error updating proposal {ProposalId} for user {UserId}", id, _currentUserService.UserId);
            return StatusCode(500, new { message = "An unexpected error occurred while updating the proposal" });
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

    /// <summary>
    /// Score proposal quality with AI
    /// </summary>
    [HttpPost("{id}/score")]
    public async Task<ActionResult> ScoreProposal(Guid id)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var command = new ProposalPilot.Application.Features.Proposals.Commands.ScoreProposal.ScoreProposalCommand(
                id,
                _currentUserService.UserId.Value
            );

            var result = await _mediator.Send(command);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scoring proposal {ProposalId} for user {UserId}", id, _currentUserService.UserId);
            return StatusCode(500, new { message = "An unexpected error occurred while scoring the proposal" });
        }
    }

    /// <summary>
    /// Export proposal as PDF
    /// </summary>
    [HttpGet("{id}/export/pdf")]
    public async Task<ActionResult> ExportPdf(Guid id)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var proposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == _currentUserService.UserId.Value);

            if (proposal == null)
                return NotFound();

            var pdfBytes = _pdfExportService.GenerateProposalPdf(proposal);

            var fileName = $"{proposal.Title.Replace(" ", "_")}_{DateTime.UtcNow:yyyyMMdd}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting PDF for proposal {ProposalId}", id);
            return StatusCode(500, new { message = "An unexpected error occurred while exporting the PDF" });
        }
    }

    /// <summary>
    /// Export proposal as DOCX
    /// </summary>
    [HttpGet("{id}/export/docx")]
    public async Task<ActionResult> ExportDocx(Guid id)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var proposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == _currentUserService.UserId.Value);

            if (proposal == null)
                return NotFound();

            var docxBytes = _docxExportService.GenerateProposalDocx(proposal);

            var fileName = $"{proposal.Title.Replace(" ", "_")}_{DateTime.UtcNow:yyyyMMdd}.docx";

            return File(docxBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting DOCX for proposal {ProposalId}", id);
            return StatusCode(500, new { message = "An unexpected error occurred while exporting the document" });
        }
    }
}
