using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Shared.DTOs.Template;
using System.Security.Claims;

namespace ProposalPilot.API.Controllers;

/// <summary>
/// Manages proposal templates
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class TemplatesController : ControllerBase
{
    private readonly ITemplateService _templateService;
    private readonly ILogger<TemplatesController> _logger;

    public TemplatesController(ITemplateService templateService, ILogger<TemplatesController> logger)
    {
        _templateService = templateService;
        _logger = logger;
    }

    /// <summary>
    /// Get all templates (system, user's own, and public)
    /// </summary>
    /// <param name="category">Filter by category</param>
    /// <param name="searchTerm">Search by name, description, or tags</param>
    /// <param name="includePublic">Include public templates from other users</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20, max: 100)</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<TemplateListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TemplateListItemDto>>> GetTemplates(
        [FromQuery] string? category = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool includePublic = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetUserId();
            pageSize = Math.Min(pageSize, 100); // Cap at 100

            var templates = await _templateService.GetTemplatesAsync(
                userId, category, searchTerm, includePublic, page, pageSize);

            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving templates");
            return StatusCode(500, new { message = "An error occurred while retrieving templates" });
        }
    }

    /// <summary>
    /// Get a specific template by ID
    /// </summary>
    /// <param name="id">Template ID</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TemplateDto>> GetTemplateById(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var template = await _templateService.GetTemplateByIdAsync(id, userId);

            if (template == null)
            {
                return NotFound(new { message = "Template not found or access denied" });
            }

            return Ok(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving template {TemplateId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the template" });
        }
    }

    /// <summary>
    /// Get all system templates
    /// </summary>
    [HttpGet("system")]
    [ProducesResponseType(typeof(List<TemplateListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TemplateListItemDto>>> GetSystemTemplates()
    {
        try
        {
            var templates = await _templateService.GetSystemTemplatesAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system templates");
            return StatusCode(500, new { message = "An error occurred while retrieving system templates" });
        }
    }

    /// <summary>
    /// Get current user's templates
    /// </summary>
    [HttpGet("my-templates")]
    [ProducesResponseType(typeof(List<TemplateListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TemplateListItemDto>>> GetMyTemplates()
    {
        try
        {
            var userId = GetUserId();
            var templates = await _templateService.GetUserTemplatesAsync(userId);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user templates");
            return StatusCode(500, new { message = "An error occurred while retrieving your templates" });
        }
    }

    /// <summary>
    /// Get template categories with counts
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Dictionary<string, int>>> GetCategories()
    {
        try
        {
            var userId = GetUserId();
            var categories = await _templateService.GetTemplateCategoriesAsync(userId);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving template categories");
            return StatusCode(500, new { message = "An error occurred while retrieving categories" });
        }
    }

    /// <summary>
    /// Create a new template
    /// </summary>
    /// <param name="request">Template details</param>
    [HttpPost]
    [ProducesResponseType(typeof(TemplateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TemplateDto>> CreateTemplate([FromBody] CreateTemplateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var template = await _templateService.CreateTemplateAsync(request, userId);

            return CreatedAtAction(
                nameof(GetTemplateById),
                new { id = template.Id },
                template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating template");
            return StatusCode(500, new { message = "An error occurred while creating the template" });
        }
    }

    /// <summary>
    /// Update an existing template
    /// </summary>
    /// <param name="id">Template ID</param>
    /// <param name="request">Updated template details</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TemplateDto>> UpdateTemplate(Guid id, [FromBody] UpdateTemplateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var template = await _templateService.UpdateTemplateAsync(id, request, userId);

            if (template == null)
            {
                return NotFound(new { message = "Template not found or access denied" });
            }

            return Ok(template);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized template update attempt");
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {TemplateId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the template" });
        }
    }

    /// <summary>
    /// Delete a template
    /// </summary>
    /// <param name="id">Template ID</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTemplate(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var deleted = await _templateService.DeleteTemplateAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new { message = "Template not found or access denied" });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized template deletion attempt");
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {TemplateId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the template" });
        }
    }

    /// <summary>
    /// Save an existing proposal as a template
    /// </summary>
    /// <param name="request">Save request details</param>
    [HttpPost("save-from-proposal")]
    [ProducesResponseType(typeof(TemplateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TemplateDto>> SaveProposalAsTemplate([FromBody] SaveAsTemplateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var template = await _templateService.SaveProposalAsTemplateAsync(request, userId);

            return CreatedAtAction(
                nameof(GetTemplateById),
                new { id = template.Id },
                template);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid proposal save attempt");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving proposal as template");
            return StatusCode(500, new { message = "An error occurred while saving the proposal as a template" });
        }
    }

    /// <summary>
    /// Create a proposal from a template
    /// </summary>
    /// <param name="request">Proposal creation details</param>
    [HttpPost("create-proposal")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateProposalFromTemplate([FromBody] CreateFromTemplateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var proposalId = await _templateService.CreateProposalFromTemplateAsync(request, userId);

            return CreatedAtAction(
                "GetProposal",
                "Proposals",
                new { id = proposalId },
                new { proposalId });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid template creation attempt");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating proposal from template");
            return StatusCode(500, new { message = "An error occurred while creating the proposal" });
        }
    }

    /// <summary>
    /// Duplicate/clone a template
    /// </summary>
    /// <param name="id">Template ID to duplicate</param>
    [HttpPost("{id}/duplicate")]
    [ProducesResponseType(typeof(TemplateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TemplateDto>> DuplicateTemplate(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var template = await _templateService.DuplicateTemplateAsync(id, userId);

            return CreatedAtAction(
                nameof(GetTemplateById),
                new { id = template.Id },
                template);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid template duplication attempt");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error duplicating template {TemplateId}", id);
            return StatusCode(500, new { message = "An error occurred while duplicating the template" });
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }
}
