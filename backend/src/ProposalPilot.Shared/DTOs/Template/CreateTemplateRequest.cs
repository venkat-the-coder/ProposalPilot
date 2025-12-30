namespace ProposalPilot.Shared.DTOs.Template;

/// <summary>
/// Request to create a new template
/// </summary>
public class CreateTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public TemplateContentDto Content { get; set; } = new();
    public PricingTemplateDto? DefaultPricing { get; set; }
    public bool IsPublic { get; set; } = false;
    public string? ThumbnailUrl { get; set; }
    public int? EstimatedTimeMinutes { get; set; }
}
