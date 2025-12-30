namespace ProposalPilot.Shared.DTOs.Template;

/// <summary>
/// Template list item (for browsing/searching)
/// </summary>
public class TemplateListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public bool IsSystemTemplate { get; set; }
    public bool IsPublic { get; set; }
    public string? UserName { get; set; }
    public int UsageCount { get; set; }
    public decimal? WinRate { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? EstimatedTimeMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
}
