namespace ProposalPilot.Domain.Entities;

/// <summary>
/// Represents a reusable proposal template
/// Templates can be system-wide or user-specific
/// </summary>
public class ProposalTemplate : BaseEntity
{
    /// <summary>
    /// Template name (e.g., "Web Development Proposal", "Marketing Campaign")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Brief description of when to use this template
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Industry/category (e.g., "Web Development", "Marketing", "Design")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Tags for searchability (JSON array: ["web", "design", "agency"])
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Template content structure (JSON with sections)
    /// Includes: introduction, problemStatement, solution, methodology, timeline, etc.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Default pricing structure (JSON with 3-tier pricing template)
    /// </summary>
    public string? DefaultPricing { get; set; }

    /// <summary>
    /// True for system templates (created by admin), False for user templates
    /// </summary>
    public bool IsSystemTemplate { get; set; } = false;

    /// <summary>
    /// True if template is publicly available in marketplace
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// User ID (null for system templates, set for user templates)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// How many times this template has been used
    /// </summary>
    public int UsageCount { get; set; } = 0;

    /// <summary>
    /// Average win rate for proposals created from this template (0-100)
    /// </summary>
    public decimal? WinRate { get; set; }

    /// <summary>
    /// Preview image URL (for template gallery)
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Estimated time to complete using this template (in minutes)
    /// </summary>
    public int? EstimatedTimeMinutes { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
}
