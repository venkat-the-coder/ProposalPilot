using ProposalPilot.Shared.DTOs.Template;

namespace ProposalPilot.Application.Interfaces;

/// <summary>
/// Service for managing proposal templates
/// </summary>
public interface ITemplateService
{
    /// <summary>
    /// Get all templates accessible to the user (system templates + user's templates)
    /// </summary>
    Task<List<TemplateListItemDto>> GetTemplatesAsync(
        Guid userId,
        string? category = null,
        string? searchTerm = null,
        bool includePublic = true,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// Get a specific template by ID
    /// </summary>
    Task<TemplateDto?> GetTemplateByIdAsync(Guid templateId, Guid userId);

    /// <summary>
    /// Get system templates only
    /// </summary>
    Task<List<TemplateListItemDto>> GetSystemTemplatesAsync();

    /// <summary>
    /// Get user's own templates
    /// </summary>
    Task<List<TemplateListItemDto>> GetUserTemplatesAsync(Guid userId);

    /// <summary>
    /// Create a new template
    /// </summary>
    Task<TemplateDto> CreateTemplateAsync(CreateTemplateRequest request, Guid userId);

    /// <summary>
    /// Update an existing template
    /// </summary>
    Task<TemplateDto?> UpdateTemplateAsync(Guid templateId, UpdateTemplateRequest request, Guid userId);

    /// <summary>
    /// Delete a template
    /// </summary>
    Task<bool> DeleteTemplateAsync(Guid templateId, Guid userId);

    /// <summary>
    /// Save an existing proposal as a template
    /// </summary>
    Task<TemplateDto> SaveProposalAsTemplateAsync(SaveAsTemplateRequest request, Guid userId);

    /// <summary>
    /// Create a proposal from a template
    /// </summary>
    Task<Guid> CreateProposalFromTemplateAsync(CreateFromTemplateRequest request, Guid userId);

    /// <summary>
    /// Duplicate/clone a template
    /// </summary>
    Task<TemplateDto> DuplicateTemplateAsync(Guid templateId, Guid userId);

    /// <summary>
    /// Get template categories with counts
    /// </summary>
    Task<Dictionary<string, int>> GetTemplateCategoriesAsync(Guid userId);

    /// <summary>
    /// Increment usage count for a template
    /// </summary>
    Task IncrementUsageCountAsync(Guid templateId);
}
